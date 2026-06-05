namespace RebalancedSpire.Core.Powers;

using Afflictions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class HungerPower : ModPowerTemplate
{
	public override PowerType Type => PowerType.Debuff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override PowerAssetProfile AssetProfile => new(
		IconPath: "res://images/powers/rebalanced_spire_power_hunger_power.png",
		BigIconPath: "res://images/powers/rebalanced_spire_power_hunger_power.png"
	);

	protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromAffliction<Devoured>(Amount);

	public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
	{
		if (card.Owner != Owner.Player || card.Affliction is not Devoured)
		{
			return false;
		}

		return keywords.Add(CardKeyword.Exhaust);
	}

	public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		var cards = Owner.Player?.PlayerCombatState?.AllCards.Where(c => c.Type != CardType.Power).ToList();
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
		var cards = Owner.Player?.PlayerCombatState?.AllCards.Where(c => c.Type != CardType.Power && c.Affliction is Devoured).ToList();
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

		await PowerCmd.Decrement(this);
	}

	private async Task Afflict(CardModel card)
	{
		if (card.Affliction != null)
		{
			return;
		}

		await CardCmd.Afflict<Devoured>(card, Amount);
	}
}
