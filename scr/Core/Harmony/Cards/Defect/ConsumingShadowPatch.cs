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
public static class ConsumingShadowPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.ConsumingShadowConfig;

    private static async Task OnPlay(ConsumingShadow instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ConsumingShadowPlusPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["ConsumingShadowPlusPower"].IntValue, instance.Owner.Creature, instance);
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

        if (__instance is not ConsumingShadow)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-CONSUMING_SHADOW.description");
        return false;
    }

    [HarmonyPatch(typeof(ConsumingShadow), nameof(ConsumingShadow.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(ConsumingShadow __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<ConsumingShadowPlusPower>(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.CanonicalEnergyCost), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ConsumingShadow)
        {
            return true;
        }

        __result = 1;
        return false;
    }

    [HarmonyPatch(typeof(ConsumingShadow), nameof(ConsumingShadow.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(ConsumingShadow __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars["ConsumingShadowPlusPower"].UpgradeValueBy(1);
        return false;
    }

    [HarmonyPatch(typeof(ConsumingShadow), nameof(ConsumingShadow.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(ConsumingShadow __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }
}