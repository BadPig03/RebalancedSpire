namespace RebalancedSpire.Core.Harmony.Runs;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
public static class RunManagerPatch
{
    [HarmonyPatch(typeof(RunManager), "SetUpNewSingleplayer")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void PreFix_SetUpNewSingleplayer()
    {
        RebalancedSpireSettingsSync.Freeze();
    }

    [HarmonyPatch(typeof(RunManager), "CleanUp")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_CleanUp()
    {
        RebalancedSpireSettingsSync.Reset();
    }
}
