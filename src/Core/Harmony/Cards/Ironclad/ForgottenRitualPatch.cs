namespace RebalancedSpire.Core.Harmony.Cards.Ironclad;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ForgottenRitualPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.ForgottenRitual;

    private static async Task OnPlay(ForgottenRitual instance)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(instance.Owner.Creature, VfxColor.Purple));
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        if (instance.WasCardExhaustedThisTurn)
        {
            await PlayerCmd.GainEnergy(instance.DynamicVars.Energy.IntValue, instance.Owner);
        }
        instance.EnergyCost.AddThisCombat(1);
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

        if (__instance is not ForgottenRitual)
        {
            return true;
        }

        __result = 0;
        return false;
    }

    [HarmonyPatch(typeof(ForgottenRitual), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(ForgottenRitual __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>().AsReadOnly();
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

        if (__instance is not ForgottenRitual)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_FORGOTTEN_RITUAL.description");
        return false;
    }

    [HarmonyPatch(typeof(ForgottenRitual), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(ForgottenRitual __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            __instance.EnergyHoverTip
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(ForgottenRitual), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(ForgottenRitual __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance);
        return false;
    }
}