namespace RebalancedSpire.scr.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

    public override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if (player != Owner.Player)
        {
            return;
        }

        if (GetInternalData<Data>().ChanneledOrbs.Contains(orb.Id))
        {
            return;
        }

        GetInternalData<Data>().ChanneledOrbs.Add(orb.Id);
        await PowerCmd.Apply<FocusPower>(choiceContext, player.Creature, Amount, player.Creature, null);
    }
}