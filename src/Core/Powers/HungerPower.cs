namespace RebalancedSpire.Core.Powers;

using Afflictions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class HungerPower : ModPowerTemplate
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override PowerAssetProfile AssetProfile => new(
		IconPath: "res://images/powers/rebalanced_spire_power_hunger_power.png",
		BigIconPath: "res://images/powers/rebalanced_spire_power_hunger_power.png"
	);

	protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromAffliction<Devoured>(Amount);

	public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		var players = Owner.CombatState?.Players.ToList();
		if (players == null)
		{
			return;
		}

		foreach (var card in players.Select(player => player.PlayerCombatState?.AllCards.Where(c => c.Type is CardType.Attack or CardType.Skill).ToList()).OfType<List<CardModel>>().SelectMany(l => l))
		{
			await Afflict(card);
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
		var players = oldOwner.CombatState?.Players.ToList();
		if (players == null)
		{
			return Task.CompletedTask;
		}

		foreach (var card in players.Select(player => player.PlayerCombatState?.AllCards.Where(c => c.Affliction is Devoured).ToList()).OfType<List<CardModel>>().SelectMany(l => l))
		{
			if (((Devoured?)card.Affliction)?.AppliedExhaust == true)
			{
				CardCmd.RemoveKeyword(card, CardKeyword.Exhaust);
			}

			CardCmd.ClearAffliction(card);
		}

		return Task.CompletedTask;
	}

	private async Task Afflict(CardModel card)
	{
		if (card.Affliction != null)
		{
			return;
		}

		var devoured = await CardCmd.Afflict<Devoured>(card, Amount);
		if (devoured == null || card.Keywords.Contains(CardKeyword.Exhaust))
		{
			return;
		}

		CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
		devoured.AppliedExhaust = true;
	}
}
