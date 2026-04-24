namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ParafrightPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheObscuraConfig;

    private static int IllusionPowerAmount => 1;
    private static int DisillusionPowerAmount => 1;

    private static readonly Func<Parafright, IReadOnlyList<Creature>, Task>? _slamMoveDelegate = Helpers.GetDelegate<Parafright>("SlamMove");

    private static async Task SlamMove(Parafright instance, IReadOnlyList<Creature> targets)
    {
        if (_slamMoveDelegate == null)
        {
            return;
        }

        await _slamMoveDelegate(instance, targets);
    }

    private static async Task AfterAddedToRoom(Parafright instance)
    {
        await PowerCmd.Apply<IllusionPower>(new ThrowingPlayerChoiceContext(), instance.Creature, IllusionPowerAmount, instance.Creature, null);
        await PowerCmd.Apply<DisillusionPower>(new ThrowingPlayerChoiceContext(), instance.Creature, DisillusionPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(Parafright), nameof(Parafright.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 3;
    }

    [HarmonyPatch(typeof(Parafright), nameof(Parafright.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(Parafright __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(Parafright), nameof(Parafright.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Parafright __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SLAM_MOVE", t => SlamMove(__instance, t), new SingleAttackIntent(__instance.SlamDamage));
        moveState.FollowUpState = moveState;
        list.Add(moveState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}