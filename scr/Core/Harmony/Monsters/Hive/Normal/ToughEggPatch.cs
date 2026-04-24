namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ToughEggPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.OvicopterConfig;

    private static readonly Func<ToughEgg, IReadOnlyList<Creature>, Task>? _hatchMoveDelegate = Helpers.GetDelegate<ToughEgg>("HatchMove");
    private static readonly Func<ToughEgg, IReadOnlyList<Creature>, Task>? _nibbleMoveDelegate = Helpers.GetDelegate<ToughEgg>("NibbleMove");

    private static async Task HatchMove(ToughEgg instance, IReadOnlyList<Creature> targets)
    {
        if (_hatchMoveDelegate == null)
        {
            return;
        }

        await _hatchMoveDelegate(instance, targets);
    }

    private static async Task NibbleMove(ToughEgg instance, IReadOnlyList<Creature> targets)
    {
        if (_nibbleMoveDelegate == null)
        {
            return;
        }

        await _nibbleMoveDelegate(instance, targets);
    }

    private static async Task AfterAddedToRoom(ToughEgg instance)
    {
        if (TestMode.IsOff && instance.HatchPos.HasValue)
        {
            NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
            if (creatureNode != null)
            {
                creatureNode.GlobalPosition = instance.HatchPos.Value;
            }
        }
        if (!instance.IsHatched)
        {
            var num = instance.CombatState.CurrentSide != CombatSide.Enemy ? 2 : 3;
            await PowerCmd.Apply<HatchPower>(new ThrowingPlayerChoiceContext(), instance.Creature, num, instance.Creature, null);
            return;
        }

        await instance.Hatch();
        if (instance.AfterHatchedState == null)
        {
            return;
        }

        instance.MoveStateMachine?.ForceCurrentState(instance.AfterHatchedState);
    }

    [HarmonyPatch(typeof(ToughEgg), nameof(ToughEgg.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(ToughEgg __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(ToughEgg), nameof(ToughEgg.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(ToughEgg __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("STUN_MOVE", _ => Task.CompletedTask, new StunIntent());
        MoveState moveState2 = new MoveState("HATCH_MOVE", t => HatchMove(__instance, t), new SummonIntent());
        MoveState moveState3 = new MoveState("NIBBLE_MOVE", t => NibbleMove(__instance, t), new SingleAttackIntent(ToughEgg.NibbleDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState3;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __instance.AfterHatchedState = moveState3;
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}