namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SicEmPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SicEmConfig;

    private static async Task OnPlay(SicEm instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
        {
            return;
        }

        if (!Osty.CheckMissingWithAnim(instance.Owner) && instance.Owner.Osty != null)
        {
            await DamageCmd.Attack(instance.DynamicVars.OstyDamage.BaseValue).FromOsty(instance.Owner.Osty, instance).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3").Execute(choiceContext);
        }
        await PowerCmd.Apply<SicEmPlusPower>(choiceContext, cardPlay.Target, instance.DynamicVars["SicEmPlusPower"].BaseValue, instance.Owner.Creature, instance);
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

        if (__instance is not SicEm)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-SIC_EM.description");
        return false;
    }

    [HarmonyPatch(typeof(SicEm), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(SicEm __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new OstyDamageVar(5, ValueProp.Move),
            new PowerVar<SicEmPlusPower>(2)
        }.AsReadOnly();
        return false;
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

        if (__instance is not SicEm)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Exhaust
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(SicEm), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(SicEm __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(SicEm), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(SicEm __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.OstyDamage.UpgradeValueBy(2);
        __instance.DynamicVars["SicEmPlusPower"].UpgradeValueBy(1);
        return false;
    }
}