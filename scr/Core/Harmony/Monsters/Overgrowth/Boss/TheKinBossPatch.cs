namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Boss;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TheKinBossPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheKinConfig;

    [HarmonyPatch(typeof(TheKinBoss), nameof(TheKinBoss.GenerateMonsters))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMonsters(TheKinBoss __instance, ref IReadOnlyList<(MonsterModel, string?)> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<(MonsterModel, string?)>(
        [
            (ModelDb.Monster<KinPriest>().ToMutable(), "leaderSlot")
        ]).AsReadOnly();
        return false;
    }
}