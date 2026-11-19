namespace RebalancedSpire.Core.Harmony.Multiplayer;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class JoinFlowPatch
{
    [HarmonyPatch(typeof(JoinFlow), "Begin")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void PreFix_Begin(JoinFlow __instance)
    {
        RebalancedSpireSettingsSync.Bind(__instance.NetService);
    }

    [HarmonyPatch(typeof(JoinFlow), "Begin")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Begin(JoinFlow __instance, ref Task<JoinResult> __result)
    {
        __result = RebalancedSpireSettingsSync.CleanUpFailedJoin(__result, __instance.NetService);
    }

    [HarmonyPatch(typeof(JoinFlow), "AttemptJoin")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_AttemptJoin(JoinFlow __instance, ref Task<ClientLobbyJoinResponseMessage> __result)
    {
        __result = RebalancedSpireSettingsSync.AwaitJoin(__result, __instance, false);
    }

    [HarmonyPatch(typeof(JoinFlow), "AttemptLoadJoin")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_AttemptLoadJoin(JoinFlow __instance, ref Task<ClientLoadJoinResponseMessage> __result)
    {
        __result = RebalancedSpireSettingsSync.AwaitJoin(__result, __instance, false);
    }

    [HarmonyPatch(typeof(JoinFlow), "AttemptRejoin")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_AttemptRejoin(JoinFlow __instance, ref Task<ClientRejoinResponseMessage> __result)
    {
        __result = RebalancedSpireSettingsSync.AwaitJoin(__result, __instance, true);
    }
}
