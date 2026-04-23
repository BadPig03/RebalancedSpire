namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Boss;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class QueenPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.QueenConfig;

    [HarmonyPatch(typeof(Queen), nameof(Queen.OffWithYourHeadDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceOffWithYourHeadDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }

    [HarmonyPatch(typeof(Queen), nameof(Queen.ExecutionDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceExecutionDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }
}