namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class NoisebotPatch
{
    private static int DazedAmount => 1;

    private static async Task NoiseMove(Noisebot instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.CastSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.6f);
        List<CardPileAddResult> statusCards = [];
        foreach (Creature target in targets)
        {
            Player? player = target.Player ?? target.PetOwner;
            if (player == null)
            {
                continue;
            }

            CardModel card = instance.CombatState.CreateCard<Dazed>(player);
            statusCards.Add(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, addedByPlayer: false));
            if (!LocalContext.IsMe(player))
            {
                continue;
            }

            CardCmd.PreviewCardPileAdd(statusCards);
            await Cmd.Wait(1f);
        }
    }

    [HarmonyPatch(typeof(Noisebot), nameof(Noisebot.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        __result = 18;
    }

    [HarmonyPatch(typeof(Noisebot), nameof(Noisebot.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(ref int __result)
    {
        __result = 18;
    }

    [HarmonyPatch(typeof(Noisebot), nameof(Noisebot.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Noisebot __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("NOISE_MOVE", t => NoiseMove(__instance, t), new StatusIntent(DazedAmount));
        moveState.FollowUpState = moveState;
        list.Add(moveState);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}