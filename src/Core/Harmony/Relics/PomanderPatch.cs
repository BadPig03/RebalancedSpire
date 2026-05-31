namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PomanderPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Pomander;

    private static async Task AfterObtained(Pomander instance)
    {
        foreach (var card in (await CardSelectCmd.FromDeckForUpgrade(prefs: new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, instance.DynamicVars.Cards.IntValue), player: instance.Owner)).ToList())
        {
            CardCmd.Upgrade(card);
        }
        foreach (var card in PileType.Deck.GetPile(instance.Owner).Cards.Where(c => c.IsUpgradable).ToList().StableShuffle(instance.Owner.RunState.Rng.Niche).Take(instance.DynamicVars.Cards.IntValue))
        {
            CardCmd.Upgrade(card);
        }
    }

    [HarmonyPatch(typeof(Pomander), "AfterObtained")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(Pomander __instance, ref Task __result)
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

        if (__instance is not Pomander)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_POMANDER.description");
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

        if (__instance is not Pomander)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_POMANDER.eventDescription");
        return false;
    }
}