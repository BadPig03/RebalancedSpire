namespace RebalancedSpire.Core.Harmony.Cards.Defect;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RocketPunchPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.RocketPunch;

    private static Task AfterCardGeneratedForCombat(RocketPunch instance, CardModel card, Player? creator)
    {
        if (creator != instance.Owner || card.Owner != instance.Owner || card.Type != CardType.Status)
        {
            return Task.CompletedTask;
        }

        instance.EnergyCost.SetUntilPlayed(0);
        return Task.CompletedTask;
    }

    [HarmonyPatch(typeof(RocketPunch), "AfterCardGeneratedForCombat")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCardGeneratedForCombat(RocketPunch __instance, CardModel card, Player? creator, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterCardGeneratedForCombat(__instance, card, creator);
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(CardModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not RocketPunch)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_ROCKET_PUNCH.description");
        return false;
    }
}