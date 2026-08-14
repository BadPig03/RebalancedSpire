namespace RebalancedSpire.Core.Harmony.Cards.Colorless;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SplashPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Splash;

    [HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Splash)
        {
            return true;
        }

        __result = 0;
        return false;
    }
}