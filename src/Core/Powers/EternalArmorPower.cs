namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class EternalArmorPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_eternal_armor_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_eternal_armor_power.png"
    );
}