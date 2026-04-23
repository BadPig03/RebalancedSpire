namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ExoskeletonPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ExoskeletonConfig;

    private static int HardToKillPowerAmount => 13;

    private static async Task AfterAddedToRoom(Exoskeleton instance)
    {
        await PowerCmd.Apply<HardToKillPower>(instance.Creature, HardToKillPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(Exoskeleton), nameof(Exoskeleton.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(Exoskeleton __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }
}