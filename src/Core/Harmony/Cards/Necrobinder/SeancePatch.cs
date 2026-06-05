namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SeancePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Seance;

    private static async Task OnPlay(Seance instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        var cards = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(instance.Owner), instance.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, instance.DynamicVars.Cards.IntValue))).ToList();
        foreach (var card in cards)
        {
            var soul = instance.CombatState?.CreateCard<Soul>(instance.Owner);
            if (soul == null)
            {
                continue;
            }

            if (instance.IsUpgraded)
            {
                CardCmd.Upgrade(soul);
            }
            await CardCmd.Transform(card, soul);
        }
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

        if (__instance is not Seance)
        {
            return true;
        }

        __result = 0;
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

        if (__instance is not Seance)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_SEANCE.description");
        return false;
    }

    [HarmonyPatch(typeof(Seance), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(Seance __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromCard<Soul>(__instance.IsUpgraded)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Seance), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Seance __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(Seance), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Seance __instance)
    {
        return Disabled;
    }
}