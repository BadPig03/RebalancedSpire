namespace RebalancedSpire.Core.Harmony;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Nodes.Debug;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NDebugInfoLabelManagerPatch
{
    [HarmonyPatch(typeof(NDebugInfoLabelManager), "UpdateText")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_UpdateText(NDebugInfoLabelManager __instance)
    {
        __instance._moddedWarning.SetTextAutoSize($"{__instance._moddedWarning.Text}\n[{RebalancedSpireMain.Version}] RebalancedSpire");
    }
}