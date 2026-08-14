namespace RebalancedSpire.Core.Harmony.Cards.Ironclad;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ExpectAFightPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.ExpectAFight;

    private static async Task OnPlay(ExpectAFight instance, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainEnergy(((CalculatedVar) instance.DynamicVars["CalculatedEnergy"]).Calculate(cardPlay.Target), instance.Owner);
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ExpectAFight)
        {
            return true;
        }

        __result = 2;
        return false;
    }

    [HarmonyPatch(typeof(ExpectAFight), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(ExpectAFight __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new EnergyVar(0),
            new CalculationBaseVar(0),
            new CalculationExtraVar(1),
            new CalculatedVar("CalculatedEnergy").WithMultiplier((card, _) => PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.Type == CardType.Attack))
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

        if (__instance is not ExpectAFight)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_EXPECT_A_FIGHT.description");
        return false;
    }

    [HarmonyPatch(typeof(ExpectAFight), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(ExpectAFight __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            __instance.EnergyHoverTip
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(ExpectAFight), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(ExpectAFight __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(ExpectAFight), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(ExpectAFight __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }

    [HarmonyPatch(typeof(ExpectAFight), "GainsBlock", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GainsBlock(ExpectAFight __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = false;
        return false;
    }
}