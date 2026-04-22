namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public class MechaKnightPatch
{
    private static int BurnAmount => 2;

    private static readonly Func<MechaKnight, IReadOnlyList<Creature>, Task>? _chargeMoveDelegate = Helpers.GetDelegate<MechaKnight>("ChargeMove");
    private static readonly Func<MechaKnight, IReadOnlyList<Creature>, Task>? _windupMoveDelegate = Helpers.GetDelegate<MechaKnight>("WindupMove");
    private static readonly Func<MechaKnight, IReadOnlyList<Creature>, Task>? _heavyCleaveMoveDelegate = Helpers.GetDelegate<MechaKnight>("HeavyCleaveMove");

    private static async Task ChargeMove(MechaKnight instance, IReadOnlyList<Creature> targets)
    {
        if (_chargeMoveDelegate == null)
        {
            return;
        }

        await _chargeMoveDelegate(instance, targets);
    }

    private static async Task FlamethrowerMove(MechaKnight instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_flamethrower");
        await CreatureCmd.TriggerAnim(instance.Creature, "flamethrower", 1.5f);
        await CardPileCmd.AddToCombatAndPreview<Burn>(targets, PileType.Hand, BurnAmount, addedByPlayer: false);
    }

    private static async Task WindupMove(MechaKnight instance, IReadOnlyList<Creature> targets)
    {
        if (_windupMoveDelegate == null)
        {
            return;
        }

        await _windupMoveDelegate(instance, targets);
    }

    private static async Task HeavyCleaveMove(MechaKnight instance, IReadOnlyList<Creature> targets)
    {
        if (_heavyCleaveMoveDelegate == null)
        {
            return;
        }

        await _heavyCleaveMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(MechaKnight), nameof(MechaKnight.ChargeDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseChargeDamage(ref int __result)
    {
        __result += 5;
    }

    [HarmonyPatch(typeof(MechaKnight), nameof(MechaKnight.HeavyCleaveDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceHeavyCleaveDamage(ref int __result)
    {
        __result -= 15;
    }

    [HarmonyPatch(typeof(MechaKnight), nameof(MechaKnight.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(MechaKnight __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("CHARGE_MOVE", t => ChargeMove(__instance, t), new SingleAttackIntent(MechaKnight.ChargeDamage));
        MoveState moveState2 = new MoveState("FLAMETHROWER_MOVE", t => FlamethrowerMove(__instance, t), new StatusIntent(BurnAmount));
        MoveState moveState3 = new MoveState("WINDUP_MOVE", t => WindupMove(__instance, t), new DefendIntent(), new BuffIntent());
        MoveState moveState4 = new MoveState("HEAVY_CLEAVE_MOVE", t => HeavyCleaveMove(__instance, t), new SingleAttackIntent(MechaKnight.HeavyCleaveDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}