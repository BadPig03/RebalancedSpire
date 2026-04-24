namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

//[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class AxebotPatch
{
    //private static readonly bool Disabled = !RebalancedSpireConfig.AxebotConfig;
    private static readonly bool Disabled = true;

    [HarmonyPatch(typeof(Axebot), nameof(Axebot.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result += 21;
    }

    [HarmonyPatch(typeof(Axebot), nameof(Axebot.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMaxInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result += 21;
    }

    [HarmonyPatch(typeof(Axebot), nameof(Axebot.StockAmount), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceStockAmount(Axebot __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = __instance._stockOverrideAmount ?? 1;
    }
}