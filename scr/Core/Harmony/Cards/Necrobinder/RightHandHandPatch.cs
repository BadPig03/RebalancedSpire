namespace RebalancedSpire.scr.Core.Harmony.Cards.Necrobinder;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RightHandHandPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.RightHandHandConfig;

    private static async Task AfterCardPlayedLate(RightHandHand instance, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != instance.Owner || cardPlay.Resources.EnergyValue < instance.DynamicVars["Required"].IntValue)
        {
            return;
        }

        var pile = instance.Pile;
        if (pile != null && pile.Type != PileType.Hand)
        {
            await CardPileCmd.Add(instance, PileType.Hand);
        }
        instance.EnergyCost.AddThisCombat(-1);
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(CardModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not RightHandHand)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-RIGHT_HAND_HAND.description");
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.CanonicalEnergyCost), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not RightHandHand)
        {
            return true;
        }

        __result = 2;
        return false;
    }

    [HarmonyPatch(typeof(RightHandHand), nameof(RightHandHand.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(RightHandHand __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new OstyDamageVar(11, ValueProp.Move),
            new EnergyVar("Required", 2),
            new EnergyVar("Energy", 1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(RightHandHand), nameof(RightHandHand.AfterCardPlayedLate))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCardPlayedLate(RightHandHand __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterCardPlayedLate(__instance, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(RightHandHand), nameof(RightHandHand.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(RightHandHand __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.OstyDamage.UpgradeValueBy(3);
        return false;
    }
}