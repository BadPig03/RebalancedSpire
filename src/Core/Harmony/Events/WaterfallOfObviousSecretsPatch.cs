namespace RebalancedSpire.Core.Harmony.Events;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WaterfallOfObviousSecretsPatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.WaterfallOfObviousSecrets;

    private static async Task BeatUp(WaterfallOfObviousSecrets instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        RelicModel relic = RelicFactory.PullNextRelicFromFront(instance.Owner, RelicRarity.Rare).ToMutable();
        await RelicCmd.Obtain(relic, instance.Owner);
        await CardPileCmd.AddCursesToDeck(Enumerable.Repeat(ModelDb.Card<PoorSleep>(), instance.DynamicVars["BeatUpCurseCount"].IntValue), instance.Owner);
        instance.SetEventFinished(instance.L10NLookup("WATERFALL_OF_OBVIOUS_SECRETS.pages.BEAT_UP.description"));
    }

    [HarmonyPatch(typeof(WaterfallOfObviousSecrets), "CanonicalVars", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_CanonicalVars(ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return;
        }

        var variables = __result.ToList();
        variables.Single(v => v.Name == "BeatUpCurseCount").BaseValue = 1;
        __result = variables;
    }

    [HarmonyPatch(typeof(EventModel), "SetEventState")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void PreFix_SetEventState(EventModel __instance, ref IEnumerable<EventOption> eventOptions)
    {
        if (__instance is not WaterfallOfObviousSecrets waterfall || Disabled)
        {
            return;
        }

        eventOptions = eventOptions.Select(option => option.TextKey == "WATERFALL_OF_OBVIOUS_SECRETS.pages.ENTER.options.BEAT_UP" ? new EventOption(waterfall, waterfall.BeatUp, "REBALANCED_SPIRE_EVENT_WATERFALL_OF_OBVIOUS_SECRETS.pages.ENTER.options.BEAT_UP", HoverTipFactory.FromCardWithCardHoverTips<PoorSleep>()) : option).ToList();
    }

    [HarmonyPatch(typeof(WaterfallOfObviousSecrets), "BeatUp")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BeatUp(WaterfallOfObviousSecrets __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = BeatUp(__instance);
        return false;
    }
}
