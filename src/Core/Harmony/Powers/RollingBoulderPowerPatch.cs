namespace RebalancedSpire.Core.Harmony.Powers;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RollingBoulderPowerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.RollingBoulderConfig;

    [HarmonyPatch(typeof(RollingBoulderPower), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(RollingBoulderPower __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(10, ValueProp.Unpowered)
        }.AsReadOnly();
        return false;
    }
}