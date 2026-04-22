namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;

//[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class TheInsatiablePatch
{
    private static int LongDistancePowerAmount => 5;

    private static async Task LiquifyMove(TheInsatiable instance, IReadOnlyList<Creature> targets)
    {
        foreach (Creature player in targets)
        {
            await PowerCmd.Apply<LongDistancePower>(player, LongDistancePowerAmount, instance.Creature, null);
        }
    }

    [HarmonyPatch(typeof(TheInsatiable), nameof(TheInsatiable.BiteDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceBiteDamage(ref int __result)
    {
        __result -= 3;
    }

    [HarmonyPatch(typeof(TheInsatiable), nameof(TheInsatiable.LiquifyMove))]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Postfix_LiquifyMove(TheInsatiable __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        __result = LiquifyMove(__instance, targets);
    }
}