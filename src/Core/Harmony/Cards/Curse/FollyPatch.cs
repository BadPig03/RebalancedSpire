namespace RebalancedSpire.Core.Harmony.Cards.Curse;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FollyPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.PreservedFog;

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
            CardKeyword.Innate,
            CardKeyword.Eternal
        }.AsReadOnly();
        return false;
    }
}