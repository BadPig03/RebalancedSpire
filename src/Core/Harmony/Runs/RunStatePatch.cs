namespace RebalancedSpire.Core.Harmony.Runs;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
public static class RunStatePatch
{
    [HarmonyPatch(typeof(RunState), "FromSerializable")]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static void PreFix_FromSerializable()
    {
        RebalancedSpireSettingsSync.Freeze();
    }
}
