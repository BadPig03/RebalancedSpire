namespace RebalancedSpire.scr.Core.Harmony.Cards;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategorySilent)]
// ReSharper disable InconsistentNaming
public static class AcrobaticsPatch
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Rarity), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is not Acrobatics)
        {
            return true;
        }

        __result = CardRarity.Common;
        return false;
    }
}