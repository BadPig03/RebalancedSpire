namespace RebalancedSpire.Core.Harmony.Cards.Regent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class HeirloomHammerPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.HeirloomHammer;

    private static async Task OnPlay(HeirloomHammer instance, PlayerChoiceContext choiceContext)
    {
        await ForgeCmd.Forge(instance.DynamicVars.Forge.BaseValue, instance.Owner, instance);
        var selected = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(instance.SelectionScreenPrompt, 1), context: choiceContext, player: instance.Owner, filter: c => c.VisualCardPool.IsColorless, source: instance)).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        var cloned = selected.CreateClone();
        if (instance.IsUpgraded)
        {
            CardCmd.Upgrade(cloned);
        }
        await CardPileCmd.AddGeneratedCardToCombat(cloned, PileType.Hand, instance.Owner);
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HeirloomHammer)
        {
            return true;
        }

        __result = 1;
        return false;
    }

    [HarmonyPatch(typeof(HeirloomHammer), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(HeirloomHammer __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new ForgeVar(5)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(CardModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HeirloomHammer)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_HEIRLOOM_HAMMER.description");
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HeirloomHammer)
        {
            return true;
        }

        var list = new List<IHoverTip>();
        list.AddRange(HoverTipFactory.FromForge());
        __result = list.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(HeirloomHammer), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(HeirloomHammer __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(HeirloomHammer), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(HeirloomHammer __instance)
    {
        return Disabled;
    }

    [HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TargetType(CardModel __instance, ref TargetType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HeirloomHammer)
        {
            return true;
        }

        __result = TargetType.Self;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Type(CardModel __instance, ref CardType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HeirloomHammer)
        {
            return true;
        }

        __result = CardType.Skill;
        return false;
    }
}