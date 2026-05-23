namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

public sealed class LeapPower : CustomTemporaryPowerModelWrapper<Leap, FocusPower>
{
    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-leap_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-leap_power.png";
}