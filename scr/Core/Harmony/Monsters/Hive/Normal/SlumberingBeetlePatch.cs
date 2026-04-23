namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SlumberingBeetlePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SlumberingBeetleConfig;

    [HarmonyPatch(typeof(SlumberingBeetle), nameof(SlumberingBeetle.RolloutDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceRolloutDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }
}