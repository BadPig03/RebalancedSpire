namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class GlobeHeadPatch
{
    private static int GalvanicPowerAmount => 3;

    [HarmonyPatch(typeof(GlobeHead), nameof(GlobeHead.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(GlobeHead __instance, ref Task __result)
    {
        __result = AfterAddedToRoom(__instance);
        return false;
    }

    private static async Task AfterAddedToRoom(GlobeHead instance)
    {
        await PowerCmd.Apply<GalvanicPower>(instance.Creature, GalvanicPowerAmount, instance.Creature, null);
    }
}