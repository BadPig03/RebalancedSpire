namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class SkulkingColonyPatch
{
    private static int SmashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 11, 9);
    private static int ZoomDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 17, 16);
    private static int ZoomBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 13, 10);
    private static int PiercingStabsDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private static int PiercingStabsCount => 2;

    private static async Task SmashMove(SkulkingColony instance)
    {
        await DamageCmd.Attack(SmashDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.15f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private static async Task ZoomMove(SkulkingColony instance)
    {
        await DamageCmd.Attack(ZoomDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.15f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
        await CreatureCmd.GainBlock(instance.Creature, ZoomBlock, ValueProp.Move, null);
    }

    private static async Task InertiaMove(SkulkingColony instance)
    {
        await PowerCmd.Apply<StrengthPower>(instance.Creature, instance.InertiaStrengthGain, instance.Creature, null);
    }

    private static async Task PiercingStabsMove(SkulkingColony instance)
    {
        await DamageCmd.Attack(PiercingStabsDamage).WithHitCount(PiercingStabsCount).FromMonster(instance).WithAttackerAnim("Attack", 0.15f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task AfterAddedToRoom(SkulkingColony instance)
    {
        await PowerCmd.Apply<HardenedShellPower>(instance.Creature, 20, instance.Creature, null);
    }

    [HarmonyPatch(typeof(SkulkingColony), nameof(SkulkingColony.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(SkulkingColony __instance, ref Task __result)
    {
        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(SkulkingColony), nameof(SkulkingColony.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(SkulkingColony __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SMASH_MOVE", _ => SmashMove(__instance), new SingleAttackIntent(SmashDamage));
        MoveState moveState2 = new MoveState("ZOOM_MOVE", _ => ZoomMove(__instance), new SingleAttackIntent(ZoomDamage), new DefendIntent());
        MoveState moveState3 = new MoveState("INERTIA_MOVE", _ => InertiaMove(__instance), new BuffIntent());
        MoveState moveState4 = new MoveState("PIERCING_STABS_MOVE", _ => PiercingStabsMove(__instance), new MultiAttackIntent(PiercingStabsDamage, PiercingStabsCount));
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