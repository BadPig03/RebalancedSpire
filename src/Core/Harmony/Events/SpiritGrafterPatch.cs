namespace RebalancedSpire.Core.Harmony.Events;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SpiritGrafterPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.SpiritGrafter;

    private static async Task StrikeBack(SpiritGrafter instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        var cards = (await CardSelectCmd.FromDeckForRemoval(prefs: new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1), player: instance.Owner, filter: c => c.Type == CardType.Attack)).ToList();
        await CardPileCmd.RemoveFromDeck(cards);
        instance.SetEventFinished(instance.L10NLookup("REBALANCED_SPIRE_EVENT_SPIRIT_GRAFTER.pages.STRIKE_BACK.description"));
    }

    [HarmonyPatch(typeof(SpiritGrafter), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(SpiritGrafter __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            new(__instance, __instance.LetItIn, "SPIRIT_GRAFTER.pages.INITIAL.options.LET_IT_IN", HoverTipFactory.FromCardWithCardHoverTips<Metamorphosis>()),
            __instance.Owner?.Deck.Cards.Any(c => c.Type == CardType.Attack) == true ? new EventOption(__instance, () => StrikeBack(__instance), "REBALANCED_SPIRE_EVENT_SPIRIT_GRAFTER.pages.INITIAL.options.STRIKE_BACK") : new EventOption(__instance, null, "REBALANCED_SPIRE_EVENT_SPIRIT_GRAFTER.pages.INITIAL.options.STRIKE_BACK_LOCKED"),
            new EventOption(__instance, __instance.Rejection, "SPIRIT_GRAFTER.pages.INITIAL.options.REJECTION").ThatDoesDamage(__instance.DynamicVars["RejectionHpLoss"].BaseValue)
        }.AsReadOnly();
        return false;
    }
}