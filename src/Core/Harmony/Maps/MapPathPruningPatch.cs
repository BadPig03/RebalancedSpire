namespace RebalancedSpire.Core.Harmony.Maps;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Map;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MapPathPruningPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.MapGenerationConfig;

    [HarmonyPatch(typeof(MapPathPruning), "PruneAndRepair")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Postfix_PruneAndRepair(ActMap map, Func<MapPointType, MapPoint, bool> isValidPointType)
    {
        if (Disabled)
        {
            return;
        }

        StandardActMapPatch.BreakLongMonsterRuns(map, point => isValidPointType(MapPointType.Unknown, point));
    }
}