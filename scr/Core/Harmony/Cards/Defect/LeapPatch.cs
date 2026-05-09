namespace RebalancedSpire.scr.Core.Harmony.Cards.Defect;

using Core.Powers;
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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LeapPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.LeapConfig;

    private static async Task OnPlay(Leap instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(instance.Owner.Creature, instance.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<LeapPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["FocusPower"].IntValue, instance.Owner.Creature, instance);
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Rarity), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Leap)
        {
            return true;
        }

        __result = CardRarity.Uncommon;
        return false;
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

        if (__instance is not Leap)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-LEAP.description");
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.ExtraHoverTips), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Leap)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromPower<FocusPower>()
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Leap), nameof(Leap.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Leap __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(9, ValueProp.Move),
            new PowerVar<FocusPower>(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Leap), nameof(Leap.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Leap __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(Leap), nameof(Leap.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Leap __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Block.UpgradeValueBy(2);
        __instance.DynamicVars["FocusPower"].UpgradeValueBy(1);
        return false;
    }
}