namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class TorchHeadAmalgamPatch
{
    [HarmonyPatch(typeof(TorchHeadAmalgam), nameof(TorchHeadAmalgam.TackleDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceTackleDamage(ref int __result)
    {
        __result -= 1;
    }

    [HarmonyPatch(typeof(TorchHeadAmalgam), nameof(TorchHeadAmalgam.SoulBeamDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSoulBeamDamage(ref int __result)
    {
        __result -= 2;
    }

    [HarmonyPatch(typeof(TorchHeadAmalgam), nameof(TorchHeadAmalgam.WeakTackleDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceWeakTackleDamage(ref int __result)
    {
        __result -= 1;
    }
}