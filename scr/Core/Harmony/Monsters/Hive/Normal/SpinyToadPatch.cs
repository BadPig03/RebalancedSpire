namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SpinyToadPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SpinyToadConfig;

    [HarmonyPatch(typeof(SpinyToad), nameof(SpinyToad.ExplosionDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceExplosionDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 4;
    }

    [HarmonyPatch(typeof(SpinyToad), nameof(SpinyToad.LashDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceLashDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 3;
    }
}