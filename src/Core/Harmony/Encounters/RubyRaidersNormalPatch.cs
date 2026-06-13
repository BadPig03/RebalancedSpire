namespace RebalancedSpire.Core.Harmony.Encounters;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RubyRaidersNormalPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.RubyRaiders;

    [HarmonyPatch(typeof(RubyRaidersNormal), "GenerateMonsters")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMonsters(RubyRaidersNormal __instance, ref IReadOnlyList<(MonsterModel, string?)> __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterModel> currentRaiders;
        List<(MonsterModel, string?)> result;
        do
        {
            currentRaiders = [];
            result = [];
            for (var i = 0; i < 3; i++)
            {
                var items = RubyRaidersNormal._raiderValidCounts.Keys.Where(r => currentRaiders.Count(c => c == r) < RubyRaidersNormal._raiderValidCounts[r]).ToList();
                var monsterModel = __instance.Rng.NextItem(items);
                if (monsterModel == null)
                {
                    continue;
                }

                currentRaiders.Add(monsterModel);
                result.Add((monsterModel.ToMutable(), null));
            }
        }
        while (currentRaiders.Any(r => r is AxeRubyRaider) && currentRaiders.Any(r => r is AssassinRubyRaider) && currentRaiders.Any(r => r is BruteRubyRaider));
        __result = result.AsReadOnly();
        return false;
    }
}