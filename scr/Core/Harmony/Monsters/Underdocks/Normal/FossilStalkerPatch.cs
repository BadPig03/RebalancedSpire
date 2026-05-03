namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FossilStalkerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FossilStalkerConfig;

    [HarmonyPatch(typeof(FossilStalker), nameof(FossilStalker.TackleDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceTackleDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }

    [HarmonyPatch(typeof(FossilStalker), nameof(FossilStalker.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(FossilStalker __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("LASH_MOVE", __instance.LashAttack, new MultiAttackIntent(__instance.LashDamage, __instance.LashRepeat));
        MoveState moveState2 = new MoveState("TACKLE_MOVE", __instance.TackleMove, new SingleAttackIntent(__instance.TackleDamage), new DebuffIntent());
        MoveState moveState3 = new MoveState("LATCH_MOVE", __instance.LatchMove, new SingleAttackIntent(__instance.LatchDamage));
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