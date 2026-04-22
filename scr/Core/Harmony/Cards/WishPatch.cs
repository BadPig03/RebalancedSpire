namespace RebalancedSpire.scr.Core.Harmony.Cards;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryVakuu)]
// ReSharper disable InconsistentNaming
public static class WishPatch
{
    private static async Task OnPlay(Wish instance, PlayerChoiceContext choiceContext)
    {
        var cards = PileType.Draw.GetPile(instance.Owner).Cards.ToList();
        cards.AddRange(PileType.Discard.GetPile(instance.Owner).Cards);
        CardModel? cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, (from c in cards orderby c.Rarity, c.Id select c).ToList(), instance.Owner, new CardSelectorPrefs(instance.SelectionScreenPrompt, 1))).FirstOrDefault();
        if (cardModel == null)
        {
            return;
        }

        await CardPileCmd.Add(cardModel, PileType.Hand);
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(CardModel __instance, ref LocString __result)
    {
        if (__instance is not Wish)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-WISH.description");
        return false;
    }

    [HarmonyPatch(typeof(Wish), nameof(Wish.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Wish __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        __result = OnPlay(__instance, choiceContext);
        return false;
    }
}