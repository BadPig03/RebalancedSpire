namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Boss;

using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CeremonialBeastPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.CeremonialBeastConfig;

    private static float FirstPlowAmount => 0.8f;
    private static float SecondPlowAmount => 0.4f;

    private static readonly Func<CeremonialBeast, IReadOnlyList<Creature>, Task>? _plowMoveDelegate = Helpers.GetDelegate<CeremonialBeast>("PlowMove");
    private static readonly Func<CeremonialBeast, IReadOnlyList<Creature>, Task>? _stunnedMoveDelegate = Helpers.GetDelegate<CeremonialBeast>("StunnedMove");
    private static readonly Func<CeremonialBeast, IReadOnlyList<Creature>, Task>? _beastCryMoveDelegate = Helpers.GetDelegate<CeremonialBeast>("BeastCryMove");
    private static readonly Func<CeremonialBeast, IReadOnlyList<Creature>, Task>? _stompMoveDelegate = Helpers.GetDelegate<CeremonialBeast>("StompMove");
    private static readonly Func<CeremonialBeast, IReadOnlyList<Creature>, Task>? _crushMoveDelegate = Helpers.GetDelegate<CeremonialBeast>("CrushMove");

    private static async Task FirstStampMove(CeremonialBeast instance)
    {
        SfxCmd.Play(instance.AttackSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Attack", 0.6f);
        await Cmd.CustomScaledWait(0f, 0.4f);
        await PowerCmd.Apply<PlowPlusPower>(instance.Creature, (int) (instance.MaxInitialHp * FirstPlowAmount), instance.Creature, null);
    }

    private static async Task PlowMove(CeremonialBeast instance, IReadOnlyList<Creature> targets)
    {
        if (_plowMoveDelegate == null)
        {
            return;
        }

        await _plowMoveDelegate(instance, targets);
    }

    private static async Task SecondStampMove(CeremonialBeast instance)
    {
        SfxCmd.Play(instance.AttackSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Attack", 0.6f);
        await Cmd.CustomScaledWait(0f, 0.4f);
        await PowerCmd.Apply<PlowPlusPower>(instance.Creature, (int) (instance.MaxInitialHp * SecondPlowAmount), instance.Creature, null);
    }

    private static async Task StunnedMove(CeremonialBeast instance, IReadOnlyList<Creature> targets)
    {
        if (_stunnedMoveDelegate == null)
        {
            return;
        }

        await _stunnedMoveDelegate(instance, targets);
    }

    private static async Task StompMove(CeremonialBeast instance, IReadOnlyList<Creature> targets)
    {
        if (_stompMoveDelegate == null)
        {
            return;
        }

        await _stompMoveDelegate(instance, targets);
    }

    private static async Task CrushMove(CeremonialBeast instance, IReadOnlyList<Creature> targets)
    {
        if (_crushMoveDelegate == null)
        {
            return;
        }

        await _crushMoveDelegate(instance, targets);
    }

    private static async Task BeastCryMove(CeremonialBeast instance, IReadOnlyList<Creature> targets)
    {
        if (_beastCryMoveDelegate == null)
        {
            return;
        }

        await _beastCryMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(CeremonialBeast), nameof(CeremonialBeast.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result += 38;
    }

    [HarmonyPatch(typeof(CeremonialBeast), nameof(CeremonialBeast.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(CeremonialBeast __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("FIRST_STAMP_MOVE", _ => FirstStampMove(__instance), new BuffIntent());
        MoveState moveState2 = new MoveState("FIRST_PLOW_MOVE", t => PlowMove(__instance, t), new SingleAttackIntent(__instance.PlowDamage), new BuffIntent());
        MoveState moveState3 = new MoveState("STUN_MOVE", t => StunnedMove(__instance, t), new StunIntent())
        {
            MustPerformOnceBeforeTransitioning = true
        };
        MoveState moveState4 = new MoveState("SECOND_STAMP_MOVE", _ => SecondStampMove(__instance), new BuffIntent());
        MoveState moveState5 = new MoveState("SECOND_PLOW_MOVE", t => PlowMove(__instance, t), new SingleAttackIntent(__instance.PlowDamage), new BuffIntent());
        __instance.BeastCryState = new MoveState("BEAST_CRY_MOVE", t => BeastCryMove(__instance, t), new DebuffIntent());
        MoveState moveState6 = new MoveState("STOMP_MOVE", t => StompMove(__instance, t), new SingleAttackIntent(__instance.StompDamage));
        MoveState moveState7 = new MoveState("CRUSH_MOVE", t => CrushMove(__instance, t), new SingleAttackIntent(__instance.CrushDamage), new BuffIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState2;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState5;
        __instance.BeastCryState.FollowUpState = moveState6;
        moveState6.FollowUpState = moveState7;
        moveState7.FollowUpState = __instance.BeastCryState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(moveState7);
        list.Add(__instance.BeastCryState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}