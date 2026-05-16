namespace RebalancedSpire.Core.Harmony.Cards.Defect;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DefragmentPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DefragmentConfig;

    [HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Defragment)
        {
            return true;
        }

        __result = CardRarity.Uncommon;
        return false;
    }
}