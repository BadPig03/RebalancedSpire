namespace RebalancedSpire.Core.Harmony.Monsters.Overgrowth.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FuzzyWurmCrawlerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FuzzyWurmCrawlerConfig;

    [HarmonyPatch(typeof(FuzzyWurmCrawler), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(FuzzyWurmCrawler __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("ACID_GOOP", __instance.AcidGoop, new SingleAttackIntent(__instance.AcidGoopDamage));
        MoveState moveState2 = new MoveState("ACID_GOOP2", __instance.AcidGoop, new SingleAttackIntent(__instance.AcidGoopDamage));
        MoveState moveState3 = new MoveState("INHALE", __instance.Inhale, new BuffIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}