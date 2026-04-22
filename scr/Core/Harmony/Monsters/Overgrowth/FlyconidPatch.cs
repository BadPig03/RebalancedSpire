namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryOvergrowth)]
// ReSharper disable InconsistentNaming
public static class FlyconidPatch
{
    [HarmonyPatch(typeof(Flyconid), nameof(Flyconid.SmashDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSmashDamage(ref int __result)
    {
        __result -= 1;
    }
}