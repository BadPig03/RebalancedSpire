namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DistinguishedCapePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DistinguishedCape;

    private static async Task AfterObtained(DistinguishedCape instance)
    {
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), instance.Owner.Creature, instance.DynamicVars.HpLoss.BaseValue, isFromCard: false);
        var results = new List<CardPileAddResult>();
        for (var i = 0; i < instance.DynamicVars.Cards.IntValue; i++)
        {
            CardModel card = instance.Owner.RunState.CreateCard<Apparition>(instance.Owner);
            results.Add(await CardPileCmd.Add(card, PileType.Deck));
        }
        CardCmd.PreviewCardPileAdd(results, 2f);
    }

    [HarmonyPatch(typeof(DistinguishedCape), "AfterObtained")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(DistinguishedCape __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterObtained(__instance);
        return false;
    }

    [HarmonyPatch(typeof(DistinguishedCape), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(DistinguishedCape __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new HpLossVar(9),
            new CardsVar(3)
        }.AsReadOnly();
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

        if (__instance is not DistinguishedCape)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_DISTINGUISHED_CAPE.description");
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

        if (__instance is not DistinguishedCape)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_DISTINGUISHED_CAPE.eventDescription");
        return false;
    }
}