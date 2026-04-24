namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Boss;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class VantomPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.VantomConfig;

    private static int PainfulStabsPowerAmount => 1;
    private static int InitSlipperyPowerAmount => 3;
    private static int SlipperyPowerAmount => 2;
    private static int StrengthPowerAmount => 2;
    private static int WeakPowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);

    private static readonly Func<Vantom, IReadOnlyList<Creature>, Task>? _inkyLanceMoveDelegate = Helpers.GetDelegate<Vantom>("InkyLanceMove");

    private static async Task InkBlotMove(Vantom instance, IReadOnlyList<Creature> targets)
    {
        if (!TestMode.IsOff || !instance.Creature.IsAlive)
        {
            return;
        }

        await Cmd.CustomScaledWait(1f, 1f);
        NRunMusicController.Instance?.UpdateMusicParameter("vantom_progress", 1f);
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/vantom/vantom_extend_2");
        await CreatureCmd.TriggerAnim(instance.Creature, "CHARGE_UP", 0.15f);
        MegaAnimationState? megaAnimationState = NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.SpineAnimation.GetAnimationState();
        megaAnimationState?.SetAnimation("_tracks/charge_up_2", loop: false, 1);
        megaAnimationState?.AddAnimation("_tracks/charged_2", 0f, loop: true, 1);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, WeakPowerAmount, instance.Creature, null);
        await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), instance.Creature, SlipperyPowerAmount, instance.Creature, null);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, StrengthPowerAmount, instance.Creature, null);
    }

    private static async Task InkyLanceMove(Vantom instance, IReadOnlyList<Creature> targets)
    {
        if (_inkyLanceMoveDelegate == null)
        {
            return;
        }

        await _inkyLanceMoveDelegate(instance, targets);
    }

    private static async Task DismemberMove(Vantom instance)
    {
        if (TestMode.IsOff && instance.Creature.IsAlive)
        {
            MegaAnimationState? megaAnimationState = NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.SpineAnimation.GetAnimationState();
            megaAnimationState?.SetAnimation("_tracks/attack_heavy", loop: false, 1);
            megaAnimationState?.AddAnimation("_tracks/charged_0", 0f, loop: true, 1);
        }
        NRunMusicController.Instance?.UpdateMusicParameter("vantom_progress", 3f);
        await CreatureCmd.TriggerAnim(instance.Creature, "ATTACK_HEAVY", 0f);
        await Cmd.Wait(0.25f);
        NCombatRoom.Instance?.RadialBlur(VfxPosition.Left);
        NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Normal, 180f + Rng.Chaotic.NextFloat(-10f, 10f));
        await DamageCmd.Attack(instance.DismemberDamage).FromMonster(instance).WithNoAttackerAnim().WithHitFx("vfx/vfx_giant_horizontal_slash", "event:/sfx/enemy/enemy_attacks/vantom/vantom_dismember").Execute(null);
        NGame.Instance?.DoHitStop(ShakeStrength.Weak, ShakeDuration.Short);
        await Cmd.Wait(0.5f);
    }

    private static async Task PrepareMove(Vantom instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/vantom/vantom_buff");
        await CreatureCmd.TriggerAnim(instance.Creature, "BUFF", 0.6f);
        if (!TestMode.IsOff || !instance.Creature.IsAlive)
        {
            return;
        }

        await Cmd.CustomScaledWait(1f, 1f);
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/vantom/vantom_extend_1");
        MegaAnimationState? megaAnimationState = NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.SpineAnimation.GetAnimationState();
        megaAnimationState?.SetAnimation("_tracks/charge_up_1", loop: false, 1);
        megaAnimationState?.AddAnimation("_tracks/charged_1", 0f, loop: true, 1);
        await CreatureCmd.TriggerAnim(instance.Creature, "CHARGE_UP", 0.25f);
        NRunMusicController.Instance?.UpdateMusicParameter("vantom_progress", 1f);
    }

    private static async Task AfterAddedToRoom(Vantom instance)
    {
        await PowerCmd.Apply<PainfulStabsPower>(new ThrowingPlayerChoiceContext(), instance.Creature, PainfulStabsPowerAmount, instance.Creature, null);
        await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), instance.Creature, InitSlipperyPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(Vantom), nameof(Vantom.InkyLanceDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceInkyLanceDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }

    [HarmonyPatch(typeof(Vantom), nameof(Vantom.DismemberDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceDismemberDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 4;
    }

    [HarmonyPatch(typeof(Vantom), nameof(Vantom.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(Vantom __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(Vantom), nameof(Vantom.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Vantom __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("INK_BLOT_MOVE", t => InkBlotMove(__instance, t), new BuffIntent(), new DebuffIntent());
        MoveState moveState2 = new MoveState("INKY_LANCE_MOVE", t => InkyLanceMove(__instance, t), new MultiAttackIntent(__instance.InkyLanceDamage, 2));
        MoveState moveState3 = new MoveState("DISMEMBER_MOVE", _ => DismemberMove(__instance), new SingleAttackIntent(__instance.DismemberDamage));
        MoveState moveState4 = new MoveState("PREPARE_MOVE", _ => PrepareMove(__instance), new StunIntent());
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