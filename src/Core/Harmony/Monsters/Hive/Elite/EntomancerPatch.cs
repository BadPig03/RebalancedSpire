namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Elite;

using Configs;
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
public static class EntomancerPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Entomancer;

    private const int PersonalHivePowerAmountNone = 1;
    private const int PersonalHivePowerAmount = 2;
    private const int StrengthPowerAmount = 1;
    private const int StrengthPowerAmountLots = 2;

    private static async Task SpitMove(Entomancer instance)
    {
        SfxCmd.Play(SrcHelpers.GetSfx(instance, "CastSfx")!);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.5f);
        var personalHivePower = instance.Creature.Powers.OfType<PersonalHivePower>().FirstOrDefault();
        if (personalHivePower == null)
        {
            await PowerCmd.Apply<PersonalHivePower>(new ThrowingPlayerChoiceContext(), instance.Creature, PersonalHivePowerAmountNone, instance.Creature, null);
        }
        else if (personalHivePower.Amount < 3)
        {
            await PowerCmd.Apply<PersonalHivePower>(new ThrowingPlayerChoiceContext(), instance.Creature, PersonalHivePowerAmount, instance.Creature, null);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, StrengthPowerAmount, instance.Creature, null);
        }
        else
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, StrengthPowerAmountLots, instance.Creature, null);
        }
    }

    [HarmonyPatch(typeof(Entomancer), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(Entomancer), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Entomancer __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("BEES_MOVE", __instance.BeesMove, new MultiAttackIntent(__instance.BeesDamage, __instance.BeesRepeat));
        MoveState moveState2 = new MoveState("PHEROMONE_SPIT_MOVE", _ => SpitMove(__instance), new BuffIntent());
        MoveState moveState3 = new MoveState("SPEAR_MOVE", __instance.SpearMove, new SingleAttackIntent(__instance.SpearMoveDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState2);
        return false;
    }
}