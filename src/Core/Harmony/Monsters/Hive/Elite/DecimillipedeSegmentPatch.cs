namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Elite;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DecimillipedeSegmentPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DecimillipedeConfig;

    private static int BulkStrength => 1;
    private static int ReattachPowerAmount => 20;

    private static async Task BulkMove(DecimillipedeSegment instance)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, BulkStrength, instance.Creature, null);
    }

    private static async Task AfterAddedToRoom(DecimillipedeSegment instance)
    {
        decimal maxHp = instance.Creature.MaxHp;
        if (maxHp % 2 == 1)
        {
            maxHp++;
        }
        var players = instance.CombatState.Players;
        var count = players.Count;
        var currentActIndex = instance.CombatState.RunState.CurrentActIndex;
        var source = instance.CombatState.Enemies.Where(c => c != instance.Creature).ToList();
        while (source.Any(c => c.MaxHp == maxHp))
        {
            maxHp += 2;
            if (maxHp > Creature.ScaleHpForMultiplayer(instance.MaxInitialHp, instance.CombatState.Encounter, count, currentActIndex))
            {
                maxHp = Creature.ScaleHpForMultiplayer(instance.MinInitialHp, instance.CombatState.Encounter, count, currentActIndex);
            }
        }
        await CreatureCmd.SetMaxAndCurrentHp(instance.Creature, maxHp);
        await PowerCmd.Apply<ReattachPower>(new ThrowingPlayerChoiceContext(), instance.Creature, ReattachPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(DecimillipedeSegment), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(DecimillipedeSegment __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(DecimillipedeSegment), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(DecimillipedeSegment __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("WRITHE_MOVE", __instance.WritheMove, new MultiAttackIntent(__instance.WritheDamage, 2));
        MoveState moveState2 = new MoveState("BULK_MOVE", _ => BulkMove(__instance), new BuffIntent());
        MoveState moveState3 = new MoveState("CONSTRICT_MOVE", __instance.ConstrictMove, new SingleAttackIntent(__instance.ConstrictDamage), new DebuffIntent());
        __instance.DeadState = new MoveState("DEAD_MOVE", __instance.DeadMove);
        MoveState moveState4 = new MoveState("REATTACH_MOVE", __instance.ReattachMove, new HealIntent())
        {
            MustPerformOnceBeforeTransitioning = true
        };
        moveState3.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState;
        moveState.FollowUpState = moveState3;
        RandomBranchState randomBranchState = new RandomBranchState("RAND");
        __instance.DeadState.FollowUpState = moveState4;
        moveState4.FollowUpState = randomBranchState;
        randomBranchState.AddBranch(moveState, MoveRepeatType.CannotRepeat);
        randomBranchState.AddBranch(moveState2, MoveRepeatType.CannotRepeat);
        randomBranchState.AddBranch(moveState3, MoveRepeatType.CannotRepeat);
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(__instance.DeadState);
        list.Add(moveState4);
        list.Add(randomBranchState);
        __result = new MonsterMoveStateMachine(list, (__instance.StarterMoveIdx % 3) switch
        {
            0 => moveState,
            1 => moveState2,
            _ => moveState3,
        });
        return false;
    }
}