namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

public sealed class SandsOfTimePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

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