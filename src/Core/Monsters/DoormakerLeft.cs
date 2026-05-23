namespace RebalancedSpire.Core.Monsters;

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Powers;

public sealed class DoormakerLeft : DoormakerBase
{
    private const int OmnidynamicsPowerAmount = 1;
    private const int ScrutinyCount = 4;
    private const int ScrutinyPlusPowerAmount = 1;

    private static int BeamDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);
    private static int FullAttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 28);
    private static int ScrutinyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private static int VulnerablePowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private static int WeakPowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

    public override bool ShouldShowInCompendium => false;

    public override async Task AfterAddedToRoom()
    {
        var body = NCombatRoom.Instance?.GetCreatureNode(Creature)?.Body;
        if (body != null)
        {
            body.Scale *= new Vector2(-1f, 1f);
        }

        await base.AfterAddedToRoom();
        foreach (var creature in CombatState.Enemies)
        {
            if (creature.Monster is not DoormakerRight doormaker)
            {
                continue;
            }

            OtherDoormaker = doormaker;
            break;
        }

        foreach (var player in CombatState.Players)
        {
            await PowerCmd.Apply<OmnidynamicsPower>(new ThrowingPlayerChoiceContext(), player.Creature, OmnidynamicsPowerAmount, Creature, null);
        }
    }

    protected override async Task Open()
    {
        await base.Open();
        UpdateVisual(EyeState);
        await Cmd.CustomScaledWait(0.2f, 0.6f);
    }

    private async Task DramaticOpenMove(IReadOnlyList<Creature> targets)
    {
        if (OtherDoormaker.Creature is { IsAlive: true, IsStunned: true })
        {
            MoveState moveState = new MoveState("READY_TO_SUMMON", _ => Task.CompletedTask, new SummonIntent())
            {
                FollowUpState = DramaticOpenState
            };
            SetMoveImmediate(moveState);
            return;
        }

        if (Creature.HpDisplay == HpDisplay.InfiniteWithoutNumbers)
        {
            await Open();
        }
        TalkCmd.Play(L10NMonsterLookup("DOORMAKER.moves.SCRUTINY.speakLine"), Creature, VfxColor.Purple);
        await Cmd.CustomScaledWait(0.2f, 0.6f);
        NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 1f);
    }

    private async Task ScrutinyMove(IReadOnlyList<Creature> targets)
    {
        if (Creature.HpDisplay == HpDisplay.InfiniteWithoutNumbers)
        {
            await Open();
        }
        await DamageCmd.Attack(ScrutinyDamage).WithHitCount(ScrutinyCount).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<ScrutinyPlusPower>(new ThrowingPlayerChoiceContext(), Creature, ScrutinyPlusPowerAmount, Creature, null);
    }

    private async Task BeamMove(IReadOnlyList<Creature> targets)
    {
        if (Creature.HpDisplay == HpDisplay.InfiniteWithoutNumbers)
        {
            await Open();
        }
        TalkCmd.Play(L10NMonsterLookup("DOORMAKER.moves.FULL_ATTACK.speakLine"), Creature, VfxColor.Purple);
        await Cmd.CustomScaledWait(0.2f, 0.6f);
        await DamageCmd.Attack(BeamDamage).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, VulnerablePowerAmount, Creature, null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, WeakPowerAmount, Creature, null);
    }

    private async Task FullAttackMove(IReadOnlyList<Creature> targets)
    {
        if (Creature.HpDisplay == HpDisplay.InfiniteWithoutNumbers)
        {
            await Open();
        }
        await PowerCmd.Remove<ScrutinyPlusPower>(Creature);
        await DamageCmd.Attack(FullAttackDamage).FromMonster(this).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await ReadyToSummon();
    }

    private async Task CloseMove(IReadOnlyList<Creature> targets)
    {
        await Close();
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SLEEP_MOVE", _ => Task.CompletedTask, new SleepIntent());
        DramaticOpenState = new MoveState("DRAMATIC_OPEN_MOVE", DramaticOpenMove, new SummonIntent());
        MoveState moveState3 = new MoveState("SCRUTINY_MOVE", ScrutinyMove, new MultiAttackIntent(ScrutinyDamage, ScrutinyCount), new CardDebuffIntent());
        MoveState moveState4 = new MoveState("BEAM_MOVE", BeamMove, new SingleAttackIntent(BeamDamage), new DebuffIntent());
        MoveState moveState5 = new MoveState("FULL_ATTACK_MOVE", FullAttackMove, new SingleAttackIntent(FullAttackDamage));
        MoveState moveState6 = new MoveState("CLOSE_MOVE", CloseMove, new EscapeIntent());
        MoveState moveState7 = new MoveState("CLOSED_MOVE", _ => Task.CompletedTask, new SleepIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("DOORMAKER_LEFT");
        branchState.AddState(moveState, () => !ShouldWakeUp());
        branchState.AddState(DramaticOpenState, ShouldWakeUp);
        ConditionalBranchState branchState2 = new ConditionalBranchState("DOORMAKER_LEFT_2");
        branchState2.AddState(moveState7, () => !ShouldWakeUp());
        branchState2.AddState(DramaticOpenState, ShouldWakeUp);
        ConditionalBranchState branchState3 = new ConditionalBranchState("DOORMAKER_LEFT_3");
        branchState3.AddState(moveState3, () => !OtherDoormaker.Creature.IsAlive);
        branchState3.AddState(moveState6, () => OtherDoormaker.Creature.IsAlive);
        moveState.FollowUpState = branchState;
        DramaticOpenState.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = branchState3;
        moveState6.FollowUpState = moveState7;
        moveState7.FollowUpState = branchState2;
        list.Add(moveState);
        list.Add(DramaticOpenState);
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