namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class EyeWithTeethPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FogmogConfig;

    private static int DazedAmount => 2;

    private static async Task DistractMove(EyeWithTeeth instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.AttackSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Attack", 0.7f);
        VfxCmd.PlayOnCreatureCenters(targets, "vfx/vfx_attack_slash");
        await CardPileCmd.AddToCombatAndPreview<Dazed>(targets, PileType.Discard, DazedAmount, addedByPlayer: false);
    }

    [HarmonyPatch(typeof(EyeWithTeeth), nameof(EyeWithTeeth.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(EyeWithTeeth __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("DISTRACT_MOVE", t => DistractMove(__instance, t), new StatusIntent(DazedAmount));
        moveState.FollowUpState = moveState;
        list.Add(moveState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}