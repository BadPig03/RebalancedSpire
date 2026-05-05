namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Boss;

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
public static class WaterfallGiantPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.WaterfallGiantConfig;

    private static int WeakPowerAmount => 1;
    private static int SteamEruptionPowerAmount => 9;

    private static async Task StompMove(WaterfallGiant instance, IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(instance.StompDamage).FromMonster(instance).WithAttackerAnim("AttackDebuff", 0.3f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/waterfall_giant/waterfall_giant_attack_stomp").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<WeakPower>(targets, WeakPowerAmount, instance.Creature, null);
    }

    private static async Task RamMove(WaterfallGiant instance)
    {
        await DamageCmd.Attack(instance.RamDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/waterfall_giant/waterfall_giant_attack_kick").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private static async Task PressureGunMove(WaterfallGiant instance)
    {
        await DamageCmd.Attack(instance.CurrentPressureGunDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/waterfall_giant/waterfall_giant_attack_kick").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        instance.CurrentPressureGunDamage += instance.PressureGunIncrease;
    }

    private static async Task PressureUpMove(WaterfallGiant instance)
    {
        await DamageCmd.Attack(instance.PressureUpDamage).FromMonster(instance).WithAttackerAnim("AttackBuff", 0.15f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/waterfall_giant/waterfall_giant_attack_stomp").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<SteamEruptionPower>(instance.Creature, SteamEruptionPowerAmount, instance.Creature, null);
        instance.IncrementBuildUpAnimationTrack();
    }

    [HarmonyPatch(typeof(WaterfallGiant), nameof(WaterfallGiant.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(WaterfallGiant __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("PRESSURIZE_MOVE", __instance.PressurizeMove, new BuffIntent());
        MoveState moveState2 = new MoveState("STOMP_MOVE", t => StompMove(__instance, t), new SingleAttackIntent(__instance.StompDamage), new DebuffIntent());
        MoveState moveState3 = new MoveState("RAM_MOVE", _ => RamMove(__instance), new SingleAttackIntent(__instance.RamDamage));
        MoveState moveState4 = new MoveState("SIPHON_MOVE", __instance.SiphonMove, new HealIntent(), new BuffIntent());
        MoveState moveState5 = new MoveState("PRESSURE_GUN_MOVE", _ => PressureGunMove(__instance), new SingleAttackIntent(() => __instance.CurrentPressureGunDamage), new BuffIntent());
        MoveState moveState6 = new MoveState("PRESSURE_UP_MOVE", _ => PressureUpMove(__instance), new SingleAttackIntent(__instance.PressureUpDamage), new BuffIntent());
        __instance.AboutToBlowState = new MoveState("ABOUT_TO_BLOW_MOVE", __instance.AboutToBlowMove, new StunIntent())
        {
            MustPerformOnceBeforeTransitioning = true
        };
        MoveState moveState7 = new MoveState("EXPLODE_MOVE", __instance.ExplodeMove, new DeathBlowIntent(() => __instance.SteamEruptionDamage));
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