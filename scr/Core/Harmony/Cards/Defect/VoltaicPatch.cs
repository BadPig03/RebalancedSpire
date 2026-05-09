namespace RebalancedSpire.scr.Core.Harmony.Cards.Defect;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class VoltaicPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.VoltaicConfig;

    [HarmonyPatch(typeof(Voltaic), nameof(Voltaic.CanonicalKeywords), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Voltaic __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>().AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Voltaic), nameof(Voltaic.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Voltaic __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}