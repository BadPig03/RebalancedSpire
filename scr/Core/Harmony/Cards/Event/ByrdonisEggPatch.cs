namespace RebalancedSpire.scr.Core.Harmony.Cards.Event;

using Core.Afflictions;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ByrdonisEggPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ByrdonisConfig;

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.ShouldGlowGold), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Postfix(CardModel __instance, ref bool __result)
    {
        if (Disabled)
        {
            return;
        }

        if (__instance is not ByrdonisEgg byrdonisEgg)
        {
            return;
        }

        __result |= byrdonisEgg.Affliction is ToItsOriginOwner;
    }
}