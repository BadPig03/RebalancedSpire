namespace RebalancedSpire.scr.Core.Harmony.Cards.Defect;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RefractPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.RefractConfig;

    private static async Task OnPlay(Refract instance, PlayerChoiceContext choiceContext)
    {
        for (var i = 0; i < instance.DynamicVars.Repeat.IntValue; i++)
        {
            await OrbCmd.Channel<GlassOrb>(choiceContext, instance.Owner);
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(CardModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Refract)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-REFRACT.description");
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Rarity), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Refract)
        {
            return true;
        }

        __result = CardRarity.Common;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Refract)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Exhaust
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Type), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Type(CardModel __instance, ref CardType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Refract)
        {
            return true;
        }

        __result = CardType.Skill;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.TargetType), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TargetType(CardModel __instance, ref TargetType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Refract)
        {
            return true;
        }

        __result = TargetType.Self;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.CanonicalEnergyCost), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Refract)
        {
            return true;
        }

        __result = 1;
        return false;
    }

    [HarmonyPatch(typeof(Refract), nameof(Refract.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Refract __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new RepeatVar(2)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Refract), nameof(Refract.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Refract __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(Refract), nameof(Refract.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Refract __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.RemoveKeyword(CardKeyword.Exhaust);
        return false;
    }
}