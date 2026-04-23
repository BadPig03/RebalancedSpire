namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class OvicopterPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.OvicopterConfig;

    [HarmonyPatch(typeof(Ovicopter), nameof(Ovicopter.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result += 15;
    }

    [HarmonyPatch(typeof(Ovicopter), nameof(Ovicopter.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMaxInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result += 15;
    }
}