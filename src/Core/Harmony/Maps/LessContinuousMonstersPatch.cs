namespace RebalancedSpire.Core.Harmony.Maps;

using System.Reflection;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Random;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LessContinuousMonstersPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.LessContinuousMonsters;
    private static readonly FieldInfo NumOfUnknownsBackingField = AccessTools.Field(typeof(MapPointTypeCounts), "<NumOfUnknowns>k__BackingField");

    private const int MaxConsecutiveMonsters = 4;
    private const int MaxMonsterBreakerPasses = 64;

    private static int BreakLongMonsterRuns(ActMap map, Func<MapPoint, bool> isValidUnknownPlacement)
    {
        var addedUnknowns = 0;
        for (var pass = 0; pass < MaxMonsterBreakerPasses; pass++)
        {
            var run = FindFirstMonsterRunLongerThan(map.StartingMapPoint, [], MaxConsecutiveMonsters);
            if (run == null)
            {
                return addedUnknowns;
            }

            var breaker = ChooseUnknownBreaker(run, isValidUnknownPlacement);
            if (breaker == null)
            {
                return addedUnknowns;
            }

            breaker.PointType = MapPointType.Unknown;
            addedUnknowns++;
        }

        return addedUnknowns;
    }

    private static List<MapPoint>? FindFirstMonsterRunLongerThan(MapPoint point, List<MapPoint> currentRun, int maxConsecutiveMonsters)
    {
        List<MapPoint> nextRun;
        if (point.PointType == MapPointType.Monster)
        {
            nextRun = new List<MapPoint>(currentRun.Count + 1);
            nextRun.AddRange(currentRun);
            nextRun.Add(point);
        }
        else
        {
            nextRun = [];
        }

        return nextRun.Count > maxConsecutiveMonsters ? nextRun : point.Children.OrderBy(child => child.coord.row).ThenBy(child => child.coord.col).Select(child => FindFirstMonsterRunLongerThan(child, nextRun, maxConsecutiveMonsters)).OfType<List<MapPoint>>().FirstOrDefault();
    }

    private static MapPoint? ChooseUnknownBreaker(List<MapPoint> monsterRun, Func<MapPoint, bool> isValidUnknownPlacement)
    {
        var preferredIndex = Math.Min(MaxConsecutiveMonsters, monsterRun.Count - 1);
        var candidates = monsterRun.Select((point, index) => new
        {
            Point = point,
            Index = index,
            Distance = Math.Abs(index - preferredIndex)
        }).OrderBy(x => x.Distance).ThenBy(x => x.Point.coord.row).ThenBy(x => x.Point.coord.col).ToList();
        var validBreaker = candidates.Where(x => x.Point.CanBeModified).Select(x => x.Point).FirstOrDefault(isValidUnknownPlacement);
        return validBreaker ?? candidates.Where(x => x.Point.CanBeModified).Select(x => x.Point).FirstOrDefault();
    }

    private static void AddUnknownsToPointTypeCounts(MapPointTypeCounts pointTypeCounts, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        NumOfUnknownsBackingField.SetValue(pointTypeCounts, pointTypeCounts.NumOfUnknowns + amount);
    }

    private static void AddUnknownsToPointTypeCounts(StandardActMap map, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        AddUnknownsToPointTypeCounts(map._pointTypeCounts, amount);
    }

    [HarmonyPatch(typeof(StandardActMap), "AssignPointTypes")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_AssignPointTypes(StandardActMap __instance)
    {
        if (Disabled)
        {
            return;
        }

        var addedUnknowns = BreakLongMonsterRuns(__instance, point => __instance.IsValidPointType(MapPointType.Unknown, point));
        AddUnknownsToPointTypeCounts(__instance, addedUnknowns);
    }

    [HarmonyPatch(typeof(MapPathPruning), "PruneAndRepair")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Postfix_PruneAndRepair(MapPoint?[,] grid, HashSet<MapPoint> startMapPoints, ActMap map, MapPointTypeCounts pointTypeCounts, Rng rng, Func<MapPointType, MapPoint, bool> isValidPointType)
    {
        if (Disabled)
        {
            return;
        }

        var addedUnknowns = BreakLongMonsterRuns(map, point => isValidPointType(MapPointType.Unknown, point));
        AddUnknownsToPointTypeCounts(pointTypeCounts, addedUnknowns);
    }
}