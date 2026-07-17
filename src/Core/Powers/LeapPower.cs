namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class LeapPower : ModTemporaryPowerTemplate
{
    public override AbstractModel OriginModel => ModelDb.Card<Leap>();

    public override PowerModel InternallyAppliedPower => ModelDb.Power<FocusPower>();

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_leap_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_leap_power.png"
    );
}