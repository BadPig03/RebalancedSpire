namespace RebalancedSpire.Core.Harmony.Cards.Event;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WishPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.SereTalon;

    [HarmonyPatch(typeof(Wish), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Wish __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Innate,
            CardKeyword.Exhaust
        }.AsReadOnly();
        return false;
    }
}