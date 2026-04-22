namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class DampCultistPatch
{
    [HarmonyPatch(typeof(DampCultist), nameof(DampCultist.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(DampCultist __instance, ref int __result)
    {
        __result = __instance.MinInitialHp;
    }

    [HarmonyPatch(typeof(DampCultist), nameof(DampCultist.IncantationAmount), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceIncantationAmount(ref int __result)
    {
        __result -= 1;
    }
}