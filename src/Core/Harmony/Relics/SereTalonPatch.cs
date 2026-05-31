namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
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

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SereTalonPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.SereTalon;

    private static async Task AfterObtained(SereTalon instance)
    {
        List<CardPileAddResult> curseResults = [];
        for (var i = 0; i < instance.DynamicVars["CurseCards"].IntValue; i++)
        {
            CardModel card = instance.Owner.RunState.CreateCard(ModelDb.Card<Guilty>(), instance.Owner);
            curseResults.Add(await CardPileCmd.Add(card, PileType.Deck));
        }
        CardCmd.PreviewCardPileAdd(curseResults, 2f);

        List<CardPileAddResult> wishResults = [];
        for (var i = 0; i < instance.DynamicVars["WishCards"].IntValue; i++)
        {
            CardModel card = instance.Owner.RunState.CreateCard(ModelDb.Card<Wish>(), instance.Owner);
            wishResults.Add(await CardPileCmd.Add(card, PileType.Deck));
        }
        CardCmd.PreviewCardPileAdd(wishResults, 2f);
        await Cmd.Wait(0.75f);
    }

    [HarmonyPatch(typeof(SereTalon), "AfterObtained")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(SereTalon __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterObtained(__instance);
        return false;
    }

    [HarmonyPatch(typeof(SereTalon), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(SereTalon __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new StringVar("Guilty", ModelDb.Card<Guilty>().Title),
            new StringVar("Wish", ModelDb.Card<Wish>().Title),
            new CardsVar("CurseCards", 1),
            new CardsVar("WishCards", 2)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SereTalon)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_SERE_TALON.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "EventDescription", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SereTalon)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_SERE_TALON.eventDescription");
        return false;
    }

    [HarmonyPatch(typeof(SereTalon), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(SereTalon __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        var list = new List<IHoverTip>();
        list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Guilty>());
        list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Wish>());
        __result = list.AsReadOnly();
        return false;
    }
}