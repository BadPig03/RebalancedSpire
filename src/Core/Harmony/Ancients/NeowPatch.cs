namespace RebalancedSpire.Core.Harmony.Ancients;

using Configs;
using Core.Relics;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NeowPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.NeowChoicesConfig;

    private static List<EventOption> CurseOptions(Neow instance) =>
    [
        SrcHelpers.RelicOption<CursedPearl>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        SrcHelpers.RelicOption<HeftyTablet>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        SrcHelpers.RelicOption<LargeCapsule>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        SrcHelpers.RelicOption<LeafyPoultice>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        SrcHelpers.RelicOption<NeowsBones>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        SrcHelpers.RelicOption<PrecariousShears>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        SrcHelpers.RelicOption<ScrollBoxes>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        SrcHelpers.RelicOption<SilverCrucible>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description")
    ];

    private static List<EventOption> PositiveOptions(Neow instance) =>
    [
        SrcHelpers.RelicOption<ArcaneScroll>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<BoomingConch>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<FishingRod>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<GoldenPearl>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<Kaleidoscope>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<LavaRock>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<LeadPaperweight>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<LostCoffer>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<MassiveScroll>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<NeowsLament>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<NeowsTalisman>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<NeowsTorment>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<NewLeaf>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<NutritiousOyster>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<PhialHolster>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<Pomander>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<PreciseScissors>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<SilkenTress>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<SmallCapsule>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<StoneHumidifier>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        SrcHelpers.RelicOption<WingedBoots>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description")
    ];

    [HarmonyPatch(typeof(Neow), "AllPossibleOptions", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AllPossibleOptions(Neow __instance, ref IEnumerable<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<EventOption> options = [];
        options.AddRange(CurseOptions(__instance));
        options.AddRange(PositiveOptions(__instance));
        __result = options.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Neow), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(Neow __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        var player = __instance.Owner;
        if (player == null || player.RunState.Modifiers.Count > 0)
        {
            return true;
        }

        var curses = CurseOptions(__instance);
        curses.RemoveAll(option => option.Relic != null && !option.Relic.IsAllowed(player.RunState));
        var chosenCurse = __instance.Rng.NextItem(curses);
        if (chosenCurse == null)
        {
            return true;
        }

        var positives = PositiveOptions(__instance);
        switch (chosenCurse.Relic)
        {
            case CursedPearl:
                positives.RemoveAll(option => option.Relic is GoldenPearl);
                break;
            case HeftyTablet:
                positives.RemoveAll(option => option.Relic is ArcaneScroll);
                break;
            case LeafyPoultice:
                positives.RemoveAll(option => option.Relic is NewLeaf);
                break;
            case PrecariousShears:
                positives.RemoveAll(option => option.Relic is PreciseScissors);
                break;
            case LargeCapsule:
                positives.RemoveAll(option => option.Relic is SmallCapsule);
                break;
        }
        positives.RemoveAll(option => option.Relic != null && !option.Relic.IsAllowed(player.RunState));
        if (__instance.Rng.NextBool())
        {
            positives.RemoveAll(option => option.Relic is NutritiousOyster);
        }
        else
        {
            positives.RemoveAll(option => option.Relic is StoneHumidifier);
        }
        if (__instance.Rng.NextBool())
        {
            positives.RemoveAll(option => option.Relic is NeowsTalisman);
        }
        else
        {
            positives.RemoveAll(option => option.Relic is Pomander);
        }
        if (__instance.Rng.NextBool())
        {
            positives.RemoveAll(option => option.Relic is WingedBoots);
        }
        else
        {
            positives.RemoveAll(option => option.Relic is NeowsLament);
        }

        var results = positives.UnstableShuffle(__instance.Rng).Take(2).ToList();
        results.Add(chosenCurse);
        __result = results.AsReadOnly();
        return false;
    }
}