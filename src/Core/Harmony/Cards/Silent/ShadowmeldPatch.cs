namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ShadowmeldPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Shadowmeld;

    [HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Shadowmeld)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Retain,
            CardKeyword.Exhaust
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Shadowmeld), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Shadowmeld __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.RemoveKeyword(CardKeyword.Exhaust);
        return false;
    }
}