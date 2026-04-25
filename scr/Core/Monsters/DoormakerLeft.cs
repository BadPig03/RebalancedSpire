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
    private static int BeamDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 16);
    private static int VulnerablePowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private static int WeakPowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private static int FullAttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 32, 30);

    private async Task DramaticOpenMove(IReadOnlyList<Creature> targets)
    {
        await Open();
        UpdateVisual(EyeState);
        await Cmd.CustomScaledWait(0.2f, 0.6f);
        NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 1f);
    }

    private async Task ScrutinyMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ScrutinyDamage).WithHitCount(ScrutinyCount).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<ScrutinyPlusPower>(new ThrowingPlayerChoiceContext(), Creature, 1, Creature, null);
    }

    private async Task BeamMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BeamDamage).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, VulnerablePowerAmount, Creature, null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, WeakPowerAmount, Creature, null);
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
            await PowerCmd.Apply<OmnidynamicsPower>(new ThrowingPlayerChoiceContext(), player.Creature, 1, Creature, null);
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
        SleepState = new MoveState("SLEEP_MOVE", _ => Task.CompletedTask, new SleepIntent());
        DramaticOpenState = new MoveState("DRAMATIC_OPEN_MOVE", DramaticOpenMove, new SummonIntent());
        MoveState moveState3 = new MoveState("SCRUTINY_MOVE", ScrutinyMove, new MultiAttackIntent(ScrutinyDamage, ScrutinyCount), new CardDebuffIntent());
        MoveState moveState4 = new MoveState("BEAM_MOVE", BeamMove, new SingleAttackIntent(BeamDamage), new DebuffIntent());
        MoveState moveState5 = new MoveState("FULL_ATTACK_MOVE", FullAttackMove, new SingleAttackIntent(FullAttackDamage));
        MoveState moveState6 = new MoveState("CLOSE_MOVE", CloseMove, new EscapeIntent());
        MoveState moveState7 = new MoveState("CLOSED_MOVE", _ => Task.CompletedTask, new SleepIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("DOORMAKER_LEFT");
        branchState.AddState(SleepState, () => !ShouldWakeUp());
        branchState.AddState(DramaticOpenState, ShouldWakeUp);
        ConditionalBranchState branchState2 = new ConditionalBranchState("DOORMAKER_LEFT_2");
        branchState2.AddState(moveState7, () => !ShouldWakeUp());
        branchState2.AddState(DramaticOpenState, ShouldWakeUp);
        ConditionalBranchState branchState3 = new ConditionalBranchState("DOORMAKER_LEFT_3");
        branchState3.AddState(moveState3, () => !ShouldClose());
        branchState3.AddState(moveState6, ShouldClose);
        SleepState.FollowUpState = branchState;
        DramaticOpenState.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = branchState3;
        moveState6.FollowUpState = moveState7;
        moveState7.FollowUpState = branchState2;
        list.Add(SleepState);
        list.Add(DramaticOpenState);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(moveState7);
        list.Add(branchState);
        list.Add(branchState2);
        list.Add(branchState3);
        return new MonsterMoveStateMachine(list, SleepState);
    }
}