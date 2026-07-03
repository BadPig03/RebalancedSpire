namespace RebalancedSpire.Core.Harmony.Cards.Token;

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

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FuelPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Fuel;

    private static async Task OnPlay(Fuel instance, PlayerChoiceContext choiceContext)
    {
        await PlayerCmd.GainEnergy(instance.DynamicVars.Energy.BaseValue, instance.Owner);
        await CardPileCmd.Draw(choiceContext, instance.DynamicVars.Cards.BaseValue, instance.Owner);
    }

    [HarmonyPatch(typeof(Fuel), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Fuel __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new EnergyVar(1),
            new CardsVar(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(CardModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Fuel)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_FUEL.description");
        return false;
    }

    [HarmonyPatch(typeof(Fuel), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Fuel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(Fuel), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Fuel __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Cards.UpgradeValueBy(1);
        return false;
    }
}