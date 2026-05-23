namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DebilitatePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DebilitateConfig;

    [HarmonyPatch(typeof(Debilitate), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Debilitate __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(10, ValueProp.Move),
            new PowerVar<DebilitatePower>(3)
        }.AsReadOnly();
        return false;
    }
}