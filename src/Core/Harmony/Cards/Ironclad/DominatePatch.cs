namespace RebalancedSpire.Core.Harmony.Cards.Ironclad;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DominatePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Dominate;

    [HarmonyPatch(typeof(Dominate), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Dominate __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.RemoveKeyword(CardKeyword.Exhaust);
        return false;
    }
}