namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
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
public class WellLaidPlansPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.WellLaidPlans;

    private static async Task OnPlay(WellLaidPlans instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "PowerUp", instance.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<WellLaidPlansPlusPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["RetainAmount"].BaseValue, instance.Owner.Creature, instance);
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(CardModel __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not WellLaidPlans)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("RetainAmount", 1m)
        }.AsReadOnly();
        return false;
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

        if (__instance is not WellLaidPlans)
        {
            return true;
        }

        __result = 1;
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

        if (__instance is not WellLaidPlans)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_WELL_LAID_PLANS.description");
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

        if (__instance is not WellLaidPlans)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromKeyword(CardKeyword.Retain)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(WellLaidPlans), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(WellLaidPlans __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(WellLaidPlans), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(WellLaidPlans __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars["RetainAmount"].UpgradeValueBy(1);
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not WellLaidPlans)
        {
            return true;
        }

        __result = CardRarity.Uncommon;
        return false;
    }
}