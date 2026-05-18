namespace RebalancedSpire.Core.Harmony.Monsters.Overgrowth.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TwigSlimeMPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TwigSlimeMConfig;

    [HarmonyPatch(typeof(TwigSlimeM), "ClumpDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceClumpDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}