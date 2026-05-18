namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SwordOfStonePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SunkenStatueConfig;

    [HarmonyPatch(typeof(SwordOfStone), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(SwordOfStone __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("Elites", 3)
        }.AsReadOnly();
        return false;
    }
}