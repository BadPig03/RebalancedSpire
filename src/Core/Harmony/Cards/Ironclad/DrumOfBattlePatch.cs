namespace RebalancedSpire.Core.Harmony.Cards.Ironclad;

using Configs;
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
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DrumOfBattlePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DrumOfBattle;

    private static async Task OnPlay(DrumOfBattle instance, PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<VigorPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["VigorPower"].BaseValue, instance.Owner.Creature, instance);
    }

    [HarmonyPatch(typeof(DrumOfBattle), "AfterCardExhausted")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCardExhausted(DrumOfBattle __instance, PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
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

        if (__instance is not DrumOfBattle)
        {
            return true;
        }

        __result = 0;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not DrumOfBattle)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Innate,
            CardKeyword.Exhaust
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(DrumOfBattle), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(DrumOfBattle __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<VigorPower>(8)
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

        if (__instance is not DrumOfBattle)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_DRUM_OF_BATTLE.description");
        return false;
    }

    [HarmonyPatch(typeof(DrumOfBattle), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(DrumOfBattle __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>().AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(DrumOfBattle), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(DrumOfBattle __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(DrumOfBattle), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(DrumOfBattle __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars["VigorPower"].UpgradeValueBy(4);
        return false;
    }
}