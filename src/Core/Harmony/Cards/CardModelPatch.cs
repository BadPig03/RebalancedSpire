namespace RebalancedSpire.Core.Harmony.Cards;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CardModelPatch
{
    [HarmonyPatch(typeof(CardModel), "ShouldShowInCardLibrary", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_ShouldShowInCardLibrary(CardModel __instance, ref bool __result)
    {
        if (__instance is not (Core.Cards.CorpseExplosion or Core.Cards.LimitBreak))
        {
            return;
        }

        __result = RebalancedSpireRuntimeSettings.Current.DustyTome;
    }
}
