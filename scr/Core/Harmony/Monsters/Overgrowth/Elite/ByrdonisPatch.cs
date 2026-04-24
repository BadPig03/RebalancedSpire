namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Elite;

using Core.Afflictions;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Byrdpip = MegaCrit.Sts2.Core.Models.Relics.Byrdpip;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ByrdonisPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ByrdonisConfig;

    private static int TerritorialPowerAmount => 1;
    private static int FrailPowerAmount => 2;
    private static int SwoopDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);
    private static int WeakAmount => 99;

    private static async Task AngryMove(Byrdonis instance, IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(instance.Creature, "AngryPermanently", 0.8f);
        await PowerCmd.Apply<TerritorialPower>(new ThrowingPlayerChoiceContext(), instance.Creature, TerritorialPowerAmount, instance.Creature, null);
        foreach (Player player in instance.CombatState.Players)
        {
            var allCards = player.PlayerCombatState?.AllCards;
            if (allCards == null)
            {
                continue;
            }

            foreach (CardModel cardModel in allCards)
            {
                if (cardModel is not ByrdonisEgg)
                {
                    continue;
                }

                await CardCmd.AfflictAndPreview<ToItsOriginOwner>(new List<CardModel> { cardModel }, 1);
            }
        }
        await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), targets, FrailPowerAmount, instance.Creature, null);
    }

    private static async Task PeckMove(Byrdonis instance)
    {
        await DamageCmd.Attack(Byrdonis.PeckDamage).WithHitCount(Byrdonis.PeckRepeat).FromMonster(instance).WithAttackerAnim("AttackAngry", 0.4f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task SwoopMove(Byrdonis instance)
    {
        await DamageCmd.Attack(SwoopDamage).FromMonster(instance).WithAttackerAnim("AttackAngry", 0.4f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task AfterAddedToRoom(Byrdonis instance)
    {
        foreach (Player player in instance.CombatState.Players)
        {
            if (player.GetRelic<Byrdpip>() == null)
            {
                continue;
            }

            await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), instance.Creature, WeakAmount, player.Creature, null);
            return;
        }
    }

    private static bool IsAngry(Byrdonis instance)
    {
        return instance.Creature.GetPower<TerritorialPower>() != null;
    }

    [HarmonyPatch(typeof(Byrdonis), nameof(Byrdonis.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(Byrdonis __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(Byrdonis), nameof(Byrdonis.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Byrdonis __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("ANGRY_MOVE", t => AngryMove(__instance, t), new DebuffIntent(), new BuffIntent());
        MoveState moveState2 = new MoveState("PECK_MOVE", _ => PeckMove(__instance), new MultiAttackIntent(Byrdonis.PeckDamage, Byrdonis.PeckRepeat));
        MoveState moveState3 = new MoveState("SWOOP_MOVE", _ => SwoopMove(__instance), new SingleAttackIntent(SwoopDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }

    [HarmonyPatch(typeof(Byrdonis), nameof(Byrdonis.GenerateAnimator))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_GenerateAnimator(Byrdonis __instance, MegaSprite controller, ref CreatureAnimator __result)
    {
        if (Disabled)
        {
            return true;
        }

        AnimState initialState = new AnimState("idle_loop", true);
        AnimState initialAngryState = new AnimState("angry_loop", true);
        AnimState state1 = new AnimState("hurt");
        AnimState state2 = new AnimState("attack");
        AnimState state3 = new AnimState("die");
        AnimState state4 = new AnimState("get_angry");
        AnimState state5 = new AnimState("get_angry_permanently");
        AnimState state6 = new AnimState("attack_angry");
        AnimState state7 = new AnimState("hurt_angry");
        AnimState state8 = new AnimState("get_not_angry");
        state1.NextState = initialState;
        state2.NextState = initialState;
        state4.NextState = initialState;
        state5.NextState = initialAngryState;
        state6.NextState = initialAngryState;
        state7.NextState = initialAngryState;
        state8.NextState = initialState;
        CreatureAnimator animator = new CreatureAnimator(initialState, controller);
        initialAngryState.AddBranch("Hit", state7, () => IsAngry(__instance));
        initialAngryState.AddBranch("Hit", state1, () => !IsAngry(__instance));
        initialState.AddBranch("Hit", state7, () => IsAngry(__instance));
        initialState.AddBranch("Hit", state1, () => !IsAngry(__instance));
        animator.AddAnyState("Attack", state2);
        animator.AddAnyState("Dead", state3);
        animator.AddAnyState("Angry", state4);
        animator.AddAnyState("AngryPermanently", state5);
        animator.AddAnyState("AttackAngry", state6);
        animator.AddAnyState("HitAngry", state7);
        animator.AddAnyState("NotAngry", state8);
        __result = animator;
        return false;
    }
}