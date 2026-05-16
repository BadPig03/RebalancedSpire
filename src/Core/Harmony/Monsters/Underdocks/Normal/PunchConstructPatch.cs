namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Normal;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PunchConstructPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.PunchOffConfig;

    private static bool ShouldFight(PunchConstruct instance)
    {
        return instance.CombatState is { Encounter: PunchOffEventEncounter, Enemies.Count: > 1 };
    }

    private static async Task FightWithMe(PunchConstruct instance)
    {
        var body = NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.Body;
        var vfxContainer = NCombatRoom.Instance?.CombatVfxContainer;
        if (instance.StartsWithStrongPunch && body != null)
        {
            body.Scale *= new Vector2(-1f, 1f);
        }
        await CreatureCmd.TriggerAnim(instance.Creature, "Attack", 0f);
        await Cmd.Wait(0.1f);
        var enemies = instance.Creature.CombatState?.Enemies;
        if (enemies == null)
        {
            return;
        }

        foreach (var enemy in enemies.Where(c => c != instance.Creature).ToList())
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemy, new DamageVar(instance.FastPunchDamage, DamageProps.monsterMove), instance.Creature);
            VfxCmd.PlayOnCreatureCenter(enemy, "vfx/vfx_attack_blunt");
            vfxContainer?.AddChildSafely(NHitSparkVfx.Create(enemy, requireInteractable: false));
            await CreatureCmd.TriggerAnim(enemy, "Hit", 0f);
        }
        if (!instance.StartsWithStrongPunch)
        {
            return;
        }

        await Cmd.Wait(0.8f);
        if (body != null)
        {
            body.Scale *= new Vector2(-1f, 1f);
        }
    }

    private static async Task AfterAddedToRoom(PunchConstruct instance)
    {
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), instance.Creature, 1, instance.Creature, null);
        if (instance.StartingHpReduction <= 0)
        {
            return;
        }

        instance.Rng.FastForwardCounter(instance.StartingHpReduction);
        await CreatureCmd.SetCurrentHp(instance.Creature, instance.Creature.CurrentHp * instance.Rng.NextInt(70, 81) / 100m);
    }

    [HarmonyPatch(typeof(PunchConstruct), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(PunchConstruct __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(PunchConstruct), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(PunchConstruct __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("READY_MOVE", __instance.ReadyMove, new DefendIntent());
        MoveState moveState2 = new MoveState("STRONG_PUNCH_MOVE", __instance.StrongPunchMove, new SingleAttackIntent(__instance.StrongPunchDamage));
        MoveState moveState3 = new MoveState("FIGHT_WITH_ME", _ => FightWithMe(__instance), new UnknownIntent());
        MoveState moveState4 = new MoveState("FAST_PUNCH_MOVE", __instance.FastPunchMove, new MultiAttackIntent(__instance.FastPunchDamage, __instance.FastPunchRepeat), new DebuffIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("FIGHT_WITH_EACH_OTHER");
        branchState.AddState(moveState3, () => ShouldFight(__instance));
        branchState.AddState(moveState4, () => !ShouldFight(__instance));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = branchState;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(branchState);
        __result = new MonsterMoveStateMachine(list, __instance.StartsWithStrongPunch ? moveState2 : moveState);
        return false;
    }
}