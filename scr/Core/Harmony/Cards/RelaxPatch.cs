namespace RebalancedSpire.scr.Core.Harmony.Cards;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryPael)]
// ReSharper disable InconsistentNaming
public static class RelaxPatch
{
    [HarmonyPatch(typeof(Relax), nameof(Relax.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Relax __instance, ref IEnumerable<DynamicVar> __result)
    {
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
        __instance.DynamicVars.Block.UpgradeValueBy(3);
        __instance.DynamicVars.Cards.UpgradeValueBy(1);
        return false;
    }
}