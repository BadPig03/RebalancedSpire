namespace RebalancedSpire.Core.Afflictions;

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

public sealed class Devoured : AfflictionModel
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

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(
	[
		HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
	]).AsReadOnly();
}
