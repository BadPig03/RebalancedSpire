namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class GremlinMercPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.GremlinMercConfig;

    [HarmonyPatch(typeof(GremlinMerc), "DoubleSmashDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceDoubleSmashDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }
}