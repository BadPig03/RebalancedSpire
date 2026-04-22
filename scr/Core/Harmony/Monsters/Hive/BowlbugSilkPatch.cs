namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class BowlbugSilkPatch
{
    [HarmonyPatch(typeof(BowlbugSilk), nameof(BowlbugSilk.ThrashDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceThrashDamage(ref int __result)
    {
        __result -= 1;
    }
}