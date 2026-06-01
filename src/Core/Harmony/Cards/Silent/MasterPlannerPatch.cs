namespace RebalancedSpire.Core.Harmony.Cards.Silent;

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
public static class MasterPlannerPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.MasterPlanner;

    private static async Task OnPlay(MasterPlanner instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<MasterPlannerPlusPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars.Cards.BaseValue, instance.Owner.Creature, instance);
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

        if (__instance is not MasterPlanner)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(1)
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

        if (__instance is not MasterPlanner)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_MASTER_PLANNER.description");
        return false;
    }

    [HarmonyPatch(typeof(MasterPlanner), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(MasterPlanner __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(MasterPlanner), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(MasterPlanner __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Cards.UpgradeValueBy(1);
        return false;
    }
}