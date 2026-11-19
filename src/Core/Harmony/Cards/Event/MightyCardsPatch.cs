namespace RebalancedSpire.Core.Harmony.Cards.Event;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MightyCardsPatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.ShrineOfTheWarlord;

    private static async Task OnPlay(MightyWeapon instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = instance.CombatState;
        if (combatState == null)
        {
            return;
        }

        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance, cardPlay).TargetingAllOpponents(combatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        for (var i = 0; i < instance.DynamicVars.Cards.IntValue; i++)
        {
            var card = combatState.CreateCard<Dazed>(instance.Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, instance.Owner, CardPilePosition.Random));
        }
    }

    private static async Task OnPlay(MightyShield instance, CardPlay cardPlay)
    {
        var combatState = instance.CombatState;
        if (combatState == null)
        {
            return;
        }

        await CreatureCmd.GainBlock(instance.Owner.Creature, instance.DynamicVars.Block, cardPlay);
        for (var i = 0; i < instance.DynamicVars.Cards.IntValue; i++)
        {
            var card = combatState.CreateCard<Dazed>(instance.Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, instance.Owner, CardPilePosition.Random));
        }
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

        if (__instance is not (MightyWeapon or MightyShield))
        {
            return true;
        }

        __result = new LocString("cards", __instance is MightyWeapon ? "REBALANCED_SPIRE_CARD_MIGHTY_WEAPON.description" : "REBALANCED_SPIRE_CARD_MIGHTY_SHIELD.description");
        return false;
    }

    [HarmonyPatch(typeof(MightyWeapon), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(MightyWeapon __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(MightyShield), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(MightyShield __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, cardPlay);
        return false;
    }
}
