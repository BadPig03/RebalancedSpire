namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

public sealed class EternalArmorPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-eternal_armor_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-eternal_armor_power.png";
}