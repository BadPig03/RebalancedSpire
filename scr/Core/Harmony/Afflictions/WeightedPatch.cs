namespace RebalancedSpire.scr.Core.Harmony.Afflictions;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;

[HarmonyPatch]

// ReSharper disable InconsistentNaming
public static class WeightedPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DoormakerConfig;

    [HarmonyPatch(typeof(Weighted), nameof(Weighted.HasExtraCardText), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_HasExtraCardText(Weighted __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Weighted), nameof(Weighted.ExtraHoverTips), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(Weighted __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = [];
        return false;
    }

    [HarmonyPatch(typeof(Weighted), nameof(Weighted.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Weighted __instance, PlayerChoiceContext choiceContext, Creature? target, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AfflictionModel), nameof(AfflictionModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(AfflictionModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Weighted)
        {
            return true;
        }

        __result = new LocString("afflictions", "REBALANCEDSPIRE-WEIGHTED.description");
        return false;
    }

    [HarmonyPatch(typeof(AfflictionModel), nameof(AfflictionModel.CanAfflictCardType))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanAfflict(AfflictionModel __instance, CardType cardType, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Weighted)
        {
            return true;
        }

        __result = true;
        return false;
    }
}