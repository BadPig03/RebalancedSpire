namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryOvergrowth)]
// ReSharper disable InconsistentNaming
public class InkletPatch
{
    private static int WhirlwindDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);
    private static int WhirlwindCount => 4;

    private static readonly Func<Inklet, IReadOnlyList<Creature>, Task>? _jabMoveDelegate = Helpers.GetDelegate<Inklet>("JabMove");
    private static readonly Func<Inklet, IReadOnlyList<Creature>, Task>? _piercingGazeMoveDelegate = Helpers.GetDelegate<Inklet>("PiercingGazeMove");

    private static async Task JabMove(Inklet instance, IReadOnlyList<Creature> targets)
    {
        if (_jabMoveDelegate == null)
        {
            return;
        }

        await _jabMoveDelegate(instance, targets);
    }

    private static async Task WhirlwindMove(Inklet instance)
    {
        await DamageCmd.Attack(WhirlwindDamage).WithHitCount(WhirlwindCount).FromMonster(instance).WithAttackerAnim("TRIPLE_ATTACK", 0.3f).OnlyPlayAnimOnce().WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/inklet/inklet_attack_triple").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private static async Task PiercingGazeMove(Inklet instance, IReadOnlyList<Creature> targets)
    {
        if (_piercingGazeMoveDelegate == null)
        {
            return;
        }

        await _piercingGazeMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(Inklet), nameof(Inklet.PiercingGazeDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReducePiercingGazeDamage(ref int __result)
    {
        __result -= 2;
    }

    [HarmonyPatch(typeof(Inklet), nameof(Inklet.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Inklet __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("JAB_MOVE", t => JabMove(__instance, t), new SingleAttackIntent(__instance.JabDamage));
        MoveState moveState2 = new MoveState("WHIRLWIND_MOVE", _ => WhirlwindMove(__instance), new MultiAttackIntent(WhirlwindDamage, WhirlwindCount));
        MoveState moveState3 = new MoveState("PIERCING_GAZE_MOVE", t => PiercingGazeMove(__instance, t), new SingleAttackIntent(__instance.PiercingGazeDamage));
        RandomBranchState randomBranchState = new RandomBranchState("INIT_RAND");
        RandomBranchState randomBranchState2 = (RandomBranchState) (moveState2.FollowUpState = moveState3.FollowUpState = moveState.FollowUpState = new RandomBranchState("RAND"));
        randomBranchState.AddBranch(moveState, 2, 1f);
        randomBranchState.AddBranch(moveState2, MoveRepeatType.CannotRepeat, 1f);
        randomBranchState2.AddBranch(moveState3, MoveRepeatType.CannotRepeat, 1f);
        randomBranchState2.AddBranch(moveState2, MoveRepeatType.CannotRepeat, 1f);
        moveState.FollowUpState = randomBranchState2;
        moveState2.FollowUpState = moveState;
        moveState3.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(randomBranchState2);
        MoveState initialState = __instance._middleInklet ? moveState2 : moveState;
        __result = new MonsterMoveStateMachine(list, initialState);
        return false;
    }
}