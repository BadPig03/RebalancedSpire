namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class OwlMagistratePatch
{
    [HarmonyPatch(typeof(OwlMagistrate), nameof(OwlMagistrate.ScrutinyDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceScrutinyDamage(OwlMagistrate __instance, ref int __result)
    {
        __result -= 6;
    }

    [HarmonyPatch(typeof(OwlMagistrate), nameof(OwlMagistrate.VerdictDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceVerdictDamage(OwlMagistrate __instance, ref int __result)
    {
        __result -= 6;
    }
}