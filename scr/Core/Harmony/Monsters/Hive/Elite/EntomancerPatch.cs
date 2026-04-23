namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Elite;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class EntomancerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.EntomancerConfig;

    private static readonly Func<Entomancer, IReadOnlyList<Creature>, Task>? _spearMoveDelegate = Helpers.GetDelegate<Entomancer>("SpearMove");
    private static readonly Func<Entomancer, IReadOnlyList<Creature>, Task>? _beesMoveDelegate = Helpers.GetDelegate<Entomancer>("BeesMove");

    private static async Task BeesMove(Entomancer instance, IReadOnlyList<Creature> targets)
    {
        if (_beesMoveDelegate == null)
        {
            return;
        }

        await _beesMoveDelegate(instance, targets);
    }

    private static async Task SpitMove(Entomancer instance)
    {
        SfxCmd.Play(instance.CastSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.5f);
        PersonalHivePower? personalHivePower = instance.Creature.Powers.OfType<PersonalHivePower>().FirstOrDefault();
        if (personalHivePower == null)
        {
            await PowerCmd.Apply<PersonalHivePower>(instance.Creature, 1, instance.Creature, null);
        } else if (personalHivePower.Amount < 3)
        {
            await PowerCmd.Apply<PersonalHivePower>(instance.Creature, 1, instance.Creature, null);
            await PowerCmd.Apply<StrengthPower>(instance.Creature, 1, instance.Creature, null);
        }
        else
        {
            await PowerCmd.Apply<StrengthPower>(instance.Creature, 2, instance.Creature, null);
        }
    }

    private static async Task SpearMove(Entomancer instance, IReadOnlyList<Creature> targets)
    {
        if (_spearMoveDelegate == null)
        {
            return;
        }

        await _spearMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(Entomancer), nameof(Entomancer.AfterAddedToRoom))]
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

    [HarmonyPatch(typeof(Entomancer), nameof(Entomancer.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Entomancer __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("BEES_MOVE", t => BeesMove(__instance, t), new MultiAttackIntent(__instance.BeesDamage, __instance.BeesRepeat));
        MoveState moveState2 = new MoveState("PHEROMONE_SPIT_MOVE", _ => SpitMove(__instance), new BuffIntent());
        MoveState moveState3 = new MoveState("SPEAR_MOVE", t => SpearMove(__instance, t), new SingleAttackIntent(__instance.SpearMoveDamage));
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