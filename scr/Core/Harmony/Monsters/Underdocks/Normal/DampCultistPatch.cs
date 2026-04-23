namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DampCultistPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DampCultistConfig;

    [HarmonyPatch(typeof(DampCultist), nameof(DampCultist.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(DampCultist __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = __instance.MinInitialHp;
    }

    [HarmonyPatch(typeof(DampCultist), nameof(DampCultist.IncantationAmount), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceIncantationAmount(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}