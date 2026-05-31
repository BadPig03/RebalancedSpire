namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class HandTrickPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.HandTrick;

    private static async Task OnPlay(HandTrick instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(instance.Owner.Creature, instance.DynamicVars.Block, cardPlay);
        var cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(instance.SelectionScreenPrompt, 1), context: choiceContext, player: instance.Owner, filter: card => !card.Keywords.Contains(CardKeyword.Sly), source: instance)).FirstOrDefault();
        if (cardModel == null)
        {
            return;
        }

        CardCmd.ApplyKeyword(cardModel, CardKeyword.Sly);
    }

    [HarmonyPatch(typeof(HandTrick), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(HandTrick __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(4, ValueProp.Move)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HandTrick)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Exhaust
        }.AsReadOnly();
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

        if (__instance is not HandTrick)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_HAND_TRICK.description");
        return false;
    }

    [HarmonyPatch(typeof(HandTrick), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(HandTrick __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }
}