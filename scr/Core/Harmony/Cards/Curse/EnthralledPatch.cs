namespace RebalancedSpire.scr.Core.Harmony.Cards.Curse;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class EnthralledPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.VakuuConfig;

    [HarmonyPatch(typeof(Enthralled), nameof(Enthralled.CanonicalKeywords), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Enthralled __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Ethereal,
            CardKeyword.Eternal
        };
        return false;
    }
}