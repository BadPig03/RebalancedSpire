namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class SpinyToadPatch
{
    [HarmonyPatch(typeof(SpinyToad), nameof(SpinyToad.ExplosionDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceExplosionDamage(ref int __result)
    {
        __result -= 4;
    }

    [HarmonyPatch(typeof(SpinyToad), nameof(SpinyToad.LashDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceLashDamage(ref int __result)
    {
        __result -= 3;
    }
}