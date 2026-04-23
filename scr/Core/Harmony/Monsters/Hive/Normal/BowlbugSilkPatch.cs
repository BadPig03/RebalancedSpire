namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BowlbugSilkPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BowlbugsConfig;

    [HarmonyPatch(typeof(BowlbugSilk), nameof(BowlbugSilk.ThrashDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceThrashDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}