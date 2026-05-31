namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TheLostPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.TheLostAndForgotten;

    [HarmonyPatch(typeof(TheLost), "MinInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 11;
    }
}