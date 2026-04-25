namespace RebalancedSpire.scr.Core.Harmony.Cards.Event;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RelaxPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.PaelsHornConfig;

    [HarmonyPatch(typeof(Relax), nameof(Relax.CanonicalVars), MethodType.Getter)]
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
        };
        return false;
    }

    [HarmonyPatch(typeof(Relax), nameof(Relax.OnUpgrade))]
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