namespace RebalancedSpire.Core.Configs;

using System.Collections.Concurrent;
using System.Text.Json;
using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib;
using STS2RitsuLib.Networking.Sidecar;
using STS2RitsuLib.Ui.Toast;

internal static class RebalancedSpireSettingsSync
{
    private const string Topic = "RebalancedSpire.settings";

    private static readonly RitsuLibSidecarMessageDescriptor<ControlMessage> ControlDescriptor = new(
        RebalancedSpireMain.ModId, "settings_control_v1", m => JsonSerializer.SerializeToUtf8Bytes(m),
        bytes => JsonSerializer.Deserialize<ControlMessage>(bytes)!);
    private static readonly ConcurrentQueue<(long Generation, Action Action)> Pending = new();
    private static readonly RebalancedSpireSettingsSession Session = new();
    private static INetGameService? _net;
    private static StartRunLobby? _startLobby;
    private static LoadRunLobby? _loadLobby;
    private static TaskCompletionSource _received = NewCompletion();
    private static long _generation;
    private static long _lastSend;
    private static bool _waitingToStart;
    private static bool _singleplayerFrozen;
    private static bool _initialized;
    private static bool _frameHooked;
    private static string _status = "";

    internal static bool Frozen => _singleplayerFrozen || Session.Frozen;
    private static bool IsClient => _net?.Type == NetGameType.Client;
    private static bool Ready => _net == null || _net.Type == NetGameType.Singleplayer || Session.Snapshot != null;

    private static TaskCompletionSource NewCompletion()
    {
        return new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    internal static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        RegisterEmptyTopic();
        RitsuLibSidecarEvents.OnConfigTopicChanged(e => Enqueue(() => ReceiveSnapshot(e)));
        RitsuLibSidecarTypedMessageRegistry.Subscribe(ControlDescriptor, e => Enqueue(() => ReceiveControl(e.SenderNetId, e.Message)));
        RitsuLibSidecarEvents.OnHandshakeCompleted(_ => Enqueue(() => _lastSend = 0));
        RitsuLibSidecarEvents.OnPeerReachabilityChanged(_ => Enqueue(() => _lastSend = 0));
        RitsuLibFramework.SubscribeLifecycle<GameReadyEvent>(_ => AttachFrameHook());
        AttachFrameHook();
    }

    private static void AttachFrameHook()
    {
        if (_frameHooked || Engine.GetMainLoop() is not SceneTree tree)
        {
            return;
        }

        tree.ProcessFrame += Tick;
        _frameHooked = true;
    }

    private static void Enqueue(Action action)
    {
        Pending.Enqueue((Interlocked.Read(ref _generation), action));
    }

    internal static void Bind(INetGameService net, StartRunLobby? start = null, LoadRunLobby? load = null)
    {
        if (!ReferenceEquals(_net, net))
        {
            Reset();
            _net = net;
            net.Disconnected += OnDisconnected;
            if (net is INetHostGameService host)
            {
                host.ClientDisconnected += OnPeerDisconnected;
            }
            Session.Begin(net.Type != NetGameType.Client, RebalancedSpireSettingsStore.Settings);
            if (Session.Snapshot is { } snapshot)
            {
                RebalancedSpireRuntimeSettings.Install(snapshot.Settings);
            }
            RegisterEmptyTopic();
            _lastSend = 0;
            if (net.Type == NetGameType.Host || net is INetClientGameService { NetClient: not null })
            {
                RitsuLibSidecarSessionManager.ObserveNetService(net);
            }
        }
        _startLobby = start ?? _startLobby;
        _loadLobby = load ?? _loadLobby;
    }

    internal static void ClientConnected(INetGameService net)
    {
        if (!ReferenceEquals(_net, net))
        {
            return;
        }

        RitsuLibSidecarSessionManager.ObserveNetService(net);
    }

    internal static void Reset()
    {
        Interlocked.Increment(ref _generation);
        if (_net != null)
        {
            _net.Disconnected -= OnDisconnected;
            if (_net is INetHostGameService host)
            {
                host.ClientDisconnected -= OnPeerDisconnected;
            }
        }
        _net = null;
        _startLobby = null;
        _loadLobby = null;
        _waitingToStart = false;
        _singleplayerFrozen = false;
        _status = "";
        _received.TrySetCanceled();
        _received = NewCompletion();
        Session.Clear();
        RebalancedSpireRuntimeSettings.RestoreLocal();
        RegisterEmptyTopic();
    }

    private static void OnDisconnected(NetErrorInfo _)
    {
        Enqueue(() =>
        {
            if (RunManager.Instance.State != null)
            {
                return;
            }

            Reset();
        });
    }

    private static void OnPeerDisconnected(ulong peer, NetErrorInfo _)
    {
        Enqueue(() => Session.RemovePeer(peer));
    }

    private static void RegisterEmptyTopic()
    {
        RitsuLibSidecarConfigSyncService.RegisterTopic<RebalancedSpireSettingsSnapshot?, JsonElement>(Topic, null, (_, _) => false, (state, _) => state);
    }

    internal static void LocalChanged()
    {
        if (Frozen || IsClient)
        {
            return;
        }

        RebalancedSpireRuntimeSettings.RestoreLocal();
        if (!Session.UpdateHost(RebalancedSpireSettingsStore.Settings))
        {
            return;
        }

        _lastSend = 0;
        Publish();
    }

    private static void Publish()
    {
        if (_net?.Type != NetGameType.Host || Session.Snapshot is not { } snapshot)
        {
            return;
        }

        RitsuLibSidecarConfigSyncService.RegisterTopic<RebalancedSpireSettingsSnapshot, JsonElement>(
            Topic, snapshot, (_, _) => false, (state, _) => state);
        RitsuLibSidecarConfigSyncService.PublishHostState(_net, Topic, _net.NetId, "host_settings");
    }

    private static void ReceiveSnapshot(SidecarConfigTopicChangedEvent e)
    {
        if (e.Topic != Topic || _net is not INetClientGameService client || client.NetClient == null || e.ChangedByPeer != client.NetClient.HostNetId || e.Reason != "snapshot")
        {
            return;
        }

        RebalancedSpireSettingsSnapshot snapshot;
        try
        {
            snapshot = RebalancedSpireSettingsSession.Parse(e.StateJson);
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            FailClient("sync.incompatible", NetError.ModMismatch);
            return;
        }

        if (snapshot.Protocol != RebalancedSpireSettingsSession.Protocol || snapshot.ModVersion != RebalancedSpireMain.Version)
        {
            FailClient("sync.incompatible", NetError.ModMismatch);
            return;
        }

        if (!Session.Accept(snapshot, true))
        {
            return;
        }

        RebalancedSpireRuntimeSettings.Install(Session.Snapshot!.Settings);
        RitsuLibSidecarTypedMessageRegistry.SendToHost(_net, ControlDescriptor, Message("ack"));
        _received.TrySetResult();
        ShowStatus("sync.ready");
    }

    private static ControlMessage Message(string kind)
    {
        return new ControlMessage(RebalancedSpireSettingsSession.Protocol, RebalancedSpireMain.Version, kind, Session.Snapshot?.SessionId ?? "", Session.Snapshot?.Revision ?? 0);
    }

    private static void ReceiveControl(ulong sender, ControlMessage message)
    {
        switch (_net)
        {
            case INetHostGameService host when host.ConnectedPeers.Any(p => p.peerId == sender):
            {
                if (message.Protocol != RebalancedSpireSettingsSession.Protocol || message.Version != RebalancedSpireMain.Version)
                {
                    RitsuLibSidecarTypedMessageRegistry.SendToPeer(_net, sender, ControlDescriptor, Message("incompatible"));
                    return;
                }

                switch (message.Kind)
                {
                    case "request":
                    {
                        Session.RemovePeer(sender);
                        Publish();
                        break;
                    }
                    case "ack":
                    {
                        Session.Acknowledge(sender, message.SessionId, message.Revision);
                        break;
                    }
                }
                break;
            }
            case INetClientGameService client when (sender == client.NetClient?.HostNetId && message.Kind == "incompatible"):
            {
                FailClient("sync.incompatible", NetError.ModMismatch);
                break;
            }
        }
    }

    private static void FailClient(string key, NetError reason)
    {
        ShowStatus(key);
        _received.TrySetException(new ClientConnectionFailedException(Text(key), new NetErrorInfo(reason, false)));
    }

    private static IEnumerable<ulong> Participants()
    {
        return _startLobby?.Players.Select(p => p.id).Where(id => id != _net?.NetId) ?? _loadLobby?.PlayerIds.Where(id => id != _net?.NetId) ?? (_net as INetHostGameService)?.ConnectedPeers.Select(p => p.peerId) ?? [];
    }

    internal static bool TryStart()
    {
        LocalChanged();
        if (_net?.Type == NetGameType.Host && !Session.CanStart(Participants()))
        {
            _waitingToStart = true;
            Publish();
            ShowStatus("sync.waiting_peers");
            return false;
        }

        if (!Ready)
        {
            ShowStatus("sync.waiting_host");
            return false;
        }

        _waitingToStart = false;
        Freeze();
        return true;
    }

    internal static void Freeze()
    {
        if (_net == null || _net.Type == NetGameType.Singleplayer)
        {
            if (!_singleplayerFrozen)
            {
                RebalancedSpireRuntimeSettings.RestoreLocal();
            }
            _singleplayerFrozen = true;
        }
        else
        {
            Session.Freeze();
        }
    }

    internal static bool TryReady(bool ready)
    {
        if (!ready || Ready)
        {
            return true;
        }

        ShowStatus("sync.waiting_host");
        return false;
    }

    internal static async Task<T> AwaitJoin<T>(Task<T> original, JoinFlow flow, bool rejoin)
    {
        var result = await original;
        var generation = _generation;
        if (!Ready)
        {
            ShowStatus("sync.waiting_host");
        }

        try
        {
            await _received.Task.WaitAsync(TimeSpan.FromSeconds(20), flow.CancelToken.Token);
        }
        catch (TimeoutException)
        {
            ShowStatus("sync.timeout");
            throw new ClientConnectionFailedException(Text("sync.timeout"), new NetErrorInfo(NetError.HandshakeTimeout, false));
        }

        if (generation != _generation || !ReferenceEquals(_net, flow.NetService))
        {
            throw new OperationCanceledException();
        }

        if (rejoin)
        {
            Freeze();
        }
        ShowStatus("sync.ready");
        return result;
    }

    internal static async Task<T> CleanUpFailedJoin<T>(Task<T> original, INetGameService net)
    {
        try
        {
            return await original;
        }
        catch
        {
            if (ReferenceEquals(_net, net))
            {
                Reset();
            }
            throw;
        }
    }

    internal static async Task CompleteLoadAttempt(Task original, LoadRunLobby lobby)
    {
        try
        {
            await original;
        }
        finally
        {
            if (ReferenceEquals(_net, lobby.NetService) && !lobby._isBeginningRun)
            {
                Session.Thaw();
                _singleplayerFrozen = false;
            }
        }
    }

    internal static void Tick()
    {
        while (Pending.TryDequeue(out var item))
        {
            if (item.Generation == _generation)
            {
                item.Action();
            }
        }
        if (_net is not { IsConnected: true } || _net.Type == NetGameType.Singleplayer)
        {
            return;
        }

        var now = System.Environment.TickCount64;
        if (now - _lastSend >= 1000)
        {
            _lastSend = now;
            if (_net.Type == NetGameType.Host)
            {
                LocalChanged();
                if (!Session.CanStart(((_net as INetHostGameService)?.ConnectedPeers ?? []).Select(p => p.peerId)))
                {
                    Publish();
                }
            }
            else if (Session.Snapshot == null)
            {
                RitsuLibSidecarTypedMessageRegistry.SendToHost(_net, ControlDescriptor, Message("request"));
            }
        }
        if (!_waitingToStart || !Session.CanStart(Participants()))
        {
            return;
        }

        _waitingToStart = false;
        _startLobby?.BeginRunForAllPlayersIfAllReady();
        _loadLobby?.BeginRunForAllPlayersIfAllReady();
    }

    private static string Text(string key)
    {
        return new LocString("settings_ui", key).GetFormattedText();
    }

    private static void ShowStatus(string key)
    {
        if (_status == key)
        {
            return;
        }

        _status = key;
        RebalancedSpireMain.Logger.Info(key);
        RitsuToastService.ShowInfo(Text(key), new LocString("settings_ui", "mod.title").GetFormattedText());
    }

    private sealed record ControlMessage(int Protocol, string Version, string Kind, string SessionId, long Revision);
}
