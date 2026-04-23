namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Elite;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class InfestedPrismPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.InfestedPrismConfig;

    [HarmonyPatch(typeof(InfestedPrism), nameof(InfestedPrism.JabDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceJabDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }

    [HarmonyPatch(typeof(InfestedPrism), nameof(InfestedPrism.RadiateDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceRadiateDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }

    [HarmonyPatch(typeof(InfestedPrism), nameof(InfestedPrism.RadiateBlock), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceRadiateBlock(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }

    [HarmonyPatch(typeof(InfestedPrism), nameof(InfestedPrism.WhirlwindDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceWhirlwindDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}