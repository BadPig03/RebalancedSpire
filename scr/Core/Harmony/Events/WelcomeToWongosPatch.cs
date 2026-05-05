namespace RebalancedSpire.scr.Core.Harmony.Events;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WelcomeToWongosPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.WelcomeToWongosConfig;

    private static async Task Leave(WelcomeToWongos instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), instance.Owner.Creature, instance.DynamicVars.Damage, null, null);
        instance.SetEventFinished(instance.L10NLookup("REBALANCEDSPIRE-WELCOME_TO_WONGOS.pages.LEAVE.description"));
    }

    [HarmonyPatch(typeof(WelcomeToWongos), nameof(WelcomeToWongos.GenerateInitialOptions))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(WelcomeToWongos __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        var owner = __instance.Owner;
        if (owner == null)
        {
            return true;
        }

        __instance.FeaturedItem = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare, r => r.IsAllowedInShops);
        ((StringVar) __instance.DynamicVars["RandomRelic"]).StringValue = __instance.FeaturedItem.Title.GetFormattedText();
        __result = new List<EventOption>
        {
            owner.Gold >= __instance.DynamicVars["BargainBinCost"].BaseValue ? new EventOption(__instance, __instance.BuyBargainBin, "WELCOME_TO_WONGOS.pages.INITIAL.options.BARGAIN_BIN") : new EventOption(__instance, null, "WELCOME_TO_WONGOS.pages.INITIAL.options.BARGAIN_BIN_LOCKED"),
            owner.Gold >= __instance.DynamicVars["FeaturedItemCost"].BaseValue ? new EventOption(__instance, __instance.BuyFeaturedItem, "WELCOME_TO_WONGOS.pages.INITIAL.options.FEATURED_ITEM", __instance.FeaturedItem.HoverTips) : new EventOption(__instance, null, "WELCOME_TO_WONGOS.pages.INITIAL.options.FEATURED_ITEM_LOCKED"),
            owner.Gold >= __instance.DynamicVars["MysteryBoxCost"].BaseValue ? new EventOption(__instance, __instance.BuyMysteryBox, "WELCOME_TO_WONGOS.pages.INITIAL.options.MYSTERY_BOX") : new EventOption(__instance, null, "WELCOME_TO_WONGOS.pages.INITIAL.options.MYSTERY_BOX_LOCKED"),
            new EventOption(__instance, () => Leave(__instance), "REBALANCEDSPIRE-WELCOME_TO_WONGOS.pages.INITIAL.options.LEAVE").ThatDoesDamage(__instance.DynamicVars.Damage.BaseValue)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(WelcomeToWongos), nameof(WelcomeToWongos.IsAllowed))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_IsAllowed(WelcomeToWongos __instance, IRunState runState, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = runState.CurrentActIndex == 1 && runState.Players.All(p => p is { Gold: >= 100, Creature.CurrentHp: >= 5 });
        return false;
    }

    [HarmonyPatch(typeof(WelcomeToWongos), nameof(WelcomeToWongos.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(WelcomeToWongos __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("BargainBinCost", 100),
            new("FeaturedItemCost", 200),
            new("MysteryBoxCost", 300),
            new("MysteryBoxRelicCount", 3),
            new("MysteryBoxCombatCount", 5),
            new("WongoPointAmount", 0),
            new("RemainingWongoPointAmount", 0),
            new("TotalWongoBadgeAmount", 0),
            new DamageVar(3, ValueProp.Unblockable | ValueProp.Unpowered),
            new StringVar("RandomRelic")
        }.AsReadOnly();
        return false;
    }
}