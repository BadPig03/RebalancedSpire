namespace RebalancedSpire.Core.Harmony.Encounters;

using Core.Cards;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ByrdonisElitePatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.GodByrd;

    private static async Task TransformCards(Task original, Byrdonis instance)
    {
        await original;
        foreach (var player in instance.CombatState.Players)
        {
            var combatCards = player.PlayerCombatState!.AllCards.OfType<GodByrd>().ToList();
            var deckCards = player.Deck.Cards.OfType<GodByrd>().ToDictionary(CardModel (c) => c, CreateReplacement);
            await CardCmd.Transform(deckCards.Select(pair => new CardTransformation(pair.Key, pair.Value)), null, CardPreviewStyle.None);
            var transformations = new List<CardTransformation>();
            foreach (var card in combatCards)
            {
                var replacement = CreateReplacement(card);
                if (card.DeckVersion != null && deckCards.TryGetValue(card.DeckVersion, out var deckVersion))
                {
                    replacement.DeckVersion = deckVersion;
                }
                transformations.Add(new CardTransformation(card, replacement));
            }
            await CardCmd.Transform(transformations, null);
        }
    }

    private static CardModel CreateReplacement(CardModel original)
    {
        var replacement = original.CardScope!.CreateCard<GodByrdAwakened>(original.Owner);
        if (original.IsUpgraded)
        {
            CardCmd.Upgrade(replacement);
        }
        return replacement;
    }

    [HarmonyPatch(typeof(Byrdonis), "AfterAddedToRoom")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_AfterAddedToRoom(Byrdonis __instance, ref Task __result)
    {
        if (Disabled || __instance.CombatState.Encounter is not ByrdonisElite)
        {
            return;
        }

        __result = TransformCards(__result, __instance);
    }
}
