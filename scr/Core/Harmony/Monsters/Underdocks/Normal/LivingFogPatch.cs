namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Normal;

using Core.Powers;
using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LivingFogPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.LivingFogConfig;

    private static int MaxGasBombs => 5;
    private static int PingPingPowerAmount => 1;

    private static readonly Func<LivingFog, IReadOnlyList<Creature>, Task>? _advancedGasMoveDelegate = Helpers.GetDelegate<LivingFog>("AdvancedGasMove");

    private static async Task AdvancedGasMove(LivingFog instance, IReadOnlyList<Creature> targets)
    {
        if (_advancedGasMoveDelegate == null)
        {
            return;
        }

        await _advancedGasMoveDelegate(instance, targets);
    }

    private static async Task BloatMove(LivingFog instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/living_fog/living_fog_summon");
        await CreatureCmd.TriggerAnim(instance.Creature, "SpawnBomb", 0.35f);
        instance.BloatAmount = Math.Min(instance.BloatAmount + 1, MaxGasBombs);
        for (var i = 0; i < instance.BloatAmount; i++)
        {
            var nextSlot = instance.CombatState.Encounter?.GetNextSlot(instance.CombatState);
            if (nextSlot == "")
            {
                continue;
            }

            SfxCmd.Play("event:/sfx/enemy/enemy_attacks/living_fog/living_fog_minion_appear");
            Creature gasBomb = await CreatureCmd.Add<GasBomb>(instance.CombatState, nextSlot);
            await PowerCmd.Apply<PingPongPower>(gasBomb, PingPingPowerAmount, instance.Creature, null);
        }
        await DamageCmd.Attack(instance.BloatDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.1f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_attack_blow").WithHitVfxNode(_ => NGaseousImpactVfx.Create(CombatSide.Player, instance.CombatState, new Color("#402f45"))).Execute(null);
    }

    private static async Task ChargeUpMove(LivingFog instance)
    {
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 1.25f);
        await CreatureCmd.GainBlock(instance.Creature, new BlockVar(instance.SuperGasBlastDamage, BlockProps.monsterMove), null);
    }

    [HarmonyPatch(typeof(LivingFog), nameof(LivingFog.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result += 14;
    }

    [HarmonyPatch(typeof(LivingFog), nameof(LivingFog.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(LivingFog __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("ADVANCED_GAS_MOVE", t => AdvancedGasMove(__instance, t), new SingleAttackIntent(__instance.AdvancedGasDamage), new CardDebuffIntent());
        MoveState moveState2 = new MoveState("BLOAT_MOVE", _ => BloatMove(__instance), new SingleAttackIntent(__instance.BloatDamage), new SummonIntent());
        MoveState moveState3 = new MoveState("CHARGE_UP_MOVE", _ => ChargeUpMove(__instance), new DefendIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}