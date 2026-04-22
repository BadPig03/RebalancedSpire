namespace RebalancedSpire.scr.Core.Harmony.Merchant;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Merchant;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryMerchant)]
// ReSharper disable InconsistentNaming
public static class MerchantCardRemovalEntryPatch
{
    private static int PriceIncrease => 25;

    [HarmonyPatch(typeof(MerchantCardRemovalEntry), nameof(MerchantCardRemovalEntry.PriceIncrease), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_PriceIncrease(MerchantCardRemovalEntry __instance, ref int __result)
    {
        __result = PriceIncrease;
        return false;
    }
}