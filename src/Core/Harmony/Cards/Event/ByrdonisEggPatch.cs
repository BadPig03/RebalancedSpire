namespace RebalancedSpire.Core.Harmony.Cards.Event;

using Afflictions;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ByrdonisEggPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ByrdonisConfig;

    [HarmonyPatch(typeof(CardModel), "ShouldGlowGoldInternal", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShouldGlowGoldInternal(CardModel __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ByrdonisEgg byrdonisEgg)
        {
            return true;
        }

        __result = byrdonisEgg.Affliction is ToItsOriginOwner;
        return false;
    }
}