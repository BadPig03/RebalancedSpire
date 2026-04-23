namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Normal;

using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SlimedBerserkerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SlimedBerserkerConfig;

    private static int SlimedAmount => 5;
    private static int LeechingHugPowerAmount => 1;
    private static int WeakPowerAmount => 3;
    private static int SmotherDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 33, 30);
    private static int PummelingDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    private static int PummelingCount => 4;

    private static async Task VomitIchorMove(SlimedBerserker instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.SlimeSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Vomit", 0.7f);
        await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedAmount, addedByPlayer: false);
        await PowerCmd.Apply<LeechingHugPower>(instance.Creature, LeechingHugPowerAmount, instance.Creature, null);
    }

    private static async Task LeechingHugMove(SlimedBerserker instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.CastSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Hug", 0.65f);
        await PowerCmd.Apply<WeakPower>(targets, WeakPowerAmount, null, null);
    }

    private static async Task SmotherMove(SlimedBerserker instance)
    {
        await DamageCmd.Attack(SmotherDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.2f).WithAttackerFx(null, instance.AttackSfx).Execute(null);
    }

    private static async Task FuriousPummelingMove(SlimedBerserker instance)
    {
        await DamageCmd.Attack(PummelingDamage).WithHitCount(PummelingCount).OnlyPlayAnimOnce().FromMonster(instance).WithAttackerAnim("Attack", 0.2f).WithAttackerFx(null, instance.AttackSfx).Execute(null);
    }

    [HarmonyPatch(typeof(SlimedBerserker), nameof(SlimedBerserker.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 40;
    }

    [HarmonyPatch(typeof(SlimedBerserker), nameof(SlimedBerserker.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(SlimedBerserker __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("VOMIT_ICHOR_MOVE", t => VomitIchorMove(__instance, t), new StatusIntent(SlimedAmount), new BuffIntent());
        MoveState moveState2 = new MoveState("FURIOUS_PUMMELING_MOVE", _ => FuriousPummelingMove(__instance), new MultiAttackIntent(__instance.PummelingDamage, PummelingCount));
        MoveState moveState3 = new MoveState("LEECHING_HUG_MOVE", t => LeechingHugMove(__instance, t), new DebuffIntent());
        MoveState moveState4 = new MoveState("SMOTHER_MOVE", _ => SmotherMove(__instance), new SingleAttackIntent(SmotherDamage));
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