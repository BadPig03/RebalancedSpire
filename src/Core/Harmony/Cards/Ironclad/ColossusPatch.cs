namespace RebalancedSpire.Core.Harmony.Cards.Ironclad;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ColossusPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Colossus;

    [HarmonyPatch(typeof(Colossus), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Colossus __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(5, ValueProp.Move),
            new("Colossus", 1)
        }.AsReadOnly();
        return false;
    }
}