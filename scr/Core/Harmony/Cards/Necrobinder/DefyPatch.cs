namespace RebalancedSpire.scr.Core.Harmony.Cards.Necrobinder;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DefyPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DefyConfig;

    [HarmonyPatch(typeof(Defy), nameof(Defy.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Defy __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Block.UpgradeValueBy(1);
        __instance.DynamicVars.Weak.UpgradeValueBy(1);
        return false;
    }
}