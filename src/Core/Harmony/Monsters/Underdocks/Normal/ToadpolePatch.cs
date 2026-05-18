namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ToadpolePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ToadpoleConfig;

    [HarmonyPatch(typeof(Toadpole), "SpikeSpitDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSpikeSpitDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }

    [HarmonyPatch(typeof(Toadpole), "WhirlDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceWhirlDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}