namespace RebalancedSpire.Core.Harmony.Maps;

using System.Reflection;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Map;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class UniformIntroGenerationPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.UniformIntroGeneration;
    private static readonly PropertyInfo GridProperty = AccessTools.Property(typeof(StandardActMap), "Grid");
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

    private static List<int> GenerateBucketedStartColumns(StandardActMap instance)
    {
        var bucketPlan = _startBucketPlan.ToList();
        bucketPlan.StableShuffle(instance._rng);
        var columns = new List<int>(StandardActMap._iterations);
        columns.AddRange(bucketPlan.Select(bucketIndex => _startColumnBuckets[bucketIndex]).Select(bucket => bucket[instance._rng.NextInt(0, bucket.Length)]));
        return columns;
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

		var points = GenerateBucketedStartColumns(__instance).Select(s => __instance.GetOrCreatePoint(s, 1));
        foreach (var point in points)
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
}