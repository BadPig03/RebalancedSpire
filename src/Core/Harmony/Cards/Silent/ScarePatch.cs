namespace RebalancedSpire.Core.Harmony.Cards.Silent;

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
public static class ScarePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Scare;

    private static async Task OnPlay(Scare instance, PlayerChoiceContext choiceContext)
    {
        if (instance.CombatState == null)
        {
            return;
        }

        await PowerCmd.Apply<WeakPower>(choiceContext, instance.CombatState.HittableEnemies, instance.DynamicVars.Weak.BaseValue, instance.Owner.Creature, instance);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, instance.CombatState.HittableEnemies, instance.DynamicVars.Vulnerable.BaseValue, instance.Owner.Creature, instance);
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

        if (__instance is not Scare)
        {
            return true;
        }

        __result = 2;
        return false;
    }

    [HarmonyPatch(typeof(Scare), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Scare __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Sly
        }.AsReadOnly();
        return false;
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

        if (__instance is not Scare)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<WeakPower>(1),
            new PowerVar<VulnerablePower>(1)
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

        if (__instance is not Scare)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_SCARE.description");
        return false;
    }

    [HarmonyPatch(typeof(Scare), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(Scare __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<VulnerablePower>()
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Scare), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Scare __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(Scare), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Scare __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Weak.UpgradeValueBy(1);
        __instance.DynamicVars.Vulnerable.UpgradeValueBy(1);
        return false;
    }
}