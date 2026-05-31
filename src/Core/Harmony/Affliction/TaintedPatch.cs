namespace RebalancedSpire.Core.Harmony.Affliction;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TaintedPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.InfestedPrism;

    [HarmonyPatch(typeof(AfflictionModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(AfflictionModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Tainted)
        {
            return true;
        }

        __result = new LocString("afflictions", "REBALANCED_SPIRE_AFFLICTION_TAINTED.description");
        return false;
    }

    [HarmonyPatch(typeof(Tainted), "IsStackable", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_IsStackable(Tainted __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Tainted), "HasExtraCardText", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_HasExtraCardText(Tainted __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Tainted), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(Tainted __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Tainted), "CanAfflictCardType")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanAfflictCardType(Tainted __instance, CardType cardType, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(AfflictionModel), "CanAfflictUnplayableCards", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanAfflictUnplayableCards(AfflictionModel __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Tainted)
        {
            return true;
        }

        __result = false;
        return false;
    }
}