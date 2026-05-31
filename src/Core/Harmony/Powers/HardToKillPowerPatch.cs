namespace RebalancedSpire.Core.Harmony.Powers;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class HardToKillPowerPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Exoskeleton;

    [HarmonyPatch(typeof(PowerModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(PowerModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HardToKillPower)
        {
            return true;
        }

        __result = new LocString("powers", "REBALANCED_SPIRE_POWER_HARD_TO_KILL_POWER.description");
        return false;
    }

    [HarmonyPatch(typeof(PowerModel), "SmartDescriptionLocKey", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_SmartDescriptionLocKey(PowerModel __instance, ref string __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not HardToKillPower)
        {
            return true;
        }

        __result = "REBALANCED_SPIRE_POWER_HARD_TO_KILL_POWER.smartDescription";
        return false;
    }
}