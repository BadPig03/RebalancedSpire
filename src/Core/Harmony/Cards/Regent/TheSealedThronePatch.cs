namespace RebalancedSpire.Core.Harmony.Cards.Regent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class TheSealedThronePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.TheSealedThrone;

    [HarmonyPatch(typeof(TheSealedThrone), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(TheSealedThrone __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.AddKeyword(CardKeyword.Innate);
        return false;
    }
}