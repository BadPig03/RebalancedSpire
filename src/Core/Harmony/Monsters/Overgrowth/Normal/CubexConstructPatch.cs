namespace RebalancedSpire.Core.Harmony.Monsters.Overgrowth.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CubexConstructPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.CubexConstruct;

    private const int ExpelCount = 2;

    private static async Task RepeaterBlastMove(CubexConstruct instance)
    {
        SfxCmd.SetParam("event:/sfx/enemy/enemy_attacks/cubex_construct/cubex_construct_charge_attack", "loop", 1f);
        await Cmd.Wait(0.4f);
        await DamageCmd.Attack(instance.BlastDamage).FromMonster(instance).WithAttackerAnim("Attack", 0f).WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3").Execute(null);
        SfxCmd.SetParam("event:/sfx/enemy/enemy_attacks/cubex_construct/cubex_construct_charge_attack", "loop", 0f);
        await Cmd.Wait(0.2f);
        await CreatureCmd.TriggerAnim(instance.Creature, "AttackEnd", 0f);
    }

    [HarmonyPatch(typeof(CubexConstruct), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(CubexConstruct __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("CHARGE_UP_MOVE", __instance.ChargeUpMove, new BuffIntent());
        MoveState moveState2 = new MoveState("REPEATER_BLAST_MOVE", _ => RepeaterBlastMove(__instance), new SingleAttackIntent(__instance.BlastDamage));
        MoveState moveState3 = new MoveState("REPEATER_BLAST_MOVE_2", _ => RepeaterBlastMove(__instance), new SingleAttackIntent(__instance.BlastDamage));
        MoveState moveState4 = new MoveState("EXPEL_MOVE", __instance.ExpelBlastMove, new MultiAttackIntent(__instance.ExpelDamage, ExpelCount));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}