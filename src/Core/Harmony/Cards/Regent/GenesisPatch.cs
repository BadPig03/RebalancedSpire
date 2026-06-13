namespace RebalancedSpire.Core.Harmony.Cards.Regent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class GenesisPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Genesis;

    [HarmonyPatch(typeof(Genesis), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Genesis __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}