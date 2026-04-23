namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SnappingJaxfruitPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SnappingJaxfruitConfig;

    [HarmonyPatch(typeof(SnappingJaxfruit), nameof(SnappingJaxfruit.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 3;
    }

    [HarmonyPatch(typeof(SnappingJaxfruit), nameof(SnappingJaxfruit.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 3;
    }
}