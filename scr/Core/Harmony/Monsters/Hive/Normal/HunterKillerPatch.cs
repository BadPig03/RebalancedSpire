namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class HunterKillerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.HunterKillerConfig;

    private static int BiteDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 16, 14);
    private static int PunctureDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private static int PunctureCount => 3;
    private static int WeakPowerAmount => 1;

    private static readonly Func<HunterKiller, IReadOnlyList<Creature>, Task>? _goopMoveDelegate = Helpers.GetDelegate<HunterKiller>("GoopMove");

    private static async Task GoopMove(HunterKiller instance, IReadOnlyList<Creature> targets)
    {
        if (_goopMoveDelegate == null)
        {
            return;
        }

        await _goopMoveDelegate(instance, targets);
    }

    private static async Task BiteMove(HunterKiller instance)
    {
        await DamageCmd.Attack(BiteDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_bite").Execute(null);
    }

    private static async Task PunctureMove(HunterKiller instance)
    {
        await DamageCmd.Attack(PunctureDamage).WithHitCount(PunctureCount).OnlyPlayAnimOnce().FromMonster(instance).WithAttackerAnim("TripleAttack", 0.3f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task WeakGoopMove(HunterKiller instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.CastSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.4f);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, WeakPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(HunterKiller), nameof(HunterKiller.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(HunterKiller __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("TENDERIZING_GOOP_MOVE", t => GoopMove(__instance, t), new DebuffIntent());
        MoveState moveState2 = new MoveState("BITE_MOVE", _ => BiteMove(__instance), new SingleAttackIntent(BiteDamage));
        MoveState moveState3 = new MoveState("PUNCTURE_MOVE", _ => PunctureMove(__instance), new MultiAttackIntent(PunctureDamage, PunctureCount));
        MoveState moveState4 = new MoveState("WEAK_GOOP_MOVE", t => WeakGoopMove(__instance, t), new DebuffIntent());
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