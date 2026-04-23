namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FlyconidPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FlyconidConfig;

    [HarmonyPatch(typeof(Flyconid), nameof(Flyconid.SmashDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSmashDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}