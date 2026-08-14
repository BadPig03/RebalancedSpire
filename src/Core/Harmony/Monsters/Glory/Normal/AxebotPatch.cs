namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class AxebotPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Axebot;

    [HarmonyPatch(typeof(Axebot), "HammerUppercutDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceHammerUppercutDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }

    [HarmonyPatch(typeof(Axebot), "OneTwoDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceOneTwoDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}