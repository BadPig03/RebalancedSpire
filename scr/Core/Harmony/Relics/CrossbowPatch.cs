namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CrossbowPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.CrossbowConfig;

    private static async Task AfterSideTurnStart(Crossbow instance)
    {
        var cardModels = (from c in instance.Owner.Character.CardPool.GetUnlockedCards(instance.Owner.UnlockState, instance.Owner.RunState.CardMultiplayerConstraint) where c.Type == CardType.Attack select c).ToList();
        if (cardModels.Count == 0)
        {
            return;
        }

        instance.Flash();
        var chosenCards = CardFactory.GetDistinctForCombat(instance.Owner, cardModels, 1, instance.Owner.RunState.Rng.CombatCardGeneration).ToList();
        foreach (CardModel cardModel in chosenCards)
        {
            cardModel.SetToFreeThisCombat();
            CardCmd.ApplyKeyword(cardModel, CardKeyword.Exhaust);
        }
        await CardPileCmd.AddGeneratedCardsToCombat(chosenCards, PileType.Hand, instance.Owner);
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

        if (__instance is not Crossbow)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-CROSSBOW.description");
        return false;
    }

    [HarmonyPatch(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterSideTurnStart(Crossbow __instance, CombatSide side, ICombatState combatState, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (side != __instance.Owner.Creature.Side)
        {
            return true;
        }

        __result = AfterSideTurnStart(__instance);
        return false;
    }
}