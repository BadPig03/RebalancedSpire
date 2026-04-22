namespace RebalancedSpire.scr.Core.Monsters;

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Powers;

public sealed class DoormakerLeft : DoormakerBase
{
    private static int ScrutinyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private static int ScrutinyCount => 4;
    private static int VulnerablePowerAmount => 4;
    private static int WeakPowerAmount => 4;
    private static int FullAttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 28);

    private async Task OpenMove(IReadOnlyList<Creature> targets)
    {
        await Open();
        UpdateVisual(EyeState);
        await Cmd.CustomScaledWait(0.2f, 0.6f);
        NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 1f);
    }

    private async Task ScrutinyMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ScrutinyDamage).WithHitCount(ScrutinyCount).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<ScrutinyPlusPower>(Creature, 1, Creature, null);
    }

    private async Task DebuffMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<VulnerablePower>(targets, VulnerablePowerAmount, Creature, null);
        await PowerCmd.Apply<WeakPower>(targets, WeakPowerAmount, Creature, null);
    }

    private async Task FullAttackMove(IReadOnlyList<Creature> targets)
    {
        IsAboutToEscape = true;
        await PowerCmd.Remove<ScrutinyPlusPower>(Creature);
        await DamageCmd.Attack(FullAttackDamage).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private async Task CloseMove(IReadOnlyList<Creature> targets)
    {
        await Close();
    }

    public override async Task AfterAddedToRoom()
    {
        Node2D? body = NCombatRoom.Instance?.GetCreatureNode(Creature)?.Body;
        if (body != null)
        {
            body.Scale *= new Vector2(-1f, 1f);
        }

        await base.AfterAddedToRoom();
        foreach (Player player in CombatState.Players)
        {
            await PowerCmd.Apply<OmnidynamicsPower>(player.Creature, 1, Creature, null);
        }
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
        if (wasRemovalPrevented || creature != Creature)
        {
            return;
        }

        await PowerCmd.Remove<ScrutinyPlusPower>(Creature);
    }

    private bool ShouldWakeUp()
    {
        DoormakerRight? doormaker = null;
        foreach (Creature enemy in CombatState.Enemies)
        {
            if (enemy.Monster is not DoormakerRight doormakerRight)
            {
                continue;
            }

            doormaker = doormakerRight;
            break;
        }

        return doormaker is null or { IsAboutToEscape: true, IsPortalOpen: true };
    }

    public override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SLEEP_MOVE", _ => Task.CompletedTask, new SleepIntent());
        MoveState moveState2 = new MoveState("OPEN_MOVE", OpenMove, new SummonIntent());
        MoveState moveState3 = new MoveState("SCRUTINY_MOVE", ScrutinyMove, new MultiAttackIntent(ScrutinyDamage, ScrutinyCount), new CardDebuffIntent());
        MoveState moveState4 = new MoveState("DEBUFF_MOVE", DebuffMove, new DebuffIntent());
        MoveState moveState5 = new MoveState("FULL_ATTACK_MOVE", FullAttackMove, new SingleAttackIntent(FullAttackDamage));
        MoveState moveState6 = new MoveState("CLOSE_MOVE", CloseMove, new EscapeIntent());
        MoveState moveState7 = new MoveState("CLOSED_MOVE", _ => Task.CompletedTask, new SleepIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("DOORMAKER_LEFT");
        branchState.AddState(moveState, () => !ShouldWakeUp());
        branchState.AddState(moveState2, ShouldWakeUp);
        ConditionalBranchState branchState2 = new ConditionalBranchState("DOORMAKER_LEFT_2");
        branchState2.AddState(moveState7, () => !ShouldWakeUp());
        branchState2.AddState(moveState2, ShouldWakeUp);
        ConditionalBranchState branchState3 = new ConditionalBranchState("DOORMAKER_LEFT_3");
        branchState3.AddState(moveState3, () => !ShouldClose());
        branchState3.AddState(moveState6, ShouldClose);
        moveState.FollowUpState = branchState;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = branchState3;
        moveState6.FollowUpState = moveState7;
        moveState7.FollowUpState = branchState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(moveState7);
        list.Add(branchState);
        list.Add(branchState2);
        list.Add(branchState3);
        return new MonsterMoveStateMachine(list, moveState);
    }
}