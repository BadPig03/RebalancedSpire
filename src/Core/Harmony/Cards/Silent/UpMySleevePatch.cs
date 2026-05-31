namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class UpMySleevePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.UpMySleeve;

    private static async Task OnPlay(UpMySleeve instance)
    {
        var combatState = instance.CombatState;
        if (combatState == null)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        for (var i = 0; i < instance.DynamicVars.Cards.IntValue; i++)
        {
            await Shiv.CreateInHand(instance.Owner, combatState);
            await Cmd.Wait(0.1f);
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

        if (__instance is not UpMySleeve)
        {
            return true;
        }

        __result = new List<CardKeyword>
        {
            CardKeyword.Sly
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

        if (__instance is not UpMySleeve)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_UP_MY_SLEEVE.description");
        return false;
    }

    [HarmonyPatch(typeof(UpMySleeve), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(UpMySleeve __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance);
        return false;
    }
}