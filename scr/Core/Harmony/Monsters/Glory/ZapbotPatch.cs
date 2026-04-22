namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class ZapbotPatch
{
    [HarmonyPatch(typeof(Zapbot), nameof(Zapbot.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        __result = 18;
    }

    [HarmonyPatch(typeof(Zapbot), nameof(Zapbot.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(ref int __result)
    {
        __result = 18;
    }

    [HarmonyPatch(typeof(Zapbot), nameof(Zapbot.ZapDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceZapDamage(ref int __result)
    {
        __result -= 2;
    }
}