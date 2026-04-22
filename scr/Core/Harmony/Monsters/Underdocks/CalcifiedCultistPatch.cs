namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class CalcifiedCultistPatch
{
    [HarmonyPatch(typeof(CalcifiedCultist), nameof(CalcifiedCultist.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(CalcifiedCultist __instance, ref int __result)
    {
        __result = __instance.MinInitialHp;
    }
}