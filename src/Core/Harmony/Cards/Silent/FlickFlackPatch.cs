namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FlickFlackPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.FlickFlack;

    [HarmonyPatch(typeof(FlickFlack), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(FlickFlack __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(7, ValueProp.Move)
        }.AsReadOnly();
        return false;
    }
}