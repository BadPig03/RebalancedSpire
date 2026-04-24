namespace RebalancedSpire.scr.Core.Monsters;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;

public sealed class DoormakerRight : DoormakerBase
{
    private static int HungerDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 24, 22);
    private static int ChargeUpDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);
    private static int ChargeUpCount => 2;

    private static int StrengthPowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private static int FullAttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private static int FullAttackCount => 4;

    private async Task DramaticOpenMove(IReadOnlyList<Creature> targets)
    {
        await Open();
        UpdateVisual(MouthState);
        await Cmd.CustomScaledWait(0.2f, 0.6f);
        NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 1f);
    }

    private async Task HungerMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(HungerDamage).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<HungerPower>(new ThrowingPlayerChoiceContext(), Creature, 1, Creature, null);
    }

    private async Task ChargeUpMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ChargeUpDamage).WithHitCount(ChargeUpCount).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Remove<HungerPower>(Creature);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, StrengthPowerAmount, Creature, null);
    }

    private async Task FullAttackMove(IReadOnlyList<Creature> targets)
    {
        IsAboutToEscape = true;
        await DamageCmd.Attack(FullAttackDamage).WithHitCount(FullAttackCount).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private async Task CloseMove(IReadOnlyList<Creature> targets)
    {
        await Close();
    }

    private bool ShouldWakeUp()
    {
        DoormakerLeft? doormaker = null;
        foreach (Creature enemy in CombatState.Enemies)
        {
            if (enemy.Monster is not DoormakerLeft doormakerLeft)
            {
                continue;
            }

            doormaker = doormakerLeft;
            break;
        }

        return doormaker is null or { IsAboutToEscape: true, IsPortalOpen: true };
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
        if (wasRemovalPrevented || creature != Creature)
        {
            return;
        }

        await PowerCmd.Remove<HungerPower>(Creature);
    }

    public override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> list = [];
        DramaticOpenState = new MoveState("DRAMATIC_OPEN_MOVE", DramaticOpenMove, new SummonIntent());
        MoveState moveState2 = new MoveState("HUNGER_MOVE", HungerMove, new SingleAttackIntent(HungerDamage), new CardDebuffIntent());
        MoveState moveState3 = new MoveState("CHARGE_UP_MOVE", ChargeUpMove, new MultiAttackIntent(ChargeUpDamage, ChargeUpCount), new BuffIntent());
        MoveState moveState4 = new MoveState("FULL_ATTACK_MOVE", FullAttackMove, new MultiAttackIntent(FullAttackDamage, FullAttackCount));
        MoveState moveState5 = new MoveState("CLOSE_MOVE", CloseMove, new EscapeIntent());
        MoveState moveState6 = new MoveState("CLOSED_MOVE", _ => Task.CompletedTask, new SleepIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("DOORMAKER_RIGHT");
        branchState.AddState(moveState6, () => !ShouldWakeUp());
        branchState.AddState(DramaticOpenState, ShouldWakeUp);
        ConditionalBranchState branchState2 = new ConditionalBranchState("DOORMAKER_RIGHT_2");
        branchState2.AddState(moveState2, () => !ShouldClose());
        branchState2.AddState(moveState5, ShouldClose);
        DramaticOpenState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = branchState2;
        moveState5.FollowUpState = moveState6;
        moveState6.FollowUpState = branchState;
        list.Add(DramaticOpenState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(branchState);
        list.Add(branchState2);
        return new MonsterMoveStateMachine(list, DramaticOpenState);
    }
}