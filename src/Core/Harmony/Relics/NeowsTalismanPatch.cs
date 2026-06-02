namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NeowsTalismanPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.NeowsTalisman;

    private static Task AfterObtained(NeowsTalisman instance)
    {
        var source = PileType.Deck.GetPile(instance.Owner).Cards.Where(c => c.Rarity == CardRarity.Basic).ToList();
        var basicCards = new List<CardModel?>([
            source.FirstOrDefault(c => c.Tags.Contains(CardTag.Strike)),
            source.FirstOrDefault(c => c.Tags.Contains(CardTag.Defend))
        ]).OfType<CardModel>().ToList();
        foreach (var card in basicCards)
        {
            CardCmd.Upgrade(card);
        }
		var randomCards = PileType.Deck.GetPile(instance.Owner).Cards.Where(c => c.IsUpgradable).ToList().StableShuffle(instance.Owner.RunState.Rng.Niche).Take(1).ToList();
        foreach (var card in randomCards)
        {
            CardCmd.Upgrade(card);
        }
        return Task.CompletedTask;
    }

    [HarmonyPatch(typeof(NeowsTalisman), "AfterObtained")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(NeowsTalisman __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterObtained(__instance);
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not NeowsTalisman)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_NEOWS_TALISMAN.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "EventDescription", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not NeowsTalisman)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_NEOWS_TALISMAN.eventDescription");
        return false;
    }

}