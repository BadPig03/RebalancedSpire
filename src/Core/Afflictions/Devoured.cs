namespace RebalancedSpire.Core.Afflictions;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Monsters;
using Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterAffliction]
public sealed class Devoured : ModAfflictionTemplate
{
	public override bool CanAfflictCardType(CardType cardType)
	{
		return cardType != CardType.Power;
	}

	protected override IEnumerable<IHoverTip> AdditionalHoverTips => new List<IHoverTip>(
	[
		HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
	]).AsReadOnly();

	public override Task AfterCardEnteredCombat(CardModel card)
	{
		if (card != Card || card.Owner.Creature.HasPower<HungerPower>())
		{
			return Task.CompletedTask;
		}

		CardCmd.ClearAffliction(Card);
		return Task.CompletedTask;
	}
}
