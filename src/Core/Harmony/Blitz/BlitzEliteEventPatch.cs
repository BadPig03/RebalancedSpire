namespace RebalancedSpire.Core.Harmony.Blitz;

using System.Reflection;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BlitzEliteEventPatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.BlitzEliteEvent;
    private static readonly FieldInfo GridBackingField = AccessTools.Field(typeof(BlitzActMap), "<Grid>k__BackingField");

    internal static bool IsEventPoint(IRunState state, MapPoint? point)
    {
        return state is { GameMode: GameMode.Blitz, Map: BlitzActMap } &&
               point is { PointType: MapPointType.Unknown, Children.Count: 1 } &&
               point.Children.Single().PointType is MapPointType.Shop or MapPointType.Elite;
    }

    [HarmonyPatch(typeof(BlitzActMap), MethodType.Constructor, [typeof(IRunState)])]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Constructor(BlitzActMap __instance, IRunState state)
    {
        if (Disabled || state.GameMode != GameMode.Blitz)
        {
            return;
        }

        var shop = __instance.GetAllMapPoints().Single(point => point.PointType == MapPointType.Shop);
        var parent = shop.parents.Single();
        var row = shop.coord.row;
        var grid = new MapPoint?[__instance.GetColumnCount(), __instance.GetRowCount() + 1];
        foreach (var point in __instance.GetAllMapPoints())
        {
            if (point.coord.row >= row)
            {
                point.coord.row++;
            }
            grid[point.coord.col, point.coord.row] = point;
        }
        __instance.BossMapPoint.coord.row++;
        if (__instance.SecondBossMapPoint is { } secondBoss)
        {
            secondBoss.coord.row++;
        }

        var eventPoint = new MapPoint(shop.coord.col, row) { PointType = MapPointType.Unknown };
        grid[eventPoint.coord.col, row] = eventPoint;
        parent.RemoveChildPoint(shop);
        parent.AddChildPoint(eventPoint);
        eventPoint.AddChildPoint(shop);
        GridBackingField.SetValue(__instance, grid);
        BlitzEventWhitelist.Apply(state.Act);
    }

    [HarmonyPatch(typeof(RunManager), "RollRoomTypeFor")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_RollRoomTypeFor(RunManager __instance, MapPointType pointType, ref RoomType __result)
    {
        if (pointType != MapPointType.Unknown || __instance.State is not { } state || !IsEventPoint(state, state.CurrentMapPoint))
        {
            return true;
        }

        __result = RoomType.Event;
        return false;
    }
}
