namespace RebalancedSpire.scr.Core.Harmony.Cards;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryVakuu)]
// ReSharper disable InconsistentNaming
public static class EnthralledPatch
{
    [HarmonyPatch(typeof(Enthralled), nameof(Enthralled.CanonicalKeywords), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Enthralled __instance, ref IEnumerable<CardKeyword> __result)
    {
        __result = new List<CardKeyword>
        {
            CardKeyword.Ethereal,
            CardKeyword.Eternal
        };
        return false;
    }
}