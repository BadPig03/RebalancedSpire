namespace RebalancedSpire.Core.Harmony.Cards.Curse;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class EnthralledPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BloodSoakedRoseConfig;

    [HarmonyPatch(typeof(Enthralled), "CanonicalKeywords", MethodType.Getter)]
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
        }.AsReadOnly();
        return false;
    }
}