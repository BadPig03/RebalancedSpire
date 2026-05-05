namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class MysteriousKnightPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheLanternKeyConfig;

    private static async Task AfterAddedToRoom(MysteriousKnight instance)
    {
        await PowerCmd.Apply<StrengthPower>(instance.Creature, 3, instance.Creature, null);
        await PowerCmd.Apply<PlatingPower>(instance.Creature, 3, instance.Creature, null);
    }

    [HarmonyPatch(typeof(MysteriousKnight), nameof(MysteriousKnight.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(MysteriousKnight __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }
}