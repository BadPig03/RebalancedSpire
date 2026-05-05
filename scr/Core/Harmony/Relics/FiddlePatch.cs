namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FiddlePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FiddleConfig;

    private static int MaxAllowedCardsInHand => 8;

    private static async Task AfterCardDrawn(Fiddle instance, PlayerChoiceContext choiceContext)
    {
        var hands = CardPile.GetCards(instance.Owner, PileType.Hand).ToList();
        if (hands.Count <= MaxAllowedCardsInHand)
        {
            return;
        }

        instance.Flash();
        for (var i = MaxAllowedCardsInHand; i < CardPile.maxCardsInHand; i++)
        {
            var cardModel = hands.ElementAtOrDefault(i);
            if (cardModel == null)
            {
                continue;
            }

            await CardCmd.Discard(choiceContext, cardModel);
        }
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Fiddle)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-FIDDLE.description");
        return false;
    }

    [HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ShouldDraw))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShouldDraw(Fiddle __instance, Player player, bool fromHandDraw, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (player != __instance.Owner)
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCardDrawn))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCardDrawn(AbstractModel __instance, PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Fiddle fiddle)
        {
            return true;
        }

        __result = AfterCardDrawn(fiddle, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(Fiddle), nameof(Fiddle.AfterPreventingDraw))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterPreventingDraw(ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }
}