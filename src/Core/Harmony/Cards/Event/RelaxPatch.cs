namespace RebalancedSpire.Core.Harmony.Cards.Event;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RelaxPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.PaelsHorn;

    [HarmonyPatch(typeof(Relax), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Relax __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(20, ValueProp.Move),
            new CardsVar(2),
            new EnergyVar(3)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Relax), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Relax __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Block.UpgradeValueBy(3);
        __instance.DynamicVars.Cards.UpgradeValueBy(1);
        return false;
    }
}