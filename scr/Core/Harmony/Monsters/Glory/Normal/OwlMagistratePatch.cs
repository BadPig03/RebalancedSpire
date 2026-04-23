namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class OwlMagistratePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.OwlMagistrateConfig;

    [HarmonyPatch(typeof(OwlMagistrate), nameof(OwlMagistrate.ScrutinyDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceScrutinyDamage(OwlMagistrate __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 6;
    }

    [HarmonyPatch(typeof(OwlMagistrate), nameof(OwlMagistrate.VerdictDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceVerdictDamage(OwlMagistrate __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 6;
    }
}