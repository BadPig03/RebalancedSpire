namespace RebalancedSpire.scr.Core.Harmony.Cards.Necrobinder;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BansheesCryPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BansheesCryConfig;

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.CanonicalEnergyCost), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        if (__instance is not BansheesCry)
        {
            return;
        }

        __result -= 1;
    }
}