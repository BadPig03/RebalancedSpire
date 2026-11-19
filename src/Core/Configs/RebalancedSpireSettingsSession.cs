namespace RebalancedSpire.Core.Configs;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class RebalancedSpireSettingsSession
{
    internal const int Protocol = 1;

    internal static readonly PropertyInfo[] Properties = typeof(RebalancedSpireSettings).GetProperties();

    private readonly HashSet<ulong> _acknowledged = [];

    internal RebalancedSpireSettingsSnapshot? Snapshot { get; private set; }
    internal bool Frozen { get; private set; }
    private bool IsHost { get; set; }

    internal void Begin(bool host, RebalancedSpireSettings local)
    {
        Clear();
        IsHost = host;
        if (host)
        {
            Snapshot = new RebalancedSpireSettingsSnapshot(Protocol, RebalancedSpireMain.Version, Guid.NewGuid().ToString("N"), 1, Copy(local));
        }
    }

    internal bool UpdateHost(RebalancedSpireSettings settings)
    {
        if (!IsHost || Frozen || Snapshot == null || Equal(Snapshot.Settings, settings))
        {
            return false;
        }

        Snapshot = Snapshot with
        {
            Revision = Snapshot.Revision + 1,
            Settings = Copy(settings)
        };
        _acknowledged.Clear();
        return true;
    }

    internal bool Accept(RebalancedSpireSettingsSnapshot snapshot, bool fromHost)
    {
        if (IsHost || !fromHost || snapshot.Protocol != Protocol || snapshot.ModVersion != RebalancedSpireMain.Version || string.IsNullOrWhiteSpace(snapshot.SessionId) || snapshot.Revision < 1 || snapshot.Settings.SchemaVersion != RebalancedSpireSettings.CurrentSchemaVersion)
        {
            return false;
        }

        if (Snapshot is { } current && (snapshot.SessionId != current.SessionId || snapshot.Revision < current.Revision || ((Frozen || snapshot.Revision == current.Revision) && (snapshot.Revision != current.Revision || !Equal(snapshot.Settings, current.Settings)))))
        {
            return false;
        }

        Snapshot = snapshot with
        {
            Settings = Copy(snapshot.Settings)
        };
        return true;
    }

    internal void Acknowledge(ulong peer, string sessionId, long revision)
    {
        if (!IsHost || Snapshot is not { } snapshot || sessionId != snapshot.SessionId || revision != snapshot.Revision)
        {
            return;
        }

        _acknowledged.Add(peer);
    }

    private bool IsAcknowledged(ulong peer)
    {
        return _acknowledged.Contains(peer);
    }

    internal void RemovePeer(ulong peer)
    {
        _acknowledged.Remove(peer);
    }

    internal bool CanStart(IEnumerable<ulong> peers)
    {
        return Snapshot != null && (!IsHost || peers.All(IsAcknowledged));
    }

    internal void Freeze()
    {
        if (Snapshot == null)
        {
            throw new InvalidOperationException("Host settings have not been received.");
        }

        Frozen = true;
    }

    internal void Thaw()
    {
        Frozen = false;
    }

    internal void Clear()
    {
        Snapshot = null;
        IsHost = false;
        Frozen = false;
        _acknowledged.Clear();
    }

    internal static RebalancedSpireSettings Copy(RebalancedSpireSettings source)
    {
        var copy = new RebalancedSpireSettings();
        foreach (var property in Properties)
        {
            property.SetValue(copy, property.GetValue(source));
        }
        return copy;
    }

    internal static bool Equal(RebalancedSpireSettings left, RebalancedSpireSettings right)
    {
        return Properties.All(p => Equals(p.GetValue(left), p.GetValue(right)));
    }

    internal static RebalancedSpireSettingsSnapshot Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var settings = document.RootElement.GetProperty(nameof(RebalancedSpireSettingsSnapshot.Settings));
        foreach (var property in Properties)
        {
            var key = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
            var value = settings.GetProperty(key);
            if (property.PropertyType == typeof(bool) && value.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
            {
                throw new JsonException($"Invalid setting: {key}");
            }
        }
        return JsonSerializer.Deserialize<RebalancedSpireSettingsSnapshot>(json) ?? throw new JsonException("Missing settings snapshot.");
    }
}
