namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class HauntedShipPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.HauntedShipConfig;

    [HarmonyPatch(typeof(HauntedShip), nameof(HauntedShip.HauntDazed), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceHauntDazed(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }
}