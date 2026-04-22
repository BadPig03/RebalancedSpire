namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class LouseProgenitorPatch
{
    private static int StrengthPowerAMount => 4;

    private static async Task CurlAndGrowMove(LouseProgenitor instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/giant_louse/giant_louse_curl");
        await CreatureCmd.TriggerAnim(instance.Creature, "Curl", 0.25f);
        await CreatureCmd.GainBlock(instance.Creature, instance.CurlBlock, ValueProp.Move, null);
        await PowerCmd.Apply<StrengthPower>(instance.Creature, StrengthPowerAMount, instance.Creature, null);
        instance.Curled = true;
    }

    [HarmonyPatch(typeof(LouseProgenitor), nameof(LouseProgenitor.CurlAndGrowMove))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CurlAndGrowMove(LouseProgenitor __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        __result = CurlAndGrowMove(__instance);
        return false;
    }
}