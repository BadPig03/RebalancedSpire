namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

public sealed class PlowedPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-plow_plus_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-plow_plus_power.png";

    public override bool ShouldPlayVfx => false;

    protected override bool IsVisibleInternal => false;
}