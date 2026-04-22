namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class TwoTailedRatPatch
{
    [HarmonyPatch(typeof(TwoTailedRat), nameof(TwoTailedRat.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(TwoTailedRat __instance, ref int __result)
    {
        __result = __instance.MinInitialHp;
    }
}