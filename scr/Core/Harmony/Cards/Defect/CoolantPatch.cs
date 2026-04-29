namespace RebalancedSpire.scr.Core.Harmony.Cards.Defect;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CoolantPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.CoolantConfig;

    [HarmonyPatch(typeof(Coolant), nameof(Coolant.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Coolant __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<CoolantPower>(3)
        };
        return false;
    }
}