namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class WellLaidPlansPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.WellLaidPlans;

    [HarmonyPatch(typeof(WellLaidPlans), "MultiplayerConstraint", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_MultiplayerConstraint(WellLaidPlans __instance, ref CardMultiplayerConstraint __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = CardMultiplayerConstraint.None;
        return false;
    }
}