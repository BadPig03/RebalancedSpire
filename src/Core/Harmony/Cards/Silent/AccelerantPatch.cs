namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class AccelerantPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Accelerant;

    [HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Accelerant)
        {
            return true;
        }

        __result = CardRarity.Rare;
        return false;
    }
}