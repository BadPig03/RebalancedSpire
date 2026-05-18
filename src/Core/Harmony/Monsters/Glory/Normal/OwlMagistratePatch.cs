namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class OwlMagistratePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.OwlMagistrateConfig;

    [HarmonyPatch(typeof(OwlMagistrate), "ScrutinyDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceScrutinyDamage(OwlMagistrate __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 3;
    }

    [HarmonyPatch(typeof(OwlMagistrate), "VerdictDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceVerdictDamage(OwlMagistrate __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 3;
    }
}