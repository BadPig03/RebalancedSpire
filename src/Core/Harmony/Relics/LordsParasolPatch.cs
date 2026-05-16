namespace RebalancedSpire.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LordsParasolPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.LordsParasolConfig;

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LordsParasol)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-LORDS_PARASOL.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(RelicModel __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LordsParasol)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new EnergyVar(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "ModifyMaxEnergy")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ModifyMaxEnergy(AbstractModel __instance, Player player, decimal amount, ref decimal __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LordsParasol lordsParasol || player != lordsParasol.Owner)
        {
            return true;
        }

        __result = amount + lordsParasol.DynamicVars.Energy.IntValue;
        return false;
    }
}