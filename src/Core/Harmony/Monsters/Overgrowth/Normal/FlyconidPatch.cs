namespace RebalancedSpire.Core.Harmony.Monsters.Overgrowth.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FlyconidPatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.Flyconid;

    [HarmonyPatch(typeof(Flyconid), "SmashDamage", MethodType.Getter)]
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