namespace RebalancedSpire.Core.Powers;

using Afflictions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using IMaxHandSizeModifier = STS2RitsuLib.Combat.HandSize.IMaxHandSizeModifier;

[RegisterPower]
public sealed class ScrutinyPower : ModPowerTemplate, IMaxHandSizeModifier
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_scrutiny_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_scrutiny_power.png"
    );

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var cards = Owner.Player?.PlayerCombatState?.AllCards.ToList();
        if (cards == null)
        {
            return;
        }

        foreach (var card in cards)
        {
            await Afflict(card);
        }
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner != Owner.Player)
        {
            return;
        }

        await Afflict(card);
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != Applier)
        {
            return;
        }

        await PowerCmd.Remove(this);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        var cards = Owner.Player?.PlayerCombatState?.AllCards.Where(c => c.Affliction is Weighted).ToList();
        if (cards == null)
        {
            return Task.CompletedTask;
        }

        foreach (var card in cards)
        {
            CardCmd.ClearAffliction(card);
        }
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || side != CombatSide.Player)
        {
            return;
        }

        await PowerCmd.ModifyAmount(choiceContext, this, -2, Applier, null);
    }

    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        if (player != Owner.Player)
        {
            return currentMaxHandSize;
        }

        return currentMaxHandSize - Amount;
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