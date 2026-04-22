namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryVakuu)]
// ReSharper disable InconsistentNaming
public static class SereTalonPatch
{
    private static async Task AfterObtained(SereTalon instance)
    {
        List<CardPileAddResult> curseResults = [];
        for (var i = 0; i < instance.DynamicVars["CurseCards"].IntValue; i++)
        {
            CardModel card = instance.Owner.RunState.CreateCard(ModelDb.Card<Writhe>(), instance.Owner);
            curseResults.Add(await CardPileCmd.Add(card, PileType.Deck));
        }

        List<CardPileAddResult> wishResults = [];
        for (var i = 0; i < instance.DynamicVars["WishCards"].IntValue; i++)
        {
            CardModel card = instance.Owner.RunState.CreateCard(ModelDb.Card<Wish>(), instance.Owner);
            wishResults.Add(await CardPileCmd.Add(card, PileType.Deck));
        }
        CardCmd.PreviewCardPileAdd(curseResults, 2f);
        CardCmd.PreviewCardPileAdd(wishResults, 2f);
        await Cmd.Wait(0.75f);
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (__instance is not SereTalon)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-SERE_TALON.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.EventDescription), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (__instance is not SereTalon)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-SERE_TALON.eventDescription");
        return false;
    }

    [HarmonyPatch(typeof(SereTalon), nameof(SereTalon.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(SereTalon __instance, ref IEnumerable<DynamicVar> __result)
    {
        __result = new List<DynamicVar>
        {
            new StringVar("Writhe", ModelDb.Card<Writhe>().Title),
            new StringVar("Wish", ModelDb.Card<Wish>().Title),
            new CardsVar("CurseCards", 1),
            new CardsVar("WishCards", 2)
        };
        return false;
    }

    [HarmonyPatch(typeof(SereTalon), nameof(SereTalon.ExtraHoverTips), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(SereTalon __instance, ref IEnumerable<IHoverTip> __result)
    {
        var list = new List<IHoverTip>();
        list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Writhe>());
        list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Wish>());
        __result = list;
        return false;
    }

    [HarmonyPatch(typeof(SereTalon), nameof(SereTalon.AfterObtained))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(SereTalon __instance, ref Task __result)
    {
        __result = AfterObtained(__instance);
        return false;
    }
}