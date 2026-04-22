namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class OvicopterPatch
{
    [HarmonyPatch(typeof(Ovicopter), nameof(Ovicopter.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        __result += 15;
    }

    [HarmonyPatch(typeof(Ovicopter), nameof(Ovicopter.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMaxInitialHp(ref int __result)
    {
        __result += 15;
    }
}