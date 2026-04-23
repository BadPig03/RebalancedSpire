namespace RebalancedSpire.scr.Core.Harmony.Merchant;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Merchant;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MerchantCardRemovalEntryPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.MerchantConfig;

    private static int PriceIncrease => 25;

    [HarmonyPatch(typeof(MerchantCardRemovalEntry), nameof(MerchantCardRemovalEntry.PriceIncrease), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_PriceIncrease(MerchantCardRemovalEntry __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = PriceIncrease;
        return false;
    }
}