namespace RebalancedSpire.scr.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

public sealed class DisillusionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;
}