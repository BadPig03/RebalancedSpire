namespace RebalancedSpire.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PreservedFogPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.PreservedFogConfig;

    [HarmonyPatch(typeof(PreservedFog), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(PreservedFog __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(5)
        }.AsReadOnly();
        return false;
    }
}