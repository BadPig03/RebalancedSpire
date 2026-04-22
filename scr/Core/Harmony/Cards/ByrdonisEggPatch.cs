using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using RebalancedSpire.scr.Core.Afflictions;

namespace RebalancedSpire.scr.Core.Harmony.Cards;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryOvergrowth)]
// ReSharper disable InconsistentNaming
public static class ByrdonisEggPatch
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.ShouldGlowGold), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance is not ByrdonisEgg byrdonisEgg)
        {
            return;
        }

        __result |= byrdonisEgg.Affliction is ToItsOriginOwner;
    }
}