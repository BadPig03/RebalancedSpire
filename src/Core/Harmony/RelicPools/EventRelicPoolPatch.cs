namespace RebalancedSpire.Core.Harmony.RelicPools;

using Configs;
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

    private static List<RelicModel> RelicsToRemove =>
    [
        ModelDb.Relic<BoneTea>(),
        ModelDb.Relic<EmberTea>(),
        ModelDb.Relic<TeaOfDiscourtesy>()
    ];

    [HarmonyPatch(typeof(EventRelicPool), "GenerateAllRelics")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_GenerateAllRelics(EventRelicPool __instance, ref IEnumerable<RelicModel> __result)
    {
        if (Disabled)
        {
            return;
        }

        var list = new List<RelicModel>(__result);
        foreach (var relic in RelicsToRemove)
        {
            list.Remove(relic);
        }
        __result = list.AsReadOnly();
    }
}