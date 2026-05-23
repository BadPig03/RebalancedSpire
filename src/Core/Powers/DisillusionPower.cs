namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

public sealed class DisillusionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-disillusion_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-disillusion_power.png";

    public override bool ShouldPlayVfx => false;

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new PowerVar<StrengthPower>(5)
    ]).AsReadOnly();
}