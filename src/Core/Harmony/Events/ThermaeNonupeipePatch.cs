namespace RebalancedSpire.Core.Harmony.Events;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ThermaeNonupeipePatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.ThermaeNonupeipe;

    private static async Task PrivateOasis(ThermaeNonupeipe instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        await PlayerCmd.LoseGold(instance.DynamicVars["PrivateOasisCost"].BaseValue, instance.Owner);
        await RelicCmd.Obtain<BathRobes>(instance.Owner);
        instance.SetEventFinished(instance.L10NLookup("THERMAE_NONUPEIPE.pages.PRIVATE_OASIS.description"));
    }

    private static async Task DrinkWaters(ThermaeNonupeipe instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), instance.Owner.Creature, instance.DynamicVars["DrinkHpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        var curses = Enumerable.Range(0, instance.DynamicVars["DrinkRegretCount"].IntValue).Select(_ => ModelDb.Card<Regret>()).ToList();
        await CardPileCmd.AddCursesToDeck(curses, instance.Owner);
        await RelicCmd.Obtain(instance.DrinkRelic, instance.Owner);
        instance.SetEventFinished(instance.L10NLookup("THERMAE_NONUPEIPE.pages.DRINK_WATERS.description"));
    }

    [HarmonyPatch(typeof(ThermaeNonupeipe), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new GoldVar("ThermalBathsCost", 88),
            new GoldVar("PrivateOasisCost", 150),
            new HpLossVar("DrinkHpLoss", 31),
            new CardsVar("DrinkRegretCount", 1),
            new StringVar("BathTowelDescription"),
            new StringVar("BathRobesDescription"),
            new StringVar("DrinkRelicName")
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(ThermaeNonupeipe), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(ThermaeNonupeipe __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        var hoverTips = HoverTipFactory.FromRelic(__instance.DrinkRelic).ToList();
        hoverTips.Add(HoverTipFactory.FromCard<Regret>());
        __result = new List<EventOption>
        {
            new(__instance, __instance.ThermalBaths, "THERMAE_NONUPEIPE.pages.INITIAL.options.THERMAL_BATHS"),
            new(__instance, () => PrivateOasis(__instance), "REBALANCED_SPIRE_EVENT_THERMAE_NONUPEIPE.pages.INITIAL.options.PRIVATE_OASIS"),
            new EventOption(__instance, () => DrinkWaters(__instance), "REBALANCED_SPIRE_EVENT_THERMAE_NONUPEIPE.pages.INITIAL.options.DRINK_WATERS", hoverTips).ThatDoesDamage(__instance.DynamicVars["DrinkHpLoss"].BaseValue)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(ThermaeNonupeipe), "IsAllowed")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_IsAllowed(IRunState runState, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = runState.Players.All(p => p.Gold >= 150 && p.Creature.CurrentHp <= p.Creature.MaxHp * 0.75m) && runState.Act.Ancient is not Nonupeipe;
        return false;
    }
}
