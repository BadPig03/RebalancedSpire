namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class SlumberingBeetlePatch
{
    [HarmonyPatch(typeof(SlumberingBeetle), nameof(SlumberingBeetle.RolloutDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceRolloutDamage(ref int __result)
    {
        __result -= 2;
    }
}