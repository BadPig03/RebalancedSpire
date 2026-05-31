namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Boss;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class QueenPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Queen;

    [HarmonyPatch(typeof(Queen), "ExecutionDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceExecutionDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}