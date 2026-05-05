namespace RebalancedSpire.scr.Core.Harmony;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class EventRelicPoolPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TeaMasterConfig;

    private static readonly List<RelicModel> _relicsToRemove =
    [
        ModelDb.Relic<BoneTea>(),
        ModelDb.Relic<EmberTea>(),
        ModelDb.Relic<TeaOfDiscourtesy>()
    ];

    [HarmonyPatch(typeof(EventRelicPool), nameof(EventRelicPool.GenerateAllRelics))]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_GenerateAllRelics(EventRelicPool __instance, ref IEnumerable<RelicModel> __result)
    {
        if (Disabled)
        {
            return;
        }

        var list = new List<RelicModel>(__result);
        foreach (var relic in _relicsToRemove)
        {
            list.Remove(relic);
        }
        __result = list.AsReadOnly();
    }
}