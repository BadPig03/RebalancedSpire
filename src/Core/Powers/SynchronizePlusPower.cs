namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

public sealed class SynchronizePlusPower : CustomPowerModel
{
    private class Data
    {
        public readonly List<ModelId> ChanneledOrbs = [];
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-synchronize_plus_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-synchronize_plus_power.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.FromPower<FocusPower>()
    ]).AsReadOnly();

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if (player != Owner.Player || GetInternalData<Data>().ChanneledOrbs.Contains(orb.Id))
        {
            return;
        }

        GetInternalData<Data>().ChanneledOrbs.Add(orb.Id);
        await PowerCmd.Apply<FocusPower>(choiceContext, player.Creature, Amount, player.Creature, null);
    }
}