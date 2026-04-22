using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace RebalancedSpire.scr.Core.Powers;

public sealed class MinionFakePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPlayVfx => false;

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature == Owner || Owner.Monster is not KinFollower follower)
        {
            return Task.CompletedTask;
        }

        var enemies = Owner.CombatState?.Enemies;
        if (enemies == null)
        {
            return Task.CompletedTask;
        }

        var aliveFound = enemies.Where(enemy => enemy != Owner).Any(enemy => enemy.IsAlive);
        if (aliveFound)
        {
            return Task.CompletedTask;
        }

        var state = (MoveState?) follower.MoveStateMachine?.States["ESCAPE_MOVE"];
        if (state == null)
        {
            return Task.CompletedTask;
        }

        TalkCmd.Play(MonsterModel.L10NMonsterLookup("KIN_FOLLOWER_FAKE.escapeLine"), Owner, VfxColor.Purple, VfxDuration.Standard);
        follower.SetMoveImmediate(state);
        return Task.CompletedTask;
    }
}