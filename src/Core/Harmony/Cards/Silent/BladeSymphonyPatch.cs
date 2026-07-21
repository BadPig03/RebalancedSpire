namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BladeSymphonyPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.BladeSymphony;

    [HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not BladeSymphony)
        {
            return true;
        }

        __result = 1;
        return false;
    }

    [HarmonyPatch(typeof(BladeSymphony), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(BladeSymphony __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Cards.UpgradeValueBy(1);
        return false;
    }
}