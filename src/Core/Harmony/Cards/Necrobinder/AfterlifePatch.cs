namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

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
public static class AfterlifePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.AfterlifeConfig;

    private static async Task OnPlay(Afterlife instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<AfterlifePower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["AfterlifePower"].BaseValue, instance.Owner.Creature, instance);
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

        if (__instance is not Afterlife)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-AFTERLIFE.description");
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

        if (__instance is not Afterlife)
        {
            return true;
        }

        __result = CardType.Power;
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

        if (__instance is not Afterlife)
        {
            return true;
        }

        __result = CardRarity.Uncommon;
        return false;
    }

    [HarmonyPatch(typeof(Afterlife), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Afterlife __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new HealVar(4),
            new SummonVar(3),
            new PowerVar<AfterlifePower>(4)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Afterlife), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Afterlife __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>().AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Afterlife), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Afterlife __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(Afterlife), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Afterlife __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Summon.UpgradeValueBy(1);
        __instance.DynamicVars.Heal.UpgradeValueBy(1);
        __instance.DynamicVars["AfterlifePower"].UpgradeValueBy(1);
        return false;
    }
}