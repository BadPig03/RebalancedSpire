namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BigMushroomPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.HungryForMushrooms;

    [HarmonyPatch(typeof(BigMushroom), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(BigMushroom __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new MaxHpVar(25),
            new CardsVar(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "EventDescription", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not BigMushroom)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_BIG_MUSHROOM.eventDescription");
        return false;
    }
}