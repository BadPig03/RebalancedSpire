namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ToastyMittensPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.ToastyMittens;

    private static async Task AfterPlayerTurnStart(ToastyMittens instance, PlayerChoiceContext choiceContext, Player player)
    {
        instance.Flash();
        var cards = (await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Draw.GetPile(player).Cards.TakeLast(instance.DynamicVars["AllCards"].IntValue).ToList(), player, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, instance.DynamicVars["ChooseCards"].IntValue))).ToList();
        foreach (var card in cards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, instance.DynamicVars.Strength.BaseValue, player.Creature, null);
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterPlayerTurnStart")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterPlayerTurnStart(AbstractModel __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ToastyMittens toastyMittens || player != toastyMittens.Owner)
        {
            return true;
        }

        __result = AfterPlayerTurnStart(toastyMittens, choiceContext, player);
        return false;
    }

    [HarmonyPatch(typeof(ToastyMittens), "BeforeHandDraw")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BeforeHandDraw(ToastyMittens __instance, Player player, PlayerChoiceContext choiceContext, CombatState combatState, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(ToastyMittens), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(ToastyMittens __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<StrengthPower>(1),
            new CardsVar("ChooseCards", 1),
            new CardsVar("AllCards", 5)
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

        if (__instance is not ToastyMittens)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_TOASTY_MITTENS.description");
        return false;
    }

}