namespace RebalancedSpire.scr.Core.Powers;

using Afflictions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

public sealed class ScrutinyPlusPower : CustomPowerModel
{
    private static int MaxAllowedCardsInHand => 6;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var players = Owner.CombatState?.Players.ToList();
        if (players == null)
        {
            return;
        }

        foreach (var cardModel in players.Select(player => player.PlayerCombatState?.AllCards).OfType<IEnumerable<CardModel>>().SelectMany(list => list))
        {
            await Afflict(cardModel);
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        var hands = CardPile.GetCards(card.Owner, PileType.Hand).ToList();
        if (hands.Count <= MaxAllowedCardsInHand)
        {
            return;
        }

        Flash();
        for (var i = MaxAllowedCardsInHand; i < CardPile.MaxCardsInHand; i++)
        {
            var cardModel = hands.ElementAtOrDefault(i);
            if (cardModel == null)
            {
                continue;
            }

            await CardCmd.Discard(choiceContext, cardModel);
        }
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Affliction != null)
        {
            return;
        }

        await Afflict(card);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.CombatState == null)
        {
            return Task.CompletedTask;
        }

        foreach (Player player in oldOwner.CombatState.Players)
        {
            var list = player.PlayerCombatState?.AllCards.Where(c => c.Affliction is Weighted).ToList();
            if (list == null)
            {
                continue;
            }

            foreach (var cardModel in list)
            {
                CardCmd.ClearAffliction(cardModel);
            }
        }
        return Task.CompletedTask;
    }

    private async Task Afflict(CardModel card)
    {
        if (card.Affliction != null)
        {
            return;
        }

        await CardCmd.Afflict<Weighted>(card, Amount);
    }
}