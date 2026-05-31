namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class SynchronizePlusPower : ModPowerTemplate
{
    private class Data
    {
        public readonly List<ModelId> ChanneledOrbs = [];
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_synchronize_plus_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_synchronize_plus_power.png"
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new List<IHoverTip>(
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