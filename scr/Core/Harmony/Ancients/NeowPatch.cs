namespace RebalancedSpire.scr.Core.Harmony.Ancients;

using Core.Relics;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryNeow)]
// ReSharper disable InconsistentNaming
public static class NeowPatch
{
    private static List<EventOption> CurseOptions(Neow instance) =>
    [
        Helpers.RelicOption<CursedPearl>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        Helpers.RelicOption<HeftyTablet>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        Helpers.RelicOption<LargeCapsule>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        Helpers.RelicOption<LeafyPoultice>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        Helpers.RelicOption<NeowsBones>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        Helpers.RelicOption<PrecariousShears>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        Helpers.RelicOption<ScrollBoxes>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description"),
        Helpers.RelicOption<SilverCrucible>(instance, customDonePage: "NEOW.pages.DONE.CURSED.description")
    ];

    private static List<EventOption> PositiveOptions(Neow instance) =>
    [
        Helpers.RelicOption<ArcaneScroll>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<BoomingConch>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<GoldenPearl>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<LavaRock>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<LeadPaperweight>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<LostCoffer>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<MassiveScroll>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<NeowsTalisman>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<NeowsTorment>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<NeowsLament>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<NewLeaf>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<NutritiousOyster>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<Pomander>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<PreciseScissors>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<PhialHolster>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<StoneHumidifier>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<SmallCapsule>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description"),
        Helpers.RelicOption<WingedBoots>(instance, customDonePage: "NEOW.pages.DONE.POSITIVE.description")
    ];

    [HarmonyPatch(typeof(Neow), nameof(Neow.AllPossibleOptions), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AllPossibleOptions(Neow __instance, ref IEnumerable<EventOption> __result)
    {
        List<EventOption> options = [];
        options.AddRange(CurseOptions(__instance));
        options.AddRange(PositiveOptions(__instance));
        __result = options;
        return false;
    }

    [HarmonyPatch(typeof(Neow), nameof(Neow.GenerateInitialOptions))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(Neow __instance, ref IReadOnlyList<EventOption> __result)
    {
        Player? player = __instance.Owner;
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