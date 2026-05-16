namespace RebalancedSpire.Core.Powers;

using Afflictions;
using BaseLib.Abstracts;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

public sealed class ScrutinyPlusPower : CustomPowerModel, IMaxHandSizeModifier
{
    private static int ReduceHandSize => 4;

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

    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        return currentMaxHandSize - ReduceHandSize;
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