namespace RebalancedSpire.Core.Harmony.Events;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ReflectionsPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Reflections;

    private const int CloneCardsLimit = 13;

    private static async Task TouchAMirror(Reflections instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        var cardModel = (await CardSelectCmd.FromDeckGeneric(instance.Owner, new CardSelectorPrefs(new LocString("card_selection", "REBALANCEDSPIRE-TO_DOWNGRADE"), 1), c => c.IsUpgraded)).FirstOrDefault();
        if (cardModel != null)
        {
            CardCmd.Downgrade(cardModel);
            CardCmd.Preview(cardModel);
            await Cmd.CustomScaledWait(0.5f, 1.2f);
        }
        var upgradableCards = instance.Owner.Deck.Cards.Where(c => c.IsUpgradable).ToList();
        for (var i = 0; i < 3; i++)
        {
            if (upgradableCards.Count <= 0)
            {
                break;
            }

            var card = instance.Rng.NextItem(upgradableCards);
            if (card == null)
            {
                break;
            }

            upgradableCards.Remove(card);
            CardCmd.Upgrade(card, CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.3f, 0.5f);
        }
        await Cmd.CustomScaledWait(0.6f, 1.2f);
        instance.SetEventFinished(instance.L10NLookup("REFLECTIONS.pages.TOUCH_A_MIRROR.description"));
    }

    private static async Task Stare(Reflections instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        foreach (var copiedCard in (await CardSelectCmd.FromDeckGeneric(instance.Owner, new CardSelectorPrefs(new LocString("card_selection", "REBALANCEDSPIRE-TO_COPY"), 0, CloneCardsLimit))).Select(c => instance.Owner.RunState.CloneCard(c)).ToList())
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(copiedCard, PileType.Deck), 1.2f, CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
        }
        await Cmd.CustomScaledWait(0.6f, 1.2f);
        await CardPileCmd.AddCurseToDeck<BadLuck>(instance.Owner);
        instance.SetEventFinished(instance.L10NLookup("REBALANCED_SPIRE_EVENT_REFLECTIONS.pages.STARE.description"));
    }

    private static async Task Shatter(Reflections instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        foreach (var copiedCard in instance.Owner.Deck.Cards.Select(c => instance.Owner.RunState.CloneCard(c)).ToList())
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(copiedCard, PileType.Deck), 1.2f, CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
        }
        await Cmd.CustomScaledWait(0.6f, 1.2f);
        instance.SetEventFinished(instance.L10NLookup("REFLECTIONS.pages.SHATTER.description"));
    }

    [HarmonyPatch(typeof(Reflections), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(Reflections __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            new(__instance, () => TouchAMirror(__instance), "REBALANCED_SPIRE_EVENT_REFLECTIONS.pages.INITIAL.options.TOUCH_A_MIRROR"),
            new(__instance, () => Stare(__instance), "REBALANCED_SPIRE_EVENT_REFLECTIONS.pages.INITIAL.options.STARE", HoverTipFactory.FromCardWithCardHoverTips<BadLuck>()),
            new(__instance, () => Shatter(__instance), "REBALANCED_SPIRE_EVENT_REFLECTIONS.pages.INITIAL.options.SHATTER")
        }.AsReadOnly();
        return false;
    }
}