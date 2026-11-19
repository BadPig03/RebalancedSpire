namespace RebalancedSpire.Core.Harmony.Multiplayer;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Multiplayer;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NetClientGameServicePatch
{
    [HarmonyPatch(typeof(NetClientGameService), "OnConnectedToHost")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void PreFix_OnConnectedToHost(NetClientGameService __instance)
    {
        RebalancedSpireSettingsSync.ClientConnected(__instance);
    }
}
