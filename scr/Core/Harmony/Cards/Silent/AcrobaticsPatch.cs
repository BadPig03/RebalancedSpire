namespace RebalancedSpire.scr.Core.Harmony.Cards.Silent;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class AcrobaticsPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.AcrobaticsConfig;

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Rarity), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Acrobatics)
        {
            return true;
        }

        __result = CardRarity.Common;
        return false;
    }
}