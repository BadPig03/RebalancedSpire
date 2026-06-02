namespace RebalancedSpire.Core.Powers;

using Afflictions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using IMaxHandSizeModifier = STS2RitsuLib.Combat.HandSize.IMaxHandSizeModifier;

[RegisterPower]
public sealed class ScrutinyPlusPower : ModPowerTemplate, IMaxHandSizeModifier
{
    private const int ReduceHandSize = 4;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_scrutiny_plus_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_scrutiny_plus_power.png"
    );

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var players = Owner.CombatState?.Players.ToList();
        if (players == null)
        {
            return;
        }

        var cards = players.Select(p => p.PlayerCombatState?.AllCards).OfType<List<CardModel>>().SelectMany(l => l).ToList();
        foreach (var card in cards)
        {
            await Afflict(card);
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

        foreach (var player in oldOwner.CombatState.Players)
        {
            var list = player.PlayerCombatState?.AllCards.Where(c => c.Affliction is Weighted).ToList();
            if (list == null)
            {
                continue;
            }

            foreach (var card in list)
            {
                CardCmd.ClearAffliction(card);
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