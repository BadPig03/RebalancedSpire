namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
public static class BrilliantScarfPatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.BrilliantScarf;

    [HarmonyPatch(typeof(BrilliantScarf), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(BrilliantScarf __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(5)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not BrilliantScarf)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_BRILLIANT_SCARF.description");
        return false;
    }
}
