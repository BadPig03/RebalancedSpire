namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Normal;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class FabricatorPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Fabricator;

    internal const float SpawnBotDamageRatio = 1 / 15f;
    private const int FabricatorPowerAmount = 1;
    private const int MinionPowerAmount = 1;

    private static int DisintegrateDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 18);

    private static async Task SpawnBot(Fabricator instance, IEnumerable<MonsterModel> options)
    {
        var bots = options.Where(m => m != instance._lastSpawned).ToList();
        var spawnBot = instance._lastSpawned = instance.RunRng.MonsterAi.NextItem(bots);
        if (spawnBot == null)
        {
            return;
        }

        var bot = await CreatureCmd.Add(spawnBot.ToMutable(), instance.CombatState, CombatSide.Enemy, instance.CombatState.Encounter?.GetNextSlot(instance.CombatState));
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), bot, MinionPowerAmount, instance.Creature, null);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), instance.Creature, new DamageVar(FabricatorPower.GetSpawnBotDamage(instance.Creature), DamageProps.nonCardHpLoss | ValueProp.SkipHurtAnim), instance.Creature, null);
    }

    private static async Task FabricateMove(Fabricator instance)
    {
        await SpawnBot(instance, Fabricator.defenseSpawns);
        await SpawnBot(instance, Fabricator.aggroSpawns);
    }

    private static async Task DisintegrateMove(Fabricator instance)
    {
        await DamageCmd.Attack(DisintegrateDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.6f).WithAttackerFx(null, SrcHelpers.GetSfx(instance, "AttackSfx")).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task EscapeMove(Fabricator instance)
    {
        await Cmd.CustomScaledWait(0.75f, 1.25f);
        await CreatureCmd.TriggerAnim(instance.Creature, "Dead", 0f);
        await Cmd.Wait(1.25f);
        await CreatureCmd.Escape(instance.Creature);
    }

    private static async Task AfterAddedToRoom(Fabricator instance)
    {
        await PowerCmd.Apply<FabricatorPower>(new ThrowingPlayerChoiceContext(), instance.Creature, FabricatorPowerAmount, instance.Creature, null);
    }

    private static bool CanFabricate(Fabricator instance)
    {
        return instance.Creature.CombatState?.Enemies.Count(c => c.IsAlive && c.HasPower<MinionPower>()) <= 2 && FabricatorPower.IsHpRemainingEnough(instance.Creature);
    }

    [HarmonyPatch(typeof(MonsterModel), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(MonsterModel __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Fabricator fabricator)
        {
            return true;
        }

        __result = AfterAddedToRoom(fabricator);
        return false;
    }

    [HarmonyPatch(typeof(Fabricator), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Fabricator __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("FABRICATE_MOVE", _ => FabricateMove(__instance), new SummonIntent());
        MoveState moveState2 = new MoveState("DISINTEGRATE_MOVE", _ => DisintegrateMove(__instance), new SingleAttackIntent(DisintegrateDamage));
        MoveState moveState3 = new MoveState("ESCAPE_MOVE", _ => EscapeMove(__instance), new EscapeIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("fabricateBranch");
        branchState.AddState(moveState, () => CanFabricate(__instance));
        branchState.AddState(moveState2, () => !CanFabricate(__instance));
        moveState.FollowUpState = branchState;
        moveState2.FollowUpState = branchState;
        moveState3.FollowUpState = moveState3;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(branchState);
        __result = new MonsterMoveStateMachine(list, branchState);
        return false;
    }

    [HarmonyPatch(typeof(Fabricator), "MinInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 240, 220);
    }
}