namespace RebalancedSpire.scr.Core.Harmony.Cards.Defect;

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

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SpinnerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SpinnerConfig;

    private static async Task OnPlay(Spinner instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SpinnerPlusPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["SpinnerPlusPower"].BaseValue, instance.Owner.Creature, instance);
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

        if (__instance is not Spinner)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-SPINNER.description");
        return false;
    }

    [HarmonyPatch(typeof(Spinner), nameof(Spinner.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Spinner __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<SpinnerPlusPower>(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Spinner), nameof(Spinner.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Spinner __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(CardModel __instance)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Spinner)
        {
            return true;
        }

        __instance.DynamicVars["SpinnerPlusPower"].UpgradeValueBy(1);
        return false;
    }
}