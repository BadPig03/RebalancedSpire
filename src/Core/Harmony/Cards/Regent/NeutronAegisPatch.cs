namespace RebalancedSpire.Core.Harmony.Cards.Regent;

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
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NeutronAegisPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.NeutronAegis;

    private static async Task OnPlay(NeutronAegis instance, PlayerChoiceContext choiceContext)
    {
        var num = instance.ResolveStarXValue();
        if (num >= instance.DynamicVars.Stars.IntValue)
        {
            num *= 2;
        }
        await PowerCmd.Apply<PlatingPower>(choiceContext, instance.Owner.Creature, num, instance.Owner.Creature, instance);
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

        if (__instance is not NeutronAegis)
        {
            return true;
        }

        __result = 0;
        return false;
    }

    [HarmonyPatch(typeof(NeutronAegis), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(NeutronAegis __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new StarsVar(5)
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

        if (__instance is not NeutronAegis)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_NEUTRON_AEGIS.description");
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "HasStarCostX", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_HasStarCostX(CardModel __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not NeutronAegis)
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(NeutronAegis), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(NeutronAegis __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(NeutronAegis), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(NeutronAegis __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Stars.UpgradeValueBy(-1);
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "ShouldGlowGoldInternal", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShouldGlowGoldInternal(CardModel __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not NeutronAegis neutronAegis)
        {
            return true;
        }

        __result = neutronAegis.Owner.PlayerCombatState?.Stars >= neutronAegis.DynamicVars.Stars.IntValue;
        return false;
    }
}