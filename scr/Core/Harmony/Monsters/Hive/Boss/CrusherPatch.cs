namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Boss;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx.Backgrounds;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CrusherPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.KaiserCrabConfig;

    private static readonly Func<Crusher, IReadOnlyList<Creature>, Task>? _thrashMoveDelegate = GetDelegate("ThrashMove");
    private static readonly Func<Crusher, IReadOnlyList<Creature>, Task>? _adaptMoveDelegate = GetDelegate("AdaptMove");
    private static readonly Func<Crusher, IReadOnlyList<Creature>, Task>? _guardedStrikeMoveDelegate = GetDelegate("GuardedStrikeMove");

    private static Func<Crusher, IReadOnlyList<Creature>, Task> GetDelegate(string name)
    {
        return (Func<Crusher, IReadOnlyList<Creature>, Task>) Delegate.CreateDelegate(typeof(Func<Crusher, IReadOnlyList<Creature>, Task>), null, AccessTools.Method(typeof(Crusher), name, [typeof(IReadOnlyList<Creature>)]));
    }

    private static async Task ThrashMove(Crusher instance, IReadOnlyList<Creature> targets)
    {
        if (_thrashMoveDelegate == null)
        {
            return;
        }

        await _thrashMoveDelegate(instance, targets);
    }

    private static async Task EnlargingStrikeMove(Crusher instance, IReadOnlyList<Creature> targets)
    {
        await instance.Background.PlayAttackAnim(NKaiserCrabBossBackground.ArmSide.Left, "attack_med", 0.65f);
        await PowerCmd.Apply<WeakPower>(targets, 2, instance.Creature, null);
    }

    private static async Task BugStingMove(Crusher instance, IReadOnlyList<Creature> targets)
    {
        await instance.Background.PlayAttackAnim(NKaiserCrabBossBackground.ArmSide.Left, "attack_double", 0.5f);
        await DamageCmd.Attack(instance.BugStingDamage).WithHitCount(instance.BugStingTimes).FromMonster(instance).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task AdaptMove(Crusher instance, IReadOnlyList<Creature> targets)
    {
        if (_adaptMoveDelegate == null)
        {
            return;
        }

        await _adaptMoveDelegate(instance, targets);
    }

    private static async Task GuardedStrikeMove(Crusher instance, IReadOnlyList<Creature> targets)
    {
        if (_guardedStrikeMoveDelegate == null)
        {
            return;
        }

        await _guardedStrikeMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(Crusher), nameof(Crusher.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 10;
    }

    [HarmonyPatch(typeof(Crusher), nameof(Crusher.AdaptStrengthGain), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceAdaptStrengthGain(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = 2;
    }

    [HarmonyPatch(typeof(Crusher), nameof(Crusher.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix(Crusher __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("THRASH_MOVE", t => ThrashMove(__instance, t), new SingleAttackIntent(__instance.ThrashDamage));
        MoveState moveState2 = new MoveState("ENLARGING_STRIKE_MOVE", t => EnlargingStrikeMove(__instance, t), new DebuffIntent());
        MoveState moveState3 = new MoveState("BUG_STING_MOVE", t => BugStingMove(__instance, t), new MultiAttackIntent(__instance.BugStingDamage, __instance.BugStingTimes));
        MoveState moveState4 = new MoveState("ADAPT_MOVE", t => AdaptMove(__instance, t), new BuffIntent());
        MoveState moveState5 = new MoveState("GUARDED_STRIKE_MOVE", t => GuardedStrikeMove(__instance, t), new SingleAttackIntent(__instance.GuardedStrikeDamage), new DefendIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}