namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DustyTomePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DustyTomeConfig;

    private static async Task AfterObtained(DustyTome instance)
    {
        var otherCardPools = ModelDb.AllCharacterCardPools.Where(cardPoolModel => cardPoolModel != instance.Owner.Character.CardPool).ToList();
        var otherOptions = instance.Owner.Character.CardPool.GetUnlockedCards(instance.Owner.RunState.UnlockState, instance.Owner.RunState.CardMultiplayerConstraint).Where(c => c.Rarity == CardRarity.Ancient && !ArchaicTooth.TranscendenceCards.Contains(c)).ToList().UnstableShuffle(instance.Owner.PlayerRng.Rewards).Take(1).Concat(new CardCreationOptions(otherCardPools, CardCreationSource.Other, CardRarityOddsType.Uniform, c => c.Rarity == CardRarity.Ancient && !ArchaicTooth.TranscendenceCards.Contains(c)).GetPossibleCards(instance.Owner).ToList().UnstableShuffle(instance.Owner.PlayerRng.Rewards).Take(2)).ToList();
        var chosenCard = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), otherOptions, instance.Owner, canSkip: false);
        if (chosenCard == null)
        {
            return;
        }

        var card = instance.Owner.RunState.CreateCard(chosenCard, instance.Owner);
        CardCmd.Upgrade(card);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
    }

    [HarmonyPatch(typeof(DustyTome), nameof(DustyTome.AfterObtained))]
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

    [HarmonyPatch(typeof(DustyTome), nameof(DustyTome.CanonicalVars), MethodType.Getter)]
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
            new CardsVar(3)
        };
        return false;
    }

    [HarmonyPatch(typeof(DustyTome), nameof(DustyTome.ExtraHoverTips), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(DustyTome __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = [];
        return false;
    }

    [HarmonyPatch(typeof(DustyTome), nameof(DustyTome.AncientCard), MethodType.Setter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AncientCard_Setter(DustyTome __instance)
    {
        return Disabled;
    }
}