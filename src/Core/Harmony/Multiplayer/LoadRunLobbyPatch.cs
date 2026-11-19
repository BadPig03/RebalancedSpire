namespace RebalancedSpire.Core.Harmony.Multiplayer;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Saves;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LoadRunLobbyPatch
{
    [HarmonyPatch(typeof(LoadRunLobby), MethodType.Constructor, [typeof(INetGameService), typeof(ILoadRunLobbyListener), typeof(SerializableRun)])]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Constructor(LoadRunLobby __instance)
    {
        RebalancedSpireSettingsSync.Bind(__instance.NetService, load: __instance);
    }

    [HarmonyPatch(typeof(LoadRunLobby), MethodType.Constructor, [typeof(INetGameService), typeof(ILoadRunLobbyListener), typeof(ClientLoadJoinResponseMessage)])]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_JoinConstructor(LoadRunLobby __instance)
    {
        RebalancedSpireSettingsSync.Bind(__instance.NetService, load: __instance);
    }

    [HarmonyPatch(typeof(LoadRunLobby), "TryBeginRunForAllPlayers")]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static bool PreFix_TryBeginRunForAllPlayers(ref Task __result)
    {
        if (RebalancedSpireSettingsSync.TryStart())
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(LoadRunLobby), "TryBeginRunForAllPlayers")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_TryBeginRunForAllPlayers(LoadRunLobby __instance, ref Task __result)
    {
        __result = RebalancedSpireSettingsSync.CompleteLoadAttempt(__result, __instance);
    }

    [HarmonyPatch(typeof(LoadRunLobby), "BeginRunLocally")]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static void PreFix_BeginRunLocally()
    {
        RebalancedSpireSettingsSync.Freeze();
    }

    [HarmonyPatch(typeof(LoadRunLobby), "SetReady")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_SetReady(bool ready)
    {
        return RebalancedSpireSettingsSync.TryReady(ready);
    }

    [HarmonyPatch(typeof(LoadRunLobby), "CleanUp")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_CleanUp(bool disconnectSession)
    {
        if (!disconnectSession)
        {
            return;
        }

        RebalancedSpireSettingsSync.Reset();
    }
}
