namespace RebalancedSpire.scr.Core.Harmony.Cards;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryOrobas)]
// ReSharper disable InconsistentNaming
public static class LuminescePatch
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Rarity), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is not Luminesce)
        {
            return true;
        }

        __result = CardRarity.Ancient;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.PortraitPath), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_PortraitPath(CardModel __instance, ref string __result)
    {
        if (__instance is not Luminesce)
        {
            return true;
        }

        __result = "images/packed/card_portraits/event/rebalancedspire_luminesce.png";
        return false;
    }
}