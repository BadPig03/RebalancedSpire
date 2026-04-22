namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryOvergrowth)]
// ReSharper disable InconsistentNaming
public static class LeafSlimeSPatch
{
    private static int SlimedAmount => 1;

    private static readonly Func<LeafSlimeS, IReadOnlyList<Creature>, Task>? _goopMoveDelegate = Helpers.GetDelegate<LeafSlimeS>("GoopMove");

    private static async Task GoopMove(LeafSlimeS instance, IReadOnlyList<Creature> targets)
    {
        if (_goopMoveDelegate == null)
        {
            return;
        }

        await _goopMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(LeafSlimeS), nameof(LeafSlimeS.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(LeafSlimeS __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("GOOP_MOVE", t => GoopMove(__instance, t), new StatusIntent(SlimedAmount));
        moveState.FollowUpState = moveState;
        list.Add(moveState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}