namespace RebalancedSpire.Core.Harmony.Cards.Regent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PillarOfCreationPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.PillarOfCreation;

    [HarmonyPatch(typeof(PillarOfCreation), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(PillarOfCreation __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(3, ValueProp.Unpowered)
        }.AsReadOnly();
        return false;
    }
}