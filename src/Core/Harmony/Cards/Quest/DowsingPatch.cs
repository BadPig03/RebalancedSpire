namespace RebalancedSpire.Core.Harmony.Cards.Quest;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DowsingPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DowsingRod;

    [HarmonyPatch(typeof(Dowsing), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Dowsing __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("Rooms", 4)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "AfterCreated")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCreated(CardModel __instance)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Dowsing dowsing)
        {
            return true;
        }

        dowsing.RoomsEntered++;
        return false;
    }
}