namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TunnelerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TunnelerConfig;

    private static readonly Func<Tunneler, IReadOnlyList<Creature>, Task>? _burrowMoveDelegate = Helpers.GetDelegate<Tunneler>("BurrowMove");
    private static readonly Func<Tunneler, IReadOnlyList<Creature>, Task>? _stillDizzyMoveDelegate = Helpers.GetDelegate<Tunneler>("StillDizzyMove");

    private static async Task BiteMove(Tunneler instance)
    {
        await DamageCmd.Attack(instance.BelowDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.25f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task BurrowMove(Tunneler instance, IReadOnlyList<Creature> targets)
    {
        if (_burrowMoveDelegate == null)
        {
            return;
        }

        await _burrowMoveDelegate(instance, targets);
    }

    private static async Task BelowMove(Tunneler instance, IReadOnlyList<Creature> targets)
    {
        if (TestMode.IsOff)
        {
            NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
            NCreature? targetNode = NCombatRoom.Instance?.GetCreatureNode(targets[0]);
            if (creatureNode == null || targetNode == null)
            {
                return;
            }

            Node2D? specialNode = creatureNode.GetSpecialNode<Node2D>("Visuals/SpineBoneNode");
            if (specialNode != null)
            {
                specialNode.Position = Vector2.Right * (targetNode.GlobalPosition.X - creatureNode.GlobalPosition.X) * 3f;
            }
            SfxCmd.Play("event:/sfx/enemy/enemy_attacks/burrowing_bug/burrowing_bug_hidden_attack");
            await CreatureCmd.TriggerAnim(instance.Creature, "BurrowAttack", 0.25f);
            await Cmd.Wait(1f);
        }
        await DamageCmd.Attack(instance.BiteDamage).FromMonster(instance).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task StillDizzyMove(Tunneler instance, IReadOnlyList<Creature> targets)
    {
        if (_stillDizzyMoveDelegate == null)
        {
            return;
        }

        await _stillDizzyMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(Tunneler), nameof(Tunneler.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix(Tunneler __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("BITE_MOVE", _ => BiteMove(__instance), new SingleAttackIntent(__instance.BelowDamage));
        MoveState moveState2 = new MoveState("BURROW_MOVE", t => BurrowMove(__instance, t), new BuffIntent(), new DefendIntent());
        MoveState moveState3 = new MoveState("BELOW_MOVE_1", t => BelowMove(__instance, t), new SingleAttackIntent(__instance.BiteDamage));
        MoveState moveState4 = new MoveState("DIZZY_MOVE", t => StillDizzyMove(__instance, t), new StunIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState3;
        moveState4.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        __result = new MonsterMoveStateMachine(list, moveState2);
        return false;
    }
}