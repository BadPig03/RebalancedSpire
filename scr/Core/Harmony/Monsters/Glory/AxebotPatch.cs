namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using Afflictions;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class AxebotPatch
{
    [HarmonyPatch(typeof(Axebot), nameof(Axebot.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        __result += 21;
    }

    [HarmonyPatch(typeof(Axebot), nameof(Axebot.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMaxInitialHp(ref int __result)
    {
        __result += 21;
    }

    [HarmonyPatch(typeof(Axebot), nameof(Axebot.StockAmount), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceStockAmount(Axebot __instance, ref int __result)
    {
        __result = __instance._stockOverrideAmount ?? 1;
    }
}