namespace RebalancedSpire.Core.Harmony.Events;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Events;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SpiralingWhirlpoolPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SpiralingWhirlpoolConfig;

    private static async Task ReachIn(SpiralingWhirlpool instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        RelicModel relic = RelicFactory.PullNextRelicFromFront(instance.Owner).ToMutable();
        await RelicCmd.Obtain(relic, instance.Owner);
        await CardPileCmd.AddCurseToDeck<Injury>(instance.Owner);
        instance.SetEventFinished(instance.L10NLookup("REBALANCEDSPIRE-SPIRALING_WHIRLPOOL.pages.REACH_IN.description"));
    }

    [HarmonyPatch(typeof(SpiralingWhirlpool), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(SpiralingWhirlpool __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            new (__instance, __instance.ObserveTheSpiral, "SPIRALING_WHIRLPOOL.pages.INITIAL.options.OBSERVE", HoverTipFactory.FromEnchantment<Spiral>()),
            new (__instance, __instance.Drink, "SPIRALING_WHIRLPOOL.pages.INITIAL.options.DRINK"),
            new (__instance, () => ReachIn(__instance), "REBALANCEDSPIRE-SPIRALING_WHIRLPOOL.pages.INITIAL.options.REACH_IN", HoverTipFactory.FromCardWithCardHoverTips<Injury>())
        }.AsReadOnly();
        return false;
    }
}