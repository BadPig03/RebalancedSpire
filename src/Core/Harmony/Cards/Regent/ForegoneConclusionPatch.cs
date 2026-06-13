namespace RebalancedSpire.Core.Harmony.Cards.Regent;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ForegoneConclusionPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.ForegoneConclusion;

    private static async Task OnPlay(ForegoneConclusion instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ForegoneConclusionPlusPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars.Cards.BaseValue, instance.Owner.Creature, instance);
    }

    [HarmonyPatch(typeof(ForegoneConclusion), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(ForegoneConclusion __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(1)
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

        if (__instance is not ForegoneConclusion)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_FOREGONE_CONCLUSION.description");
        return false;
    }

    [HarmonyPatch(typeof(ForegoneConclusion), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(ForegoneConclusion __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(ForegoneConclusion), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(ForegoneConclusion __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.EnergyCost.UpgradeBy(-1);
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

        if (__instance is not ForegoneConclusion)
        {
            return true;
        }

        __result = CardType.Power;
        return false;
    }
}