namespace RebalancedSpire.Core.Afflictions;

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterAffliction]
public sealed class Devoured : ModAfflictionTemplate
{
	private bool _appliedExhaust;

	public bool AppliedExhaust
	{
		get => _appliedExhaust;
		set
		{
			AssertMutable();
			_appliedExhaust = value;
		}
	}

	public override bool CanAfflictCardType(CardType cardType)
	{
		return cardType != CardType.Power;
	}

	protected override IEnumerable<IHoverTip> AdditionalHoverTips => new List<IHoverTip>(
	[
		HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
	]).AsReadOnly();
}
