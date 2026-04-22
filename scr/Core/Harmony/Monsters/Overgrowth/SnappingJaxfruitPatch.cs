namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryOvergrowth)]
// ReSharper disable InconsistentNaming
public static class SnappingJaxfruitPatch
{
    [HarmonyPatch(typeof(SnappingJaxfruit), nameof(SnappingJaxfruit.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        __result -= 3;
    }

    [HarmonyPatch(typeof(SnappingJaxfruit), nameof(SnappingJaxfruit.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(ref int __result)
    {
        __result -= 3;
    }
}