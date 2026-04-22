namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryOvergrowth)]
// ReSharper disable InconsistentNaming
public static class FogmogPatch
{
    [HarmonyPatch(typeof(Fogmog), nameof(Fogmog.HeadbuttDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceHeadbuttDamage(ref int __result)
    {
        __result -= 1;
    }
}