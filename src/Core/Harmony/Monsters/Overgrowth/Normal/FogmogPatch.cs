namespace RebalancedSpire.Core.Harmony.Monsters.Overgrowth.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FogmogPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FogmogConfig;

    [HarmonyPatch(typeof(Fogmog), "HeadbuttDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceHeadbuttDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}