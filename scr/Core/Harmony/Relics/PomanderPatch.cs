namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PomanderPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.PomanderConfig;

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Pomander)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-POMANDER.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.EventDescription), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Pomander)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-POMANDER.eventDescription");
        return false;
    }

    [HarmonyPatch(typeof(Pomander), nameof(Pomander.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Pomander __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(2)
        };
        return false;
    }
}