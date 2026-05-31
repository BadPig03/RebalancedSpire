namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using Core.Enchantments;
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
public static class HiddenDaggersPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.HiddenDaggers;

    private static async Task OnPlay(HiddenDaggers instance, PlayerChoiceContext choiceContext)
    {
        var combatState = instance.CombatState;
        if (combatState == null)
        {
            return;
        }

        await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, instance.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, instance.DynamicVars.Cards.IntValue), null, instance));
        var shivs = (await Shiv.CreateInHand(instance.Owner, instance.DynamicVars["Shivs"].IntValue, combatState)).ToList();
        foreach (var shiv in shivs)
        {
            CardCmd.Enchant<Energetic>(shiv, 1);
        }
    }

    [HarmonyPatch(typeof(HiddenDaggers), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(HiddenDaggers __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(3),
            new("Shivs", 2)
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

        if (__instance is not HiddenDaggers)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_HIDDEN_DAGGERS.description");
        return false;
    }

    [HarmonyPatch(typeof(HiddenDaggers), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(HiddenDaggers __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        var shiv = (Shiv) ModelDb.Card<Shiv>().MutableClone();
        CardCmd.Enchant<Energetic>(shiv, 1);
        shiv.DynamicVars.Damage.BaseValue = 0;
        var list = new List<IHoverTip>
        {
            HoverTipFactory.FromCard(shiv)
        };
        list.AddRange(HoverTipFactory.FromEnchantment<Energetic>());
        __result = list.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(HiddenDaggers), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(HiddenDaggers __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(CardModel __instance)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HiddenDaggers)
        {
            return true;
        }

        __instance.DynamicVars.Cards.UpgradeValueBy(-1);
        return false;
    }
}