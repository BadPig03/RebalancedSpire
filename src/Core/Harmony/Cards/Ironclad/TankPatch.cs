namespace RebalancedSpire.Core.Harmony.Cards.Ironclad;

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
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TankPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Tank;

    private static async Task OnPlay(Tank instance, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "PowerUp", instance.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<TankPlusPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["TankPlusPower"].BaseValue, instance.Owner.Creature, instance);
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

        if (__instance is not Tank)
        {
            return true;
        }

        __result = 2;
        return false;
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

        if (__instance is not Tank)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<TankPlusPower>(8)
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

        if (__instance is not Tank)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_TANK.description");
        return false;
    }

    [HarmonyPatch(typeof(Tank), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Tank __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(Tank), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Tank __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars["TankPlusPower"].UpgradeValueBy(4);
        return false;
    }
}