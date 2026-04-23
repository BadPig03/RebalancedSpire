namespace RebalancedSpire.scr.Core.Harmony.Cards.Event;

using System.Collections.ObjectModel;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
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
public static class NeowsFuryPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.NeowConfig;

    private static async Task OnPlay(NeowsFury instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
        {
            return;
        }

        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance).Targeting(target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        var cards = (await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Discard.GetPile(instance.Owner).Cards, instance.Owner, new CardSelectorPrefs(instance.SelectionScreenPrompt, instance.DynamicVars.Cards.IntValue))).ToList();
        if (cards.Count == 0)
        {
            return;
        }

        await CardPileCmd.Add(cards, PileType.Hand);
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

        if (__instance is not NeowsFury)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-NEOWS_FURY.description");
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.SelectionScreenPrompt), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_SelectionScreenPrompt(CardModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not NeowsFury)
        {
            return true;
        }

        LocString str = new LocString("cards", "REBALANCEDSPIRE-NEOWS_FURY.selectionScreenPrompt");
        __instance.DynamicVars.AddTo(str);
        __result = str;
        return false;
    }

    [HarmonyPatch(typeof(NeowsFury), nameof(NeowsFury.CanonicalVars), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_CanonicalVars(NeowsFury __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = new ReadOnlyCollection<DynamicVar>([
            new DamageVar(10, ValueProp.Move),
            new CardsVar(1)
        ]);
    }

    [HarmonyPatch(typeof(NeowsFury), nameof(NeowsFury.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(NeowsFury __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(NeowsFury), nameof(NeowsFury.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(NeowsFury __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Damage.UpgradeValueBy(4);
        __instance.DynamicVars.Cards.UpgradeValueBy(1);
        return false;
    }
}