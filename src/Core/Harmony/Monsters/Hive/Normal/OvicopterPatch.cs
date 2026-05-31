namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class OvicopterPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Ovicopter;

    [HarmonyPatch(typeof(Ovicopter), "MaxInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMaxInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result += 20;
    }

    [HarmonyPatch(typeof(Ovicopter), "MinInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result += 20;
    }
}