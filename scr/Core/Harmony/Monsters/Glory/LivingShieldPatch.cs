namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class LivingShieldPatch
{
    private static int RampartPowerAmount => 15;
    private static int SmashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 16, 14);
    private static int BlockAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 25, 20);
    private static int StrengthPowerAmount => 2;

    private static async Task ShieldUpMove(LivingShield instance)
    {
        await CreatureCmd.GainBlock(instance.Creature, new BlockVar(BlockAmount, ValueProp.Move), null);
    }

    private static async Task SmashMove(LivingShield instance)
    {
        await DamageCmd.Attack(SmashDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
        await PowerCmd.Apply<StrengthPower>(instance.Creature, StrengthPowerAmount, instance.Creature, null);
    }

    private static async Task AfterAddedToRoom(LivingShield instance)
    {
        await PowerCmd.Apply<RampartPower>(instance.Creature, RampartPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(LivingShield), nameof(LivingShield.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(LivingShield __instance, ref Task __result)
    {
        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(LivingShield), nameof(LivingShield.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(LivingShield __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SHIELD_UP_MOVE", _ => ShieldUpMove(__instance), new DefendIntent());
        MoveState moveState2 = new MoveState("SMASH_MOVE", _ => SmashMove(__instance), new SingleAttackIntent(SmashDamage), new BuffIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("SHIELD_SLAM_BRANCH");
        moveState.FollowUpState = branchState;
        branchState.AddState(moveState, () => __instance.GetAllyCount() > 0);
        branchState.AddState(moveState2, () => __instance.GetAllyCount() == 0);
        moveState2.FollowUpState = moveState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(branchState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}