namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DustyTomePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DustyTome;

    private static async Task AfterObtained(DustyTome instance)
    {
        var cards = new List<CardModel>();
        var options = new List<CardModel>
        {
            instance.Owner.Character.CardPool.GetUnlockedCards(instance.Owner.RunState.UnlockState, instance.Owner.RunState.CardMultiplayerConstraint).Where(c => c.Rarity == CardRarity.Ancient && !ArchaicTooth.TranscendenceCards.Contains(c)).ToList().StableShuffle(instance.Owner.PlayerRng.Rewards).First()
        };
        var otherCardPools = ModelDb.AllCharacterCardPools.Where(c => c != instance.Owner.Character.CardPool).ToList();
        options.AddRange(new CardCreationOptions(otherCardPools, CardCreationSource.Other, CardRarityOddsType.Uniform, c => c.Rarity == CardRarity.Ancient && !ArchaicTooth.TranscendenceCards.Contains(c)).GetPossibleCards(instance.Owner).ToList().StableShuffle(instance.Owner.PlayerRng.Rewards).Take(2));
        foreach (var card in options.Select(c => instance.Owner.RunState.CreateCard(c, instance.Owner)).ToList())
        {
            CardCmd.Upgrade(card);
            cards.Add(card);
        }
        var chosenCard = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), cards, instance.Owner, canSkip: false);
        if (chosenCard == null)
        {
            return;
        }

        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(chosenCard, PileType.Deck));
    }

    [HarmonyPatch(typeof(DustyTome), "AfterObtained")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(DustyTome __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterObtained(__instance);
        return false;
    }

    [HarmonyPatch(typeof(DustyTome), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(DustyTome __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(3),
            new StringVar("AncientCard")
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

        if (__instance is not DustyTome)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_DUSTY_TOME.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "EventDescription", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not DustyTome)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_DUSTY_TOME.eventDescription");
        return false;
    }

    [HarmonyPatch(typeof(DustyTome), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(DustyTome __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>().AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(DustyTome), "SetupForPlayer")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_SetupForPlayer(DustyTome __instance, Player player)
    {
        return Disabled;
    }
}