namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class DisillusionPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_disillusion_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_disillusion_power.png"
    );

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;
}