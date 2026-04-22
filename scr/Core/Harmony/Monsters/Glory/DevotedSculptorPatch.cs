namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class DevotedSculptorPatch
{
    [HarmonyPatch(typeof(DevotedSculptor), nameof(DevotedSculptor.SavageDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSavageDamage(ref int __result)
    {
        __result -= 4;
    }
}