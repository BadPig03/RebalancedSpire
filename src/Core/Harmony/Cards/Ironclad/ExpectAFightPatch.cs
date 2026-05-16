namespace RebalancedSpire.Core.Harmony.Cards.Ironclad;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ExpectAFightPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ExpectAFightConfig;

    [HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ExpectAFight)
        {
            return true;
        }

        __result = 1;
        return false;
    }
}