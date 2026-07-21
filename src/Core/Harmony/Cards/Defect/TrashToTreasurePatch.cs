namespace RebalancedSpire.Core.Harmony.Cards.Defect;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TrashToTreasurePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.TrashToTreasure;

    [HarmonyPatch(typeof(TrashToTreasure), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(TrashToTreasure __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.AddKeyword(CardKeyword.Innate);
        return false;
    }
}