namespace RebalancedSpire.Core.Harmony.Cards.Defect;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
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
public static class ShatterPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Shatter;

    private static async Task OnPlay(Shatter instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (instance.CombatState == null)
        {
            return;
        }

        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance, cardPlay).TargetingAllOpponents(instance.CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        var orbCount = instance.Owner.PlayerCombatState?.OrbQueue.Orbs.Count;
        for (var i = 0; i < orbCount; i++)
        {
            if (instance.IsUpgraded) {
                await OrbCmd.EvokeNext(choiceContext, instance.Owner, dequeue: false);
            }
            await OrbCmd.EvokeNext(choiceContext, instance.Owner);
        }
    }

    [HarmonyPatch(typeof(Shatter), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Shatter __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>().AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Shatter), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Shatter __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(9, ValueProp.Move)
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

        if (__instance is not Shatter)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_SHATTER.description");
        return false;
    }

    [HarmonyPatch(typeof(Shatter), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Shatter __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(Shatter), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Shatter __instance)
    {
        return Disabled;
    }
}