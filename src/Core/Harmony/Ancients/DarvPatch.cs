namespace RebalancedSpire.Core.Harmony.Ancients;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DarvPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DarvAncientChoices;

    [HarmonyPatch(typeof(Darv), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(Darv __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        var player = __instance.Owner;
        if (player == null)
        {
            return true;
        }

        var options = __instance.AllowedOptions(Darv._validRelicSets.Where(s => s.filter(player)).Select(s => SrcHelpers.RelicOption(__instance, __instance.Rng.NextItem(s.relics)!.ToMutable()))).UnstableShuffle(__instance.Rng);
        var dustyTome = (DustyTome) ModelDb.Relic<DustyTome>().ToMutable();
        var canOfferDustyTome = dustyTome.IsAllowed(player.RunState);
        var list = options.Take(canOfferDustyTome ? 2 : 3).ToList();
        if (canOfferDustyTome)
        {
            dustyTome.SetupForPlayer(player);
            list.Add(SrcHelpers.RelicOption(__instance, dustyTome));
        }
        __result = list.AsReadOnly();
        return false;
    }
}
