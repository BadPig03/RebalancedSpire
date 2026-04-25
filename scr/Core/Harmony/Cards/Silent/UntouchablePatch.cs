namespace RebalancedSpire.scr.Core.Harmony.Cards.Silent;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class UntouchablePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.UntouchableConfig;

    [HarmonyPatch(typeof(Untouchable), nameof(Untouchable.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Untouchable __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(7, ValueProp.Move)
        };
        return false;
    }

    [HarmonyPatch(typeof(Untouchable), nameof(Untouchable.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Untouchable __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Block.UpgradeValueBy(2);
        return false;
    }
}