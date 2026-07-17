namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class TankPlusPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_tank_plus_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_tank_plus_power.png"
    );

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
        {
            return;
        }

        var allies = Owner.CombatState?.PlayerCreatures.Where(c => c != Owner).ToList();
        if (allies == null)
        {
            return;
        }

        Flash();
        foreach (var ally in allies)
        {
            await CreatureCmd.GainBlock(ally, Amount, ValueProp.Unpowered, null);
        }
    }
}