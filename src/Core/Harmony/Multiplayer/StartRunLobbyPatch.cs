namespace RebalancedSpire.Core.Harmony.Multiplayer;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Daily;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class StartRunLobbyPatch
{
    [HarmonyPatch(typeof(StartRunLobby), MethodType.Constructor, [typeof(GameMode), typeof(bool), typeof(INetGameService), typeof(IStartRunLobbyListener), typeof(int)])]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Constructor(StartRunLobby __instance)
    {
        RebalancedSpireSettingsSync.Bind(__instance.NetService, start: __instance);
    }

    [HarmonyPatch(typeof(StartRunLobby), MethodType.Constructor, [typeof(GameMode), typeof(bool), typeof(INetGameService), typeof(IStartRunLobbyListener), typeof(TimeServerResult), typeof(int)])]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_DailyConstructor(StartRunLobby __instance)
    {
        RebalancedSpireSettingsSync.Bind(__instance.NetService, start: __instance);
    }

    [HarmonyPatch(typeof(StartRunLobby), "BeginRunForAllPlayers")]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static bool PreFix_BeginRunForAllPlayers()
    {
        return RebalancedSpireSettingsSync.TryStart();
    }

    [HarmonyPatch(typeof(StartRunLobby), "BeginRunLocally")]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static void PreFix_BeginRunLocally()
    {
        RebalancedSpireSettingsSync.Freeze();
    }

    [HarmonyPatch(typeof(StartRunLobby), "SetReady")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_SetReady(bool ready)
    {
        return RebalancedSpireSettingsSync.TryReady(ready);
    }

    [HarmonyPatch(typeof(StartRunLobby), "CleanUp")]
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
