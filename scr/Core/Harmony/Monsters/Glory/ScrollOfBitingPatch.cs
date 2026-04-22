namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class ScrollOfBitingPatch
{
    private static int PaperCutsPowerAmount => 1;

    private static async Task AfterAddedToRoom(ScrollOfBiting instance)
    {
        await PowerCmd.Apply<PaperCutsPower>(instance.Creature, PaperCutsPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(ScrollOfBiting), nameof(ScrollOfBiting.ChompDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceChompDamage(ref int __result)
    {
        __result -= 2;
    }

    [HarmonyPatch(typeof(ScrollOfBiting), nameof(ScrollOfBiting.ChewDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceChewDamage(ref int __result)
    {
        __result -= 1;
    }

    [HarmonyPatch(typeof(ScrollOfBiting), nameof(ScrollOfBiting.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(ScrollOfBiting __instance, ref Task __result)
    {
        __result = AfterAddedToRoom(__instance);
        return false;
    }
}