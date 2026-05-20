namespace RebalancedSpire.Core.Harmony.Maps;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Random;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MapPathPruningPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.MapGenerationConfig;

    [HarmonyPatch(typeof(MapPathPruning), "PruneAndRepair")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Postfix_PruneAndRepair(MapPoint?[,] grid, HashSet<MapPoint> startMapPoints, ActMap map, MapPointTypeCounts pointTypeCounts, Rng rng, Func<MapPointType, MapPoint, bool> isValidPointType)
    {
        if (Disabled)
        {
            return;
        }

        var addedUnknowns = StandardActMapPatch.BreakLongMonsterRuns(map, point => isValidPointType(MapPointType.Unknown, point));
        StandardActMapPatch.AddUnknownsToPointTypeCounts(pointTypeCounts, addedUnknowns);
    }
}