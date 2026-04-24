namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Boss;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class KinPriestPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheKinConfig;

    private static int StrengthPowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);
    private static int BlockAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);
    private static int FrailPowerAmount => 1;
    private static int WeakPowerAmount => 1;
    private static int HealAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private static int BreakUpDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private static int BreakUpCount => 3;
    private static int BeamDamage => 0;
    private static int BeamCount => 3;

    private static readonly Func<KinPriest, IReadOnlyList<Creature>, Task>? _ritualMoveDelegate = Helpers.GetDelegate<KinPriest>("RitualMove");

    private static async Task GuardMove(KinPriest instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_priest/the_kin_priest_rally");
        await CreatureCmd.TriggerAnim(instance.Creature, "Rally", 1f);
        await CreatureCmd.Add(ModelDb.Monster<KinFollower>().ToMutable(), instance.CombatState, CombatSide.Enemy, "slot1");
        KinFollower fakeKinFollower = (KinFollower) ModelDb.Monster<KinFollower>().ToMutable();
        fakeKinFollower.StartsWithDance = true;
        await CreatureCmd.Add(fakeKinFollower, instance.CombatState, CombatSide.Enemy, "slot2");
        TalkCmd.Play(MonsterModel.L10NMonsterLookup("KIN_PRIEST.summonFollowersLine"), instance.Creature, VfxColor.Purple, VfxDuration.Standard);
    }

    private static async Task PowerUpMove(KinPriest instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_priest/the_kin_priest_rally");
        await CreatureCmd.TriggerAnim(instance.Creature, "Rally", 1f);
        foreach (Creature enemy in instance.CombatState.Enemies)
        {
            if (enemy.Monster is KinPriest)
            {
                continue;
            }

            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), enemy, StrengthPowerAmount, instance.Creature, null);
        }
    }

    private static async Task ShieldUpMove(KinPriest instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_priest/the_kin_priest_rally");
        await CreatureCmd.TriggerAnim(instance.Creature, "Rally", 1f);
        foreach (Creature enemy in instance.CombatState.Enemies)
        {
            if (enemy.Monster is KinPriest)
            {
                continue;
            }

            await CreatureCmd.GainBlock(enemy, new BlockVar(BlockAmount, ValueProp.Move), null);
        }
    }

    private static async Task BreakUpMove(KinPriest instance)
    {
        await DamageCmd.Attack(BreakUpDamage).WithHitCount(BreakUpCount).FromMonster(instance).WithAttackerAnim("AttackLaser", 0.4f).AfterAttackerAnim(delegate
            {
                NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.GetSpecialNode<NKinPriestBeamVfx>("Visuals/Beam")?.Fire();
                SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_priest/the_kin_priest_soul_beam");
                return Task.CompletedTask;
            })
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3").OnlyPlayAnimOnce().Execute(null);
        foreach (Creature player in instance.CombatState.PlayerCreatures)
        {
            await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), player, FrailPowerAmount, instance.Creature, null);
            await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), player, WeakPowerAmount, instance.Creature, null);
        }
    }

    private static async Task HealUpMove(KinPriest instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_priest/the_kin_priest_rally");
        await CreatureCmd.TriggerAnim(instance.Creature, "Rally", 1f);
        var count = instance.Creature.CombatState?.Players.Count;
        if (count == null)
        {
            return;
        }

        foreach (Creature enemy in instance.CombatState.Enemies)
        {
            if (enemy.Monster is KinPriest)
            {
                continue;
            }

            await CreatureCmd.Heal(enemy, (decimal)(HealAmount * count));
        }
    }

    private static async Task BeamMove(KinPriest instance)
    {
        await DamageCmd.Attack(BeamDamage).WithHitCount(BeamCount).FromMonster(instance).WithAttackerAnim("AttackLaser", 0.4f).AfterAttackerAnim(delegate
            {
                NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.GetSpecialNode<NKinPriestBeamVfx>("Visuals/Beam")?.Fire();
                SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_priest/the_kin_priest_soul_beam");
                return Task.CompletedTask;
            }).WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3").OnlyPlayAnimOnce().Execute(null);
    }

    private static async Task RitualMove(KinPriest instance, IReadOnlyList<Creature> targets)
    {
        if (_ritualMoveDelegate == null)
        {
            return;
        }

        await _ritualMoveDelegate(instance, targets);
    }

    private static async Task AfterDeath(KinPriest instance, Creature creature)
    {
        if (creature == instance.Creature)
        {
            NRunMusicController.Instance?.UpdateMusicParameter("the_kin_progress", 5f);
            foreach (Creature enemy in instance.CombatState.Enemies)
            {
                if (enemy.Monster is not KinFollower { StartsWithDance: false } follower)
                {
                    continue;
                }

                TalkCmd.Play(MonsterModel.L10NMonsterLookup("KIN_FOLLOWER.rageLine"), enemy, VfxColor.Purple, VfxDuration.Standard);
                var state = (MoveState?) follower.MoveStateMachine?.States["REVENGE_DANCE_MOVE"];
                if (state == null)
                {
                    return;
                }

                follower.SetMoveImmediate(state);
            }
        }
        else if (creature.Monster is KinFollower)
        {
            foreach (Creature enemy in instance.CombatState.Enemies)
            {
                if (enemy.Monster is not KinPriest priest)
                {
                    continue;
                }

                NRunMusicController.Instance?.UpdateMusicParameter("the_kin_progress", 1f);
                priest.AllFollowerDeathResponse();
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), enemy, StrengthPowerAmount, enemy, null);
                if (priest.SpeechUsed)
                {
                    var state = (MoveState?) priest.MoveStateMachine?.States["RITUAL_MOVE"];
                    if (state == null)
                    {
                        return;
                    }

                    priest.SetMoveImmediate(state);
                }
                priest.SpeechUsed = true;
            }
        }
    }

    [HarmonyPatch(typeof(KinPriest), nameof(KinPriest.AfterDeath))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterDeath(KinPriest __instance, PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterDeath(__instance, creature);
        return false;
    }

    [HarmonyPatch(typeof(KinPriest), nameof(KinPriest.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(KinPriest __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("GUARD_MOVE", _ => GuardMove(__instance), new SummonIntent());
        MoveState moveState2 = new MoveState("POWER_UP_MOVE", _ => PowerUpMove(__instance), new BuffIntent());
        MoveState moveState3 = new MoveState("SHIELD_UP_MOVE", _ => ShieldUpMove(__instance), new DefendIntent());
        MoveState moveState4 = new MoveState("BREAK_UP_MOVE", _ => BreakUpMove(__instance), new MultiAttackIntent(BreakUpDamage, BreakUpCount), new DebuffIntent());
        MoveState moveState5 = new MoveState("HEAL_UP_MOVE", _ => HealUpMove(__instance), new HealIntent());
        MoveState moveState6 = new MoveState("BEAM_MOVE", _ => BeamMove(__instance), new MultiAttackIntent(BeamDamage, BeamCount));
        MoveState moveState7 = new MoveState("RITUAL_MOVE", t => RitualMove(__instance, t), new BuffIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState2;
        moveState6.FollowUpState = moveState7;
        moveState7.FollowUpState = moveState6;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(moveState7);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}