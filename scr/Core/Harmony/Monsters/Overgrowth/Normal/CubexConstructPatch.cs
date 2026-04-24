namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CubexConstructPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.CubexConstructConfig;

    private static int ExpelCount => 2;

    private static readonly Func<CubexConstruct, IReadOnlyList<Creature>, Task>? _chargeUpMoveDelegate = Helpers.GetDelegate<CubexConstruct>("ChargeUpMove");
    private static readonly Func<CubexConstruct, IReadOnlyList<Creature>, Task>? _expelBlastMoveDelegate = Helpers.GetDelegate<CubexConstruct>("ExpelBlastMove");
    private static readonly Func<CubexConstruct, IReadOnlyList<Creature>, Task>? _submergeMoveDelegate = Helpers.GetDelegate<CubexConstruct>("SubmergeMove");

    private static async Task ChargeUpMove(CubexConstruct instance, IReadOnlyList<Creature> targets)
    {
        if (_chargeUpMoveDelegate == null)
        {
            return;
        }

        await _chargeUpMoveDelegate(instance, targets);
    }

    private static async Task RepeaterBlastMove(CubexConstruct instance)
    {
        SfxCmd.SetParam("event:/sfx/enemy/enemy_attacks/cubex_construct/cubex_construct_charge_attack", "loop", 1f);
        await Cmd.Wait(0.4f);
        await DamageCmd.Attack(instance.BlastDamage).FromMonster(instance).WithAttackerAnim("Attack", 0f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3").Execute(null);
        SfxCmd.SetParam("event:/sfx/enemy/enemy_attacks/cubex_construct/cubex_construct_charge_attack", "loop", 0f);
        await Cmd.Wait(0.2f);
        await CreatureCmd.TriggerAnim(instance.Creature, "AttackEnd", 0f);
    }

    private static async Task ExpelBlastMove(CubexConstruct instance, IReadOnlyList<Creature> targets)
    {
        if (_expelBlastMoveDelegate == null)
        {
            return;
        }

        await _expelBlastMoveDelegate(instance, targets);
    }

    private static async Task SubmergeMove(CubexConstruct instance, IReadOnlyList<Creature> targets)
    {
        if (_submergeMoveDelegate == null)
        {
            return;
        }

        await _submergeMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(CubexConstruct), nameof(CubexConstruct.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(CubexConstruct __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("CHARGE_UP_MOVE", t => ChargeUpMove(__instance, t), new BuffIntent());
        MoveState moveState2 = new MoveState("REPEATER_BLAST_MOVE", _ => RepeaterBlastMove(__instance), new SingleAttackIntent(__instance.BlastDamage));
        MoveState moveState3 = new MoveState("REPEATER_BLAST_MOVE_2", _ => RepeaterBlastMove(__instance), new SingleAttackIntent(__instance.BlastDamage));
        MoveState moveState4 = new MoveState("EXPEL_BLAST_MOVE", t => ExpelBlastMove(__instance, t), new MultiAttackIntent(__instance.ExpelDamage, ExpelCount));
        MoveState moveState5 = new MoveState("SUBMERGE_MOVE", t => SubmergeMove(__instance, t), new DefendIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}