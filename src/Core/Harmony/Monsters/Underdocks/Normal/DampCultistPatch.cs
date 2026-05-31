namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DampCultistPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DampCultist;

    [HarmonyPatch(typeof(DampCultist), "IncantationAmount", MethodType.Getter)]
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

    [HarmonyPatch(typeof(DampCultist), "MaxInitialHp", MethodType.Getter)]
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
}