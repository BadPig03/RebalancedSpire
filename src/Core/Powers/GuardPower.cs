namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

public sealed class GuardPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-guard_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-guard_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new StringVar("MasterName", ModelDb.Monster<KinPriest>().Title.GetFormattedText())
    ]).AsReadOnly();

    public override bool ShouldAllowTargeting(Creature target)
    {
        if (target == Owner || target.Monster is not KinPriest)
        {
            return true;
        }

        var enemies = target.CombatState?.Enemies;
        if (enemies == null)
        {
            return true;
        }

        foreach (var enemy in enemies)
        {
            if (enemy.Monster is not KinFollower { StartsWithDance: false })
            {
                continue;
            }

            return enemy.Block == 0;
        }
        return true;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            return;
        }

        await PowerCmd.Remove(this);
    }
}