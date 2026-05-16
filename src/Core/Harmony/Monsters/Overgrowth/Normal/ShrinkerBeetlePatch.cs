namespace RebalancedSpire.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ShrinkerBeetlePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ShrinkerBeetleConfig;

    private static int StompDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private static int StompCount => 2;

    private static async Task StompMove(ShrinkerBeetle instance)
    {
        await DamageCmd.Attack(StompDamage).WithHitCount(StompCount).FromMonster(instance).WithAttackerAnim("Attack", 0.25f).WithAttackerFx(null, SrcHelpers.GetSfx(instance, "AttackSfx")).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    [HarmonyPatch(typeof(ShrinkerBeetle), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(ShrinkerBeetle __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SHRINKER_MOVE", __instance.ShrinkMove, new DebuffIntent(strong: true));
        MoveState moveState2 = new MoveState("CHOMP_MOVE", __instance.ChompMove, new SingleAttackIntent(__instance.ChompDamage));
        MoveState moveState3 = new MoveState("STOMP_MOVE", _ => StompMove(__instance), new MultiAttackIntent(StompDamage, StompCount));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}