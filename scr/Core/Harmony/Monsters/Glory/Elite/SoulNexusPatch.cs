namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Elite;

using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SoulNexusPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SoulNexusConfig;

    private static int SoulStrikeDamage => 3;
    private static int SoulStrikeRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private static int MaelstromDamage => 2;
    private static int MaelstromRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 12);
    private static int DrainLifeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 19);
    private static int VulnerablePowerAmount => 1;
    private static int SoulWitherAmount => 1;

    private static async Task SoulStrikeMove(SoulNexus instance)
    {
        await DamageCmd.Attack(SoulStrikeDamage).WithHitCount(SoulStrikeRepeat).FromMonster(instance).OnlyPlayAnimOnce().WithAttackerAnim("Attack", 0.8f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task MaelstromMove(SoulNexus instance)
    {
        await DamageCmd.Attack(MaelstromDamage).WithHitCount(MaelstromRepeat).FromMonster(instance).OnlyPlayAnimOnce().WithAttackerAnim("Attack", 0.8f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task DrainLifeMove(SoulNexus instance)
    {
        await DamageCmd.Attack(DrainLifeDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.6f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task SoulMarkMove(SoulNexus instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.CastSfx, 0.8f);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.8f);
        foreach (SoulWitherPower power in instance.Creature.GetPowerInstances<SoulWitherPower>())
        {
            power.Reset();
        }
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, VulnerablePowerAmount, instance.Creature, null);
    }

    private static async Task SoulBurnMove(SoulNexus instance)
    {
        await DamageCmd.Attack(instance.SoulBurnDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.6f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task AfterAddedToRoom(SoulNexus instance)
    {
        foreach (Creature creature in instance.CombatState.PlayerCreatures)
        {
            SoulWitherPower soulWitherPower = (SoulWitherPower) ModelDb.Power<SoulWitherPower>().ToMutable();
            soulWitherPower.Target = creature;
            await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), soulWitherPower, instance.Creature, SoulWitherAmount, instance.Creature, null);
        }
    }

    private static bool IsAnyPlayerExceedLimit(SoulNexus instance)
    {
        return instance.Creature.GetPowerInstances<SoulWitherPower>().Any(power => power.ExceedLimit());
    }

    [HarmonyPatch(typeof(SoulNexus), nameof(SoulNexus.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(SoulNexus __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(SoulNexus), nameof(SoulNexus.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(SoulNexus __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SOUL_STRIKE_MOVE", _ => SoulStrikeMove(__instance), new MultiAttackIntent(SoulStrikeDamage, SoulStrikeRepeat));
        MoveState moveState2 = new MoveState("MAELSTROM_MOVE", _ => MaelstromMove(__instance), new MultiAttackIntent(MaelstromDamage, MaelstromRepeat));
        MoveState moveState3 = new MoveState("DRAIN_LIFE_MOVE", _ => DrainLifeMove(__instance), new SingleAttackIntent(DrainLifeDamage));
        MoveState moveState4 = new MoveState("SOUL_MARK_MOVE", t => SoulMarkMove(__instance, t), new DebuffIntent());
        MoveState moveState5 = new MoveState("SOUL_BURN_MOVE", _ => SoulBurnMove(__instance), new SingleAttackIntent(__instance.SoulBurnDamage));
        ConditionalBranchState branchState = new ConditionalBranchState("soulNexusBranch");
        branchState.AddState(moveState4, () => IsAnyPlayerExceedLimit(__instance));
        branchState.AddState(moveState2, () => !IsAnyPlayerExceedLimit(__instance));
        ConditionalBranchState branchState2 = new ConditionalBranchState("soulNexus2Branch");
        branchState2.AddState(moveState4, () => IsAnyPlayerExceedLimit(__instance));
        branchState2.AddState(moveState3, () => !IsAnyPlayerExceedLimit(__instance));
        ConditionalBranchState branchState3 = new ConditionalBranchState("soulNexus3Branch");
        branchState3.AddState(moveState4, () => IsAnyPlayerExceedLimit(__instance));
        branchState3.AddState(moveState, () => !IsAnyPlayerExceedLimit(__instance));
        moveState.FollowUpState = branchState;
        moveState2.FollowUpState = branchState2;
        moveState3.FollowUpState = branchState3;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(branchState);
        list.Add(branchState2);
        list.Add(branchState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}