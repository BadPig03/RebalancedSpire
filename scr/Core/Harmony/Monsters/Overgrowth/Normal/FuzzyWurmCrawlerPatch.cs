namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FuzzyWurmCrawlerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FuzzyWurmCrawlerConfig;

    private static readonly Func<FuzzyWurmCrawler, IReadOnlyList<Creature>, Task>? _inhaleMoveDelegate = Helpers.GetDelegate<FuzzyWurmCrawler>("Inhale");
    private static readonly Func<FuzzyWurmCrawler, IReadOnlyList<Creature>, Task>? _acidGoopMoveDelegate = Helpers.GetDelegate<FuzzyWurmCrawler>("AcidGoop");

    private static async Task InhaleMove(FuzzyWurmCrawler instance, IReadOnlyList<Creature> targets)
    {
        if (_inhaleMoveDelegate == null)
        {
            return;
        }

        await _inhaleMoveDelegate(instance, targets);
    }

    private static async Task AcidGoopMove(FuzzyWurmCrawler instance, IReadOnlyList<Creature> targets)
    {
        if (_acidGoopMoveDelegate == null)
        {
            return;
        }

        await _acidGoopMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(FuzzyWurmCrawler), nameof(FuzzyWurmCrawler.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(FuzzyWurmCrawler __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("ACID_GOOP", t => AcidGoopMove(__instance, t), new SingleAttackIntent(__instance.AcidGoopDamage));
        MoveState moveState2 = new MoveState("ACID_GOOP2", t => AcidGoopMove(__instance, t), new SingleAttackIntent(__instance.AcidGoopDamage));
        MoveState moveState3 = new MoveState("INHALE", t => InhaleMove(__instance, t), new BuffIntent());
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