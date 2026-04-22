namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryVakuu)]
// ReSharper disable InconsistentNaming
public static class PreservedFogPatch
{
    [HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(PreservedFog __instance, ref IEnumerable<DynamicVar> __result)
    {
        __result = new List<DynamicVar>
        {
            new CardsVar(5)
        };
        return false;
    }
}