namespace RebalancedSpire.scr.Core.Harmony.Cards.Curse;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BadLuckPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ReflectionsConfig;

    [HarmonyPatch(typeof(BadLuck), nameof(BadLuck.CanonicalKeywords), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(BadLuck __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Unplayable
        }.AsReadOnly();
        return false;
    }
}