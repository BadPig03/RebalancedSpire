namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class FossilStalkerPatch
{
    private static readonly Func<FossilStalker, IReadOnlyList<Creature>, Task>? _tackleMoveDelegate = Helpers.GetDelegate<FossilStalker>("TackleMove");
    private static readonly Func<FossilStalker, IReadOnlyList<Creature>, Task>? _latchMoveDelegate = Helpers.GetDelegate<FossilStalker>("LatchMove");
    private static readonly Func<FossilStalker, IReadOnlyList<Creature>, Task>? _lashAttackDelegate = Helpers.GetDelegate<FossilStalker>("LashAttack");

    private static async Task LashAttack(FossilStalker instance, IReadOnlyList<Creature> targets)
    {
        if (_lashAttackDelegate == null)
        {
            return;
        }

        await _lashAttackDelegate(instance, targets);
    }

    private static async Task TackleMove(FossilStalker instance, IReadOnlyList<Creature> targets)
    {
        if (_tackleMoveDelegate == null)
        {
            return;
        }

        await _tackleMoveDelegate(instance, targets);
    }

    private static async Task LatchMove(FossilStalker instance, IReadOnlyList<Creature> targets)
    {
        if (_latchMoveDelegate == null)
        {
            return;
        }

        await _latchMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(FossilStalker), nameof(FossilStalker.TackleDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceTackleDamage(ref int __result)
    {
        __result -= 1;
    }

    [HarmonyPatch(typeof(FossilStalker), nameof(FossilStalker.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix(FossilStalker __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("LASH_MOVE", t => LashAttack(__instance, t), new MultiAttackIntent(__instance.LashDamage, __instance.LashRepeat));
        MoveState moveState2 = new MoveState("TACKLE_MOVE", t => TackleMove(__instance, t), new SingleAttackIntent(__instance.TackleDamage), new DebuffIntent());
        MoveState moveState3 = new MoveState("LATCH_MOVE", t => LatchMove(__instance, t), new SingleAttackIntent(__instance.LatchDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}