namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Boss;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TestSubjectPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TestSubjectConfig;

    private static int AdaptablePowerAmount => 1;
    private static int EnragePowerAmount => 3;
    private static int SkullBashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 14, 12);
    private static int VulnerablePowerAmount => 1;
    private static int BiteDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 18);
    private static int MultiClawDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 11, 10);
    private static int PounceDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 28);
    private static int LacerateDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 11, 10);
    private static int LacerateCount => 3;
    private static int BigPounceDamage => 43;
    private static int BurnCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private static int StrengthPowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2,1);
    private static int PainfulStabsPowerAmount => 1;
    private static int NemesisPowerAmount => 1;

    private static async Task GrowlMove(TestSubject instance)
    {
        await CreatureCmd.TriggerAnim(instance.Creature, "BurnTrigger", 1.25f);
        await PowerCmd.Apply<EnragePower>(new ThrowingPlayerChoiceContext(), instance.Creature, EnragePowerAmount, instance.Creature, null);
    }

    private static async Task RespawnMove(TestSubject instance)
    {
        instance.Respawns++;
        NRunMusicController.Instance?.UpdateMusicParameter("test_subject_progress", 2f);
        SfxCmd.Play(instance.Respawns == 1 ? "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_revive_two_heads" : "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_revive_three_heads");
        await CreatureCmd.TriggerAnim(instance.Creature, "RespawnTrigger", 0f);
        await Cmd.Wait(0.8f);
        NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.SetDefaultScaleTo(1f + instance.Respawns * 0.1f, 0.1f);
        await Cmd.Wait(1.15f);
        if (instance.Creature.CombatState == null)
        {
            return;
        }

        instance.Creature.GetPower<AdaptablePower>()?.DoRevive();
        switch (instance.Respawns)
        {
            case 1:
                await instance.Revive(instance.SecondFormHp);
                await PowerCmd.Apply<PainfulStabsPower>(new ThrowingPlayerChoiceContext(), instance.Creature, PainfulStabsPowerAmount, instance.Creature, null);
                break;
            case 2:
                await instance.Revive(instance.ThirdFormHp);
                NemesisPower? nemesisPower = await PowerCmd.Apply<NemesisPower>(new ThrowingPlayerChoiceContext(), instance.Creature, NemesisPowerAmount, instance.Creature, null);
                if (nemesisPower != null)
                {
                    nemesisPower._shouldApplyIntangible = true;
                }
                await PowerCmd.Remove<AdaptablePower>(instance.Creature);
                await PowerCmd.Remove<PainfulStabsPower>(instance.Creature);
                break;
        }
    }

    private static async Task BiteMove(TestSubject instance, IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BiteDamage).FromMonster(instance).WithAttackerAnim("BiteTrigger", 0.25f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_bite").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private static async Task SkullBashMove(TestSubject instance, IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(SkullBashDamage).FromMonster(instance).WithAttackerAnim("BiteTrigger", 0.25f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_bite").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, VulnerablePowerAmount, instance.Creature, null);
    }

    private static async Task MultiClawMove(TestSubject instance)
    {
        await DamageCmd.Attack(MultiClawDamage).WithHitCount(instance.MultiClawTotalCount).FromMonster(instance).OnlyPlayAnimOnce().WithAttackerAnim("MultiAttackTrigger", 0.2f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_slash").Execute(null);
        instance.ExtraMultiClawCount++;
    }

    private static async Task PounceMove(TestSubject instance)
    {
        await DamageCmd.Attack(PounceDamage).FromMonster(instance).WithAttackerAnim("BiteTrigger", 0.25f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_bite").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private static async Task Phase3LacerateMove(TestSubject instance)
    {
        await DamageCmd.Attack(LacerateDamage).WithHitCount(LacerateCount).FromMonster(instance).OnlyPlayAnimOnce().WithAttackerAnim("MultiAttackTrigger", 0.2f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_slash").Execute(null);
    }

    private static async Task BigPounceMove(TestSubject instance)
    {
        await DamageCmd.Attack(BigPounceDamage).FromMonster(instance).WithAttackerAnim("BiteTrigger", 0.25f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_bite").WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private static async Task BurningGrowlMove(TestSubject instance, IReadOnlyList<Creature> targets)
    {
        instance.SetColor(Colors.White);
        NCombatRoom.Instance?.BackCombatVfxContainer.AddChildSafely(NTestSubjectBurnVfx.Create());
        await CreatureCmd.TriggerAnim(instance.Creature, "BurnTrigger", 1.25f);
        await CardPileCmd.AddToCombatAndPreview<Burn>(targets, PileType.Discard, BurnCount, null);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, StrengthPowerAmount, instance.Creature, null);
    }

    private static async Task AfterAddedToRoom(TestSubject instance)
    {
        await PowerCmd.Apply<AdaptablePower>(new ThrowingPlayerChoiceContext(), instance.Creature, AdaptablePowerAmount, instance.Creature, null);
        instance.Creature.PowerApplied += instance.AfterPowerApplied;
        instance.Creature.PowerRemoved += instance.AfterPowerRemoved;
    }

    [HarmonyPatch(typeof(TestSubject), nameof(TestSubject.SecondFormHp), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_SecondFormHp(TestSubject __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 222, 210);
        return false;
    }

    [HarmonyPatch(typeof(TestSubject), nameof(TestSubject.ThirdFormHp), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_ThirdFormHp(TestSubject __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 333, 320);
        return false;
    }

    [HarmonyPatch(typeof(TestSubject), nameof(TestSubject.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(TestSubject __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(TestSubject), nameof(TestSubject.ShouldShowMoveInBestiary))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_ShouldShowMoveInBestiary(TestSubject __instance, string moveStateId, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(TestSubject), nameof(TestSubject.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(TestSubject __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        __instance.DeadState = new MoveState("RESPAWN_MOVE", _ => RespawnMove(__instance), new HealIntent(), new BuffIntent())
        {
            MustPerformOnceBeforeTransitioning = true
        };
        MoveState moveState = new MoveState("GROWL_MOVE", _ => GrowlMove(__instance), new BuffIntent());
        MoveState moveState2 = new MoveState("SKULL_BASH_MOVE", t => SkullBashMove(__instance, t), new SingleAttackIntent(SkullBashDamage), new DebuffIntent());
        MoveState moveState3 = new MoveState("BITE_MOVE", t => BiteMove(__instance, t), new SingleAttackIntent(BiteDamage));
        MoveState moveState4 = new MoveState("MULTI_CLAW_MOVE", _ => MultiClawMove(__instance), new MultiAttackIntent(MultiClawDamage, () => __instance.MultiClawTotalCount));
        MoveState moveState5 = new MoveState("POUNCE_MOVE", _ => PounceMove(__instance), new SingleAttackIntent(PounceDamage));
        MoveState moveState6 = new MoveState("PHASE3_LACERATE_MOVE", _ => Phase3LacerateMove(__instance), new MultiAttackIntent(LacerateDamage, LacerateCount));
        MoveState moveState7 = new MoveState("BIG_POUNCE_MOVE", _ => BigPounceMove(__instance), new SingleAttackIntent(BigPounceDamage));
        MoveState moveState8 = new MoveState("BURNING_GROWL_MOVE", t => BurningGrowlMove(__instance, t), new StatusIntent(BurnCount), new BuffIntent());
        ConditionalBranchState conditionalBranchState = new ConditionalBranchState("REVIVE_BRANCH");
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState2;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState4;
        moveState6.FollowUpState = moveState7;
        moveState7.FollowUpState = moveState8;
        moveState8.FollowUpState = moveState6;
        __instance.DeadState.FollowUpState = conditionalBranchState;
        conditionalBranchState.AddState(moveState4, () => __instance.Respawns < 2);
        conditionalBranchState.AddState(moveState6, () => __instance.Respawns >= 2);
        list.Add(__instance.DeadState);
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(moveState7);
        list.Add(moveState8);
        list.Add(conditionalBranchState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}