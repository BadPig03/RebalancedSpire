namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BowlbugRockPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BowlbugsConfig;

    [HarmonyPatch(typeof(BowlbugRock), "HeadbuttDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceHeadbuttDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }
}