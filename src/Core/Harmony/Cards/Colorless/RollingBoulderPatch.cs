namespace RebalancedSpire.Core.Harmony.Cards.Colorless;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RollingBoulderPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.RollingBoulderConfig;

    [HarmonyPatch(typeof(RollingBoulder), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(RollingBoulder __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<RollingBoulderPower>(10),
            new("IncrementAmount", 10)
        }.AsReadOnly();
        return false;
    }
}