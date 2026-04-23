namespace RebalancedSpire.scr.Core.Powers;

using Afflictions;
using BaseLib.Abstracts;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;

public sealed class ScrutinyPlusPower : CustomPowerModel, IMaxHandSizeModifier
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new CardsVar(3)
    };

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var players = Owner.CombatState?.Players;
        if (players == null)
        {
            return;
        }

        foreach (Player player in players)
        {
            var list = player.PlayerCombatState?.AllCards;
            if (list == null)
            {
                continue;
            }

            foreach (CardModel cardModel in list)
            {
                await Afflict(cardModel);
            }
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

            foreach (CardModel cardModel in list)
            {
                CardCmd.ClearAffliction(cardModel);
            }
        }
        return Task.CompletedTask;
    }

    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        return currentMaxHandSize - DynamicVars.Cards.IntValue;
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