namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Boss;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx.Backgrounds;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RocketPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.KaiserCrab;

    private const int StrengthPowerAmount = 2;
    private const int FrailPowerAmount = 2;

    private static async Task TargetingReticleMove(Rocket instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/kaiser_crab/kaiser_crab_right_attack_snap");
        await instance.Background.PlayAttackAnim(NKaiserCrabBossBackground.ArmSide.Right, "attack", 0.35f);
        await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), targets, FrailPowerAmount, instance.Creature, null);
    }

    private static async Task ChargeUpMove(Rocket instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/kaiser_crab/kaiser_crab_right_buff");
        await instance.Background.PlayRightSideChargeUpAnim(0.7f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, StrengthPowerAmount, instance.Creature, null);
    }

    private static async Task LaserMove(Rocket instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/kaiser_crab/kaiser_crab_rocket");
        await instance.Background.PlayRightSideHeavy(0.5f);
        await DamageCmd.Attack(instance.LaserDamage).FromMonster(instance).WithAttackerFx(null, SrcHelpers.GetSfx(instance, "AttackSfx")).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), instance.Creature, 10, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
    }

    [HarmonyPatch(typeof(Rocket), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix(Rocket __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("TARGETING_RETICLE_MOVE", t => TargetingReticleMove(__instance, t), new DebuffIntent());
        MoveState moveState2 = new MoveState("PRECISION_BEAM_MOVE", __instance.PrecisionBeamMove, new SingleAttackIntent(__instance.PrecisionBeamDamage));
        MoveState moveState3 = new MoveState("CHARGE_UP_MOVE", _ => ChargeUpMove(__instance), new BuffIntent());
        MoveState moveState4 = new MoveState("LASER_MOVE", _ => LaserMove(__instance), new SingleAttackIntent(__instance.LaserDamage));
        MoveState moveState5 = new MoveState("RECHARGE_MOVE", __instance.RechargeMove, new SleepIntent());
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

    [HarmonyPatch(typeof(Rocket), "MinInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 10;
    }
}