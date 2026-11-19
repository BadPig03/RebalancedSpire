namespace RebalancedSpire.Core.Harmony.Blitz;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NBlitzTopBarMapPointPatch
{
    [HarmonyPatch(typeof(NHoverTipSet), "CreateAndShow", [typeof(Control), typeof(IHoverTip), typeof(HoverTipAlignment)])]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void PreFix_CreateAndShow(Control owner, ref IHoverTip hoverTip)
    {
        if (owner is not NBlitzTopBarMapPoint point || !BlitzEliteEventPatch.IsEventPoint(point._runState, point.Point))
        {
            return;
        }

        hoverTip = new HoverTip(new LocString("map", "REBALANCED_SPIRE_BLITZ_EVENT.hoverTip.title"),
            new LocString("map", "REBALANCED_SPIRE_BLITZ_EVENT.hoverTip.description"));
    }

    [HarmonyPatch(typeof(NBlitzTopBarMapPoint), "UpdateIcon")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void PreFix_UpdateIcon(NBlitzTopBarMapPoint __instance)
    {
        if (__instance.Point.PointType != MapPointType.Unknown)
        {
            return;
        }

        var icon = NNormalMapPoint.IconName(MapPointType.Unknown);
        __instance._icon.Texture = ResourceLoader.Load<Texture2D>(NNormalMapPoint.IconPath(icon));
        __instance._outline.Texture = ResourceLoader.Load<Texture2D>(NNormalMapPoint.OutlinePath(icon));
    }
}
