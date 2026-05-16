namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

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
public static class SpurPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SpurConfig;

    private static async Task OnPlay(Spur instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        var amount = instance.DynamicVars.Heal.BaseValue;
        if (instance.Owner is { IsOstyAlive: true, Osty: not null })
        {
            var healAmount = Math.Min(amount, instance.Owner.Osty.MaxHp - instance.Owner.Osty.CurrentHp);
            if (healAmount > 0)
            {
                await CreatureCmd.Heal(instance.Owner.Osty, healAmount);
                amount -= healAmount;
            }
        }
        if (amount <= 0)
        {
            return;
        }

        await OstyCmd.Summon(choiceContext, instance.Owner, amount, instance);
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

        if (__instance is not Spur)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-SPUR.description");
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Spur)
        {
            return true;
        }

        __result = CardRarity.Common;
        return false;
    }

    [HarmonyPatch(typeof(Spur), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(Spur __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.Static(StaticHoverTip.SummonStatic)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Spur), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Spur __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new HealVar(7)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Spur), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Spur __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>().AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Spur), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Spur __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(Spur), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Spur __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Heal.UpgradeValueBy(2);
        return false;
    }
}