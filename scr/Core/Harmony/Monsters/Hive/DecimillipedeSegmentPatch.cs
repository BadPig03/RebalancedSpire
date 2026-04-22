namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class DecimillipedeSegmentPatch
{
    private static int BulkStrength => 1;
    private static int ReattachPowerAmount => 20;

    private static readonly Func<DecimillipedeSegment, IReadOnlyList<Creature>, Task>? _writheMoveDelegate = Helpers.GetDelegate<DecimillipedeSegment>("WritheMove");
    private static readonly Func<DecimillipedeSegment, IReadOnlyList<Creature>, Task>? _constrictMoveDelegate = Helpers.GetDelegate<DecimillipedeSegment>("ConstrictMove");
    private static readonly Func<DecimillipedeSegment, IReadOnlyList<Creature>, Task>? _reattachMoveDelegate = Helpers.GetDelegate<DecimillipedeSegment>("ReattachMove");

    private static async Task WritheMove(DecimillipedeSegment instance, IReadOnlyList<Creature> targets)
    {
        if (_writheMoveDelegate == null)
        {
            return;
        }

        await _writheMoveDelegate(instance, targets);
    }

    private static async Task BulkMove(DecimillipedeSegment instance)
    {
        await PowerCmd.Apply<StrengthPower>(instance.Creature, BulkStrength, instance.Creature, null);
    }

    private static async Task ConstrictMove(DecimillipedeSegment instance, IReadOnlyList<Creature> targets)
    {
        if (_constrictMoveDelegate == null)
        {
            return;
        }

        await _constrictMoveDelegate(instance, targets);
    }

    private static async Task ReattachMove(DecimillipedeSegment instance, IReadOnlyList<Creature> targets)
    {
        if (_reattachMoveDelegate == null)
        {
            return;
        }

        await _reattachMoveDelegate(instance, targets);
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
        List<Creature> source = (from c in instance.CombatState.GetTeammatesOf(instance.Creature) where c != instance.Creature select c).ToList();
        while (source.Any(c => c.MaxHp == maxHp))
        {
            maxHp += 2;
            if (maxHp > Creature.ScaleHpForMultiplayer(instance.MaxInitialHp, instance.CombatState.Encounter, count, currentActIndex))
            {
                maxHp = Creature.ScaleHpForMultiplayer(instance.MinInitialHp, instance.CombatState.Encounter, count, currentActIndex);
            }
        }
        await CreatureCmd.SetMaxAndCurrentHp(instance.Creature, maxHp);
        await PowerCmd.Apply<ReattachPower>(instance.Creature, ReattachPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(DecimillipedeSegment), nameof(DecimillipedeSegment.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(DecimillipedeSegment __instance, ref Task __result)
    {
        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(DecimillipedeSegment), nameof(DecimillipedeSegment.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(DecimillipedeSegment __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("WRITHE_MOVE", t => WritheMove(__instance, t), new MultiAttackIntent(__instance.WritheDamage, 2));
        MoveState moveState2 = new MoveState("BULK_MOVE", _ => BulkMove(__instance), new BuffIntent());
        MoveState moveState3 = new MoveState("CONSTRICT_MOVE", t => ConstrictMove(__instance, t), new SingleAttackIntent(__instance.ConstrictDamage), new DebuffIntent());
        __instance.DeadState = new MoveState("DEAD_MOVE", __instance.DeadMove);
        MoveState moveState4 = new MoveState("REATTACH_MOVE", t => ReattachMove(__instance, t), new HealIntent())
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