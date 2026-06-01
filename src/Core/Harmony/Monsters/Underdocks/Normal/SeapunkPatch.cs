namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SeapunkPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Seapunk;

    [HarmonyPatch(typeof(Seapunk), "SeaKickDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSeaKickDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }
}