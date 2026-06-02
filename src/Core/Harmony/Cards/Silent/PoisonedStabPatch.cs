namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using Core.Enchantments;
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
public static class PoisonedStabPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.PoisonedStab;

    private static async Task OnPlay(PoisonedStab instance)
    {
        var combatState = instance.CombatState;
        if (combatState == null)
        {
            return;
        }

        var shivs = (await Shiv.CreateInHand(instance.Owner, instance.DynamicVars.Cards.IntValue, combatState)).ToList();
        foreach (var shiv in shivs)
        {
            CardCmd.Enchant<Poisonous>(shiv, 1);
            if (!instance.IsUpgraded)
            {
                continue;
            }

            CardCmd.Upgrade(shiv);
        }
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not PoisonedStab)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Exhaust
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(PoisonedStab), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(PoisonedStab __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(2)
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

        if (__instance is not PoisonedStab)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_POISONED_STAB.description");
        return false;
    }

    [HarmonyPatch(typeof(PoisonedStab), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(PoisonedStab __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        var shiv = (Shiv) ModelDb.Card<Shiv>().MutableClone();
        CardCmd.Enchant<Poisonous>(shiv, 1);
        if (__instance.IsUpgraded)
        {
            CardCmd.Upgrade(shiv);
        }
        var list = new List<IHoverTip>
        {
            HoverTipFactory.FromCard(shiv)
        };
        list.AddRange(HoverTipFactory.FromEnchantment<Poisonous>());
        __result = list.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(PoisonedStab), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(PoisonedStab __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance);
        return false;
    }

    [HarmonyPatch(typeof(PoisonedStab), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(PoisonedStab __instance)
    {
        return Disabled;
    }

    [HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TargetType(CardModel __instance, ref TargetType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not PoisonedStab)
        {
            return true;
        }

        __result = TargetType.Self;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Type(CardModel __instance, ref CardType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not PoisonedStab)
        {
            return true;
        }

        __result = CardType.Skill;
        return false;
    }

}