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
public static class InfiniteBladesPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.InfiniteBlades;

    private const int InfiniteBladesPlusPowerAmount = 1;

    private static async Task OnPlay(InfiniteBlades instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        var power = await PowerCmd.Apply<InfiniteBladesPlusPower>(choiceContext, instance.Owner.Creature, InfiniteBladesPlusPowerAmount, instance.Owner.Creature, instance);
        if (power == null)
        {
            return;
        }

        power.DynamicVars.Cards.BaseValue += instance.DynamicVars.Cards.BaseValue;
        power.InvokeDisplayAmountChanged();
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

        if (__instance is not InfiniteBlades)
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

        if (__instance is not InfiniteBlades)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_INFINITE_BLADES.description");
        return false;
    }

    [HarmonyPatch(typeof(InfiniteBlades), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(InfiniteBlades __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(InfiniteBlades), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(InfiniteBlades __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.AddKeyword(CardKeyword.Innate);
        __instance.DynamicVars.Cards.UpgradeValueBy(1);
        return false;
    }
}