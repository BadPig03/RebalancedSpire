namespace RebalancedSpire.Core.Harmony.Ancients;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DarvPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DarvChoicesConfig;

    [HarmonyPatch(typeof(Darv), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(Darv __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance.Owner == null)
        {
            return true;
        }

        var list = Darv._validRelicSets.Where(s => s.filter(__instance.Owner)).Select(s => SrcHelpers.RelicOption(__instance, __instance.Rng.NextItem(s.relics)!.ToMutable())).ToList().UnstableShuffle(__instance.Rng).Take(2).ToList();
        list.Add(SrcHelpers.RelicOption<DustyTome>(__instance));
        __result = list.AsReadOnly();
        return false;
    }
}