namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Elite;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class InfestedPrismPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.InfestedPrism;

    private const int TaintedPlusPowerAmount = 1;
    private const int WhirlwindRepeat = 3;
    private const int PulsatePowerAmount = 4;

    private static int JabDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 22, 20);
    private static int RadiateDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 16, 14);
    private static int WhirlwindDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);

    private static async Task AfterAddedToRoom(InfestedPrism instance)
    {
        var players = instance.CombatState.Players.ToList();
        foreach (var player in players)
        {
            await PowerCmd.Apply<TaintedPlusPower>(new ThrowingPlayerChoiceContext(), player.Creature, TaintedPlusPowerAmount, instance.Creature, null);
        }
    }

    private static async Task JabMove(InfestedPrism instance)
    {
        await DamageCmd.Attack(JabDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.1f).WithAttackerFx(null, SrcHelpers.GetSfx(instance, "AttackSfx")).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task RadiateMove(InfestedPrism instance)
    {
        await DamageCmd.Attack(RadiateDamage).FromMonster(instance).WithAttackerAnim("AttackBlock", 0.25f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack_defend").WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task WhirlwindMove(InfestedPrism instance)
    {
        await DamageCmd.Attack(WhirlwindDamage).WithHitCount(WhirlwindRepeat).FromMonster(instance).WithAttackerAnim("AttackDouble", 0.2f).OnlyPlayAnimOnce().WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack_spin").WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task PulsateMove(InfestedPrism instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_buff");
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.6f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, PulsatePowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(InfestedPrism), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(InfestedPrism __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(InfestedPrism), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(InfestedPrism __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("JAB_MOVE", _ => JabMove(__instance), new SingleAttackIntent(JabDamage));
        MoveState moveState2 = new MoveState("RADIATE_MOVE", _ => RadiateMove(__instance), new SingleAttackIntent(RadiateDamage));
        MoveState moveState3 = new MoveState("WHIRLWIND_MOVE", _ => WhirlwindMove(__instance), new MultiAttackIntent(WhirlwindDamage, WhirlwindRepeat));
        MoveState moveState4 = new MoveState("PULSATE_MOVE", _ => PulsateMove(__instance), new BuffIntent());
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