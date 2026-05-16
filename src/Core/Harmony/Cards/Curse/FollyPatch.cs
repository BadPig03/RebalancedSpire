namespace RebalancedSpire.Core.Harmony.Cards.Curse;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FollyPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.PreservedFogConfig;

    [HarmonyPatch(typeof(Folly), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Folly __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Unplayable,
            CardKeyword.Eternal,
            CardKeyword.Innate
        }.AsReadOnly();
        return false;
    }
}