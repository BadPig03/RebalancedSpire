namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LargeCapsulePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.LargeCapsuleConfig;

    private static async Task AfterObtained(LargeCapsule instance)
    {
        List<Reward> rewards =
        [
            new RelicReward(RelicFactory.PullNextRelicFromFront(instance.Owner, RelicRarity.Uncommon).ToMutable(), instance.Owner),
            new RelicReward(RelicFactory.PullNextRelicFromFront(instance.Owner, RelicRarity.Rare).ToMutable(), instance.Owner)
        ];
        await new RewardsSet(instance.Owner).WithCustomRewards(rewards).WithSkippingDisallowed().Offer();
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(instance.Owner.RunState.CreateCard(ModelDb.Card<Writhe>(), instance.Owner), PileType.Deck), 2f);
        await Cmd.Wait(0.75f);
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LargeCapsule)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-LARGE_CAPSULE.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.EventDescription), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LargeCapsule)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-LARGE_CAPSULE.eventDescription");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.ExtraHoverTips), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(RelicModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LargeCapsule)
        {
            return true;
        }

        __result = HoverTipFactory.FromCardWithCardHoverTips<Writhe>();
        return false;
    }

    [HarmonyPatch(typeof(LargeCapsule), nameof(LargeCapsule.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(LargeCapsule __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new IntVar("Relics", 1),
            new StringVar("Writhe", ModelDb.Card<Writhe>().Title)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(LargeCapsule), nameof(LargeCapsule.AfterObtained))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(LargeCapsule __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterObtained(__instance);
        return false;
    }
}