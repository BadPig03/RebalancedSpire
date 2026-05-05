namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Elite;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BygoneEffigyPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BygoneEffigyConfig;

    private static int StrengthPowerAmount => 3;

    private static async Task WakeMove(BygoneEffigy instance)
    {
        if (TestMode.IsOff)
        {
            NRunMusicController.Instance?.TriggerEliteSecondPhase();
        }
        await PowerCmd.Apply<StrengthPower>(instance.Creature, StrengthPowerAmount, instance.Creature, null);
        LocString line = MonsterModel.L10NMonsterLookup("BYGONE_EFFIGY.moves.SLEEP.speakLine2");
        TalkCmd.Play(line, instance.Creature, VfxColor.DarkGray, VfxDuration.Long);
        await Cmd.Wait(0.5f);
    }

    private static async Task SlashMove(BygoneEffigy instance, IReadOnlyList<Creature> targets)
    {
        if (TestMode.IsOff)
        {
            Vector2? vector = null;
            foreach (var target in targets)
            {
                var creatureNode = NCombatRoom.Instance?.GetCreatureNode(target);
                if (creatureNode != null && (!vector.HasValue || vector.Value.X > creatureNode.GlobalPosition.X))
                {
                    vector = creatureNode.GlobalPosition;
                }
            }
            var creatureNode2 = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
            var node2D = creatureNode2?.GetSpecialNode<Node2D>("Visuals/SpineBoneNode");
            if (creatureNode2 != null && node2D != null && vector.HasValue)
            {
                node2D.Position = Vector2.Left * (vector.Value.X - creatureNode2.GlobalPosition.X - 300f);
            }
        }
        NCombatRoom.Instance?.RadialBlur(VfxPosition.Left);
        await DamageCmd.Attack(instance.SlashDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.1f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
        await PowerCmd.Apply<StrengthPower>(instance.Creature, StrengthPowerAmount, instance.Creature, null);
        await Cmd.Wait(0.25f);
    }

    [HarmonyPatch(typeof(BygoneEffigy), nameof(BygoneEffigy.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(BygoneEffigy __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SLEEP_MOVE", __instance.InitialSleepMove, new SleepIntent());
        MoveState moveState2 = new MoveState("WAKE_MOVE", _ => WakeMove(__instance), new BuffIntent());
        MoveState moveState3 = new MoveState("SLASHES_MOVE", t => SlashMove(__instance, t), new SingleAttackIntent(__instance.SlashDamage), new BuffIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState3;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}