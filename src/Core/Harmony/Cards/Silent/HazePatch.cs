namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class HazePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Haze;

    [HarmonyPatch(typeof(Haze), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Haze __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<PoisonPower>(4),
            new PowerVar<WeakPower>(2)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Haze)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Exhaust
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Haze), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Haze __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Poison.UpgradeValueBy(3);
        return false;
    }
}