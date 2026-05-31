namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Elite;

using Configs;
using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MagiKnightPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Knights;

    private static int BombDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 45, 40);

    private static async Task PrepMove(MagiKnight instance)
    {
        TalkCmd.Play(new LocString("monsters", "MAGI_KNIGHT.PREP.banter"), instance.Creature, VfxColor.Orange, VfxDuration.Long);
        await Cmd.Wait(0.5f);
    }

    private static async Task Prep2Move(MagiKnight instance)
    {
        TalkCmd.Play(new LocString("monsters", "MAGI_KNIGHT.PREP_2.banter"), instance.Creature, VfxColor.Orange, VfxDuration.Long);
        await Cmd.Wait(0.5f);
    }

    private static async Task MagicBombMove(MagiKnight instance, IReadOnlyList<Creature> targets)
    {
        if (TestMode.IsOff)
        {
            Vector2? vector = null;
            foreach (Creature target in targets)
            {
                var creatureNode = NCombatRoom.Instance?.GetCreatureNode(target);
                if (creatureNode != null && (!vector.HasValue || vector.Value.X > creatureNode.GlobalPosition.X))
                {
                    vector = creatureNode.GlobalPosition;
                }
            }
            var creatureNode2 = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
            var specialNode = creatureNode2?.GetSpecialNode<Node2D>("Visuals/AttackDistanceControl");
            if (specialNode != null && creatureNode2 != null && vector.HasValue)
            {
                var x = creatureNode2.Visuals.GetCurrentBody().Scale.X;
                specialNode.Position = Vector2.Left * ((creatureNode2.GlobalPosition.X - vector.Value.X - 600f) / x);
            }
        }
        await DamageCmd.Attack(BombDamage).FromMonster(instance).WithAttackerAnim("BombCast", 1.2f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/magi_knight/magi_knight_attack_bomb").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    [HarmonyPatch(typeof(MagiKnight), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(MagiKnight __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("POWER_SHIELD_MOVE", __instance.PowerShieldMove, new SingleAttackIntent(__instance.PowerShieldDamage), new DefendIntent());
        MoveState moveState2 = new MoveState("DAMPEN_MOVE", __instance.DampenMove, new DebuffIntent());
        MoveState moveState3 = new MoveState("PREP_MOVE", _ => PrepMove(__instance), new UnknownIntent());
        MoveState moveState4 = new MoveState("PREP_2_MOVE", _ => Prep2Move(__instance), new UnknownIntent());
        MoveState moveState5 = new MoveState("MAGIC_BOMB", t => MagicBombMove(__instance, t), new SingleAttackIntent(BombDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState3;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}