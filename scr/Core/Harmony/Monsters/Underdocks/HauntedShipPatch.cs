namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class HauntedShipPatch
{
    [HarmonyPatch(typeof(HauntedShip), nameof(HauntedShip.HauntDazed), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceHauntDazed(ref int __result)
    {
        __result -= 2;
    }
}