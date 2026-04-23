namespace RebalancedSpire.scr.Core.Harmony.Relics;

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
    private static readonly bool Disabled = !RebalancedSpireConfig.TezcataraConfig;

    private static async Task AfterPlayerTurnStart(ToastyMittens instance, PlayerChoiceContext choiceContext, Player player)
    {
        foreach (CardModel card in await CardSelectCmd.FromSimpleGrid(choiceContext, (from c in PileType.Draw.GetPile(player).Cards where !c.Keywords.Contains(CardKeyword.Unplayable) orderby c.Rarity, c.Id select c).ToList(), player, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, instance.DynamicVars.Cards.IntValue)))
        {
            instance.Flash();
            await CardCmd.Exhaust(choiceContext, card);
            await PowerCmd.Apply<StrengthPower>(player.Creature, instance.DynamicVars.Strength.BaseValue, player.Creature, null);
        }
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

        if (__instance is not ToastyMittens)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-TOASTY_MITTENS.description");
        return false;
    }

    [HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.CanonicalVars), MethodType.Getter)]
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
            new CardsVar(1)
        };
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterPlayerTurnStart))]
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

    [HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]
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
}