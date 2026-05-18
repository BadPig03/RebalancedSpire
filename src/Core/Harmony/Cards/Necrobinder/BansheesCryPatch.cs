namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BansheesCryPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BansheesCryConfig;

    [HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not BansheesCry)
        {
            return true;
        }

        __result = 8;
        return false;
    }
}