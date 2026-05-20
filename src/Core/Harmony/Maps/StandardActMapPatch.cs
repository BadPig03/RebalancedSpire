namespace RebalancedSpire.Core.Harmony.Maps;

using System.Reflection;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Map;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class StandardActMapPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.MapGenerationConfig;
    private static readonly PropertyInfo GridProperty = AccessTools.Property(typeof(StandardActMap), "Grid");
    private static readonly FieldInfo NumOfUnknownsBackingField = AccessTools.Field(typeof(MapPointTypeCounts), "<NumOfUnknowns>k__BackingField");
    private static readonly int[][] _startColumnBuckets =
    [
        [0, 1],
        [2, 3, 4],
        [5, 6]
    ];
    private static readonly int[] _startBucketPlan =
    [
        0, 1, 2, 0, 1, 2, 1
    ];

    private const int MaxConsecutiveMonsters = 4;
    private const int MaxMonsterBreakerPasses = 64;

    public static int BreakLongMonsterRuns(ActMap map, Func<MapPoint, bool> isValidUnknownPlacement)
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

    public static void AddUnknownsToPointTypeCounts(MapPointTypeCounts pointTypeCounts, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        NumOfUnknownsBackingField.SetValue(pointTypeCounts, pointTypeCounts.NumOfUnknowns + amount);
    }

    private static List<int> GenerateBucketedStartColumns(StandardActMap instance)
    {
        var bucketPlan = _startBucketPlan.ToList();
        bucketPlan.StableShuffle(instance._rng);
        var columns = new List<int>(StandardActMap._iterations);
        columns.AddRange(bucketPlan.Select(bucketIndex => _startColumnBuckets[bucketIndex]).Select(bucket => bucket[instance._rng.NextInt(0, bucket.Length)]));
        return columns;
    }

    private static void AddUnknownsToPointTypeCounts(StandardActMap map, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        AddUnknownsToPointTypeCounts(map._pointTypeCounts, amount);
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

    [HarmonyPatch(typeof(StandardActMap), "GenerateMap")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMap(StandardActMap __instance)
    {
        if (Disabled)
        {
            return true;
        }

        var grid = (MapPoint?[,]?) GridProperty.GetValue(__instance);
        if (grid == null)
        {
            return true;
        }

        foreach (var point in GenerateBucketedStartColumns(__instance).Select(s => __instance.GetOrCreatePoint(s, 1)))
        {
            __instance.startMapPoints.Add(point);
            __instance.PathGenerate(point);
        }
        StandardActMap.ForEachInRow(grid, __instance.GetRowCount() - 1, x => x.AddChildPoint(__instance.BossMapPoint));
        if (__instance.SecondBossMapPoint != null)
        {
            __instance.BossMapPoint.AddChildPoint(__instance.SecondBossMapPoint);
        }
        StandardActMap.ForEachInRow(grid, 1, x => __instance.StartingMapPoint.AddChildPoint(x));
        return false;
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
}