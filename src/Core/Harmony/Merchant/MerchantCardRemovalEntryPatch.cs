namespace RebalancedSpire.Core.Harmony.Merchant;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Merchant;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MerchantCardRemovalEntryPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.LessPriceIncrease;

    [HarmonyPatch(typeof(MerchantCardRemovalEntry), "PriceIncrease", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_PriceIncrease(MerchantCardRemovalEntry __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = 25;
        return false;
    }
}