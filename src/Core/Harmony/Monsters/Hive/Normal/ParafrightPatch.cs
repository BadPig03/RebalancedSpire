namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Normal;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ParafrightPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.TheObscura;

    private const int IllusionPowerAmount = 1;
    private const int DisillusionPowerAmount = 5;

    private static async Task AfterAddedToRoom(Parafright instance)
    {
        await PowerCmd.Apply<IllusionPower>(new ThrowingPlayerChoiceContext(), instance.Creature, IllusionPowerAmount, instance.Creature, null);
        await PowerCmd.Apply<DisillusionPower>(new ThrowingPlayerChoiceContext(), instance.Creature, DisillusionPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(Parafright), "AfterAddedToRoom")]
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

    [HarmonyPatch(typeof(Parafright), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Parafright __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SLAM_MOVE", __instance.SlamMove, new SingleAttackIntent(__instance.SlamDamage));
        moveState.FollowUpState = moveState;
        list.Add(moveState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}