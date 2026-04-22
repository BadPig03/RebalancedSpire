namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class QueenPatch
{
    [HarmonyPatch(typeof(Queen), nameof(Queen.OffWithYourHeadDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceOffWithYourHeadDamage(ref int __result)
    {
        __result -= 1;
    }

    [HarmonyPatch(typeof(Queen), nameof(Queen.ExecutionDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceExecutionDamage(ref int __result)
    {
        __result -= 2;
    }
}