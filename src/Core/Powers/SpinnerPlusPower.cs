namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;

public sealed class SpinnerPlusPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-spinner_plus_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-spinner_plus_power.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<GlassOrb>()
    ]).AsReadOnly();

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        for (var i = 0; i < Amount; i++)
        {
            await OrbCmd.Channel<GlassOrb>(new ThrowingPlayerChoiceContext(), Owner.Player);
        }
        var orbs = Owner.Player.PlayerCombatState?.OrbQueue.Orbs.OfType<GlassOrb>().ToList();
        if (orbs == null)
        {
            return;
        }

        foreach (var orb in orbs)
        {
            await OrbCmd.Passive(new ThrowingPlayerChoiceContext(), orb, null);
        }
    }
}