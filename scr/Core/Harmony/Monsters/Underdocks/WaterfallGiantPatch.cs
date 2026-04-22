namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class WaterfallGiantPatch
{
    private static readonly Func<WaterfallGiant, IReadOnlyList<Creature>, Task>? _pressurizeMoveDelegate = Helpers.GetDelegate<WaterfallGiant>("PressurizeMove");
    private static readonly Func<WaterfallGiant, IReadOnlyList<Creature>, Task>? _siphonMoveDelegate = Helpers.GetDelegate<WaterfallGiant>("SiphonMove");
    private static readonly Func<WaterfallGiant, IReadOnlyList<Creature>, Task>? _pressureUpMoveDelegate = Helpers.GetDelegate<WaterfallGiant>("PressureUpMove");
    private static readonly Func<WaterfallGiant, IReadOnlyList<Creature>, Task>? _aboutToBlowMoveDelegate = Helpers.GetDelegate<WaterfallGiant>("AboutToBlowMove");
    private static readonly Func<WaterfallGiant, IReadOnlyList<Creature>, Task>? _explodeMoveDelegate = Helpers.GetDelegate<WaterfallGiant>("ExplodeMove");

    private static async Task PressurizeMove(WaterfallGiant instance, IReadOnlyList<Creature> targets)
    {
        if (_pressurizeMoveDelegate == null)
        {
            return;
        }

        await _pressurizeMoveDelegate(instance, targets);
    }

    private static async Task StompMove(WaterfallGiant instance, IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(instance.StompDamage).FromMonster(instance).WithAttackerAnim("AttackDebuff", 0.3f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/waterfall_giant/waterfall_giant_attack_stomp").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<WeakPower>(targets, 1, instance.Creature, null);
    }

    private static async Task RamMove(WaterfallGiant instance)
    {
        await DamageCmd.Attack(instance.RamDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/waterfall_giant/waterfall_giant_attack_kick").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private static async Task SiphonMove(WaterfallGiant instance, IReadOnlyList<Creature> targets)
    {
        if (_siphonMoveDelegate == null)
        {
            return;
        }

        await _siphonMoveDelegate(instance, targets);
        await PowerCmd.Apply<SteamEruptionPower>(instance.Creature, 3, instance.Creature, null);
    }

    private static async Task PressureGunMove(WaterfallGiant instance)
    {
        await DamageCmd.Attack(instance.CurrentPressureGunDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/waterfall_giant/waterfall_giant_attack_kick").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        instance.CurrentPressureGunDamage += instance.PressureGunIncrease;
    }

    private static async Task PressureUpMove(WaterfallGiant instance, IReadOnlyList<Creature> targets)
    {
        if (_pressureUpMoveDelegate == null)
        {
            return;
        }

        await _pressureUpMoveDelegate(instance, targets);
    }

    private static async Task AboutToBlowMove(WaterfallGiant instance, IReadOnlyList<Creature> targets)
    {
        if (_aboutToBlowMoveDelegate == null)
        {
            return;
        }

        await _aboutToBlowMoveDelegate(instance, targets);
    }

    private static async Task ExplodeMove(WaterfallGiant instance, IReadOnlyList<Creature> targets)
    {
        if (_explodeMoveDelegate == null)
        {
            return;
        }

        await _explodeMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(WaterfallGiant), nameof(WaterfallGiant.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(WaterfallGiant __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("PRESSURIZE_MOVE", t => PressurizeMove(__instance, t), new BuffIntent());
        MoveState moveState2 = new MoveState("STOMP_MOVE", t => StompMove(__instance, t), new SingleAttackIntent(__instance.StompDamage), new DebuffIntent());
        MoveState moveState3 = new MoveState("RAM_MOVE", _ => RamMove(__instance), new SingleAttackIntent(__instance.RamDamage));
        MoveState moveState4 = new MoveState("SIPHON_MOVE", t => SiphonMove(__instance, t), new HealIntent(), new BuffIntent());
        MoveState moveState5 = new MoveState("PRESSURE_GUN_MOVE", _ => PressureGunMove(__instance), new SingleAttackIntent(() => __instance.CurrentPressureGunDamage), new BuffIntent());
        MoveState moveState6 = new MoveState("PRESSURE_UP_MOVE", t => PressureUpMove(__instance, t), new SingleAttackIntent(__instance.PressureUpDamage), new BuffIntent());
        __instance.AboutToBlowState = new MoveState("ABOUT_TO_BLOW_MOVE", t => AboutToBlowMove(__instance, t), new StunIntent())
        {
            MustPerformOnceBeforeTransitioning = true
        };
        MoveState moveState7 = new MoveState("EXPLODE_MOVE", t => ExplodeMove(__instance, t), new DeathBlowIntent(() => __instance.SteamEruptionDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState6;
        moveState6.FollowUpState = moveState2;
        __instance.AboutToBlowState.FollowUpState = moveState7;
        moveState7.FollowUpState = moveState7;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(moveState7);
        list.Add(__instance.AboutToBlowState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}