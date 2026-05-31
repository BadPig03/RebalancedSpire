namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class SandsOfTimePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_sands_of_time_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_sands_of_time_power.png"
    );

    public override bool ShouldTakeExtraTurn(Player player)
    {
        return Owner == player.Creature;
    }

    public override async Task AfterTakingExtraTurn(Player player)
    {
        if (Owner != player.Creature)
        {
            return;
        }

        Flash();
        await PowerCmd.Decrement(this);
    }
}