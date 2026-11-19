namespace RebalancedSpire.Core.Harmony.Events;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MuseumOfTorturePatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.MuseumOfTorture;

    private static async Task TakeRelic(MuseumOfTorture instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        await RelicCmd.Obtain(instance.OfferedRelic.ToMutable(), instance.Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(instance.Owner.RunState.CreateCard<Injury>(instance.Owner), PileType.Deck));
        instance.SetEventFinished(instance.L10NLookup("MUSEUM_OF_TORTURE.pages.TAKE.description"));
    }

    private static async Task BreakOut(MuseumOfTorture instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), instance.Owner.Creature,
            instance.DynamicVars["BreakOutHpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        instance.SetEventFinished(instance.L10NLookup("MUSEUM_OF_TORTURE.pages.BREAK_OUT.description"));
    }

    [HarmonyPatch(typeof(MuseumOfTorture), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar> { new HpLossVar("BreakOutHpLoss", 8) }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(EventModel), "SetEventState")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void PreFix_SetEventState(EventModel __instance, ref IEnumerable<EventOption> eventOptions)
    {
        if (__instance is not MuseumOfTorture museum || Disabled)
        {
            return;
        }

        eventOptions = eventOptions.Select(option =>
        {
            if (option.TextKey == "MUSEUM_OF_TORTURE.pages.DISPLAY.options.TAKE")
            {
                var take = new EventOption(museum, museum.TakeRelic, "REBALANCED_SPIRE_EVENT_MUSEUM_OF_TORTURE.pages.DISPLAY.options.TAKE",
                    option.HoverTips.Concat(HoverTipFactory.FromCardWithCardHoverTips<Injury>()));
                take.Description.Add("Relic", museum.OfferedRelic.Title.GetFormattedText());
                return take;
            }

            return option.TextKey == "MUSEUM_OF_TORTURE.pages.DISPLAY.options.BREAK_OUT"
                ? new EventOption(museum, museum.BreakOut, "REBALANCED_SPIRE_EVENT_MUSEUM_OF_TORTURE.pages.DISPLAY.options.BREAK_OUT")
                    .ThatDoesDamage(museum.DynamicVars["BreakOutHpLoss"].BaseValue)
                : option;
        }).ToList();
    }

    [HarmonyPatch(typeof(MuseumOfTorture), "TakeRelic")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TakeRelic(MuseumOfTorture __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = TakeRelic(__instance);
        return false;
    }

    [HarmonyPatch(typeof(MuseumOfTorture), "BreakOut")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BreakOut(MuseumOfTorture __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = BreakOut(__instance);
        return false;
    }
}
