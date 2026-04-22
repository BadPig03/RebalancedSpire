namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class ToadpolePatch
{
    [HarmonyPatch(typeof(Toadpole), nameof(Toadpole.SpikeSpitDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSpikeSpitDamage(ref int __result)
    {
        __result -= 1;
    }

    [HarmonyPatch(typeof(Toadpole), nameof(Toadpole.WhirlDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceWhirlDamage(ref int __result)
    {
        __result -= 1;
    }
}