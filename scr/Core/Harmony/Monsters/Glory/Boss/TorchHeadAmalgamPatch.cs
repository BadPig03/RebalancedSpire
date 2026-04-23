namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Boss;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TorchHeadAmalgamPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.QueenConfig;

    [HarmonyPatch(typeof(TorchHeadAmalgam), nameof(TorchHeadAmalgam.TackleDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceTackleDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }

    [HarmonyPatch(typeof(TorchHeadAmalgam), nameof(TorchHeadAmalgam.SoulBeamDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSoulBeamDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }

    [HarmonyPatch(typeof(TorchHeadAmalgam), nameof(TorchHeadAmalgam.WeakTackleDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceWeakTackleDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}