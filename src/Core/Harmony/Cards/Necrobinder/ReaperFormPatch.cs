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

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class ReaperFormPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.ReaperForm;

    private static async Task OnPlay(ReaperForm instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "PowerUp", instance.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<ReaperFormPlusPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["ReaperFormPlusPower"].BaseValue, instance.Owner.Creature, instance);
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(CardModel __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ReaperForm)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<ReaperFormPlusPower>(1)
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

        if (__instance is not ReaperForm)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_REAPER_FORM.description");
        return false;
    }

    [HarmonyPatch(typeof(ReaperForm), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(ReaperForm __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }
}