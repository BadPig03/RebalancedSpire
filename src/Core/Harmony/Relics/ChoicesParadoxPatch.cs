namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ChoicesParadoxPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.ChoicesParadox;

    private static async Task AfterPlayerTurnStart(ChoicesParadox instance, PlayerChoiceContext choiceContext, Player player)
    {
        if (player != instance.Owner || player.PlayerCombatState?.TurnNumber != 1)
        {
            return;
        }

        instance.Flash();
        var cards = CardFactory.GetDistinctForCombat(player, player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint), instance.DynamicVars.Cards.IntValue, player.RunState.Rng.CombatCardGeneration).ToList();
        if (cards.Count == 0)
        {
            return;
        }

        foreach (var card in cards)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
            CardCmd.Upgrade(card);
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, player, new CardSelectorPrefs(RelicModel.L10NLookup("CHOICES_PARADOX.selectionScreenPrompt"), 1))).ToList();
        foreach (var card in selected)
        {
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        }
    }

    [HarmonyPatch(typeof(ChoicesParadox), "AfterPlayerTurnStart")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterPlayerTurnStart(ChoicesParadox __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterPlayerTurnStart(__instance, choiceContext, player);
        return false;
    }

    [HarmonyPatch(typeof(ChoicesParadox), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(ChoicesParadox __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(6)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ChoicesParadox)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_CHOICES_PARADOX.description");
        return false;
    }

}