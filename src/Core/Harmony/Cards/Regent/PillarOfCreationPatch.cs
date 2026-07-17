namespace RebalancedSpire.Core.Harmony.Cards.Regent;

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
public static class PillarOfCreationPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.PillarOfCreation;

    private static async Task OnPlay(PillarOfCreation instance, PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<PillarOfCreationPlusPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars.Block.BaseValue, instance.Owner.Creature, instance);

    }

    [HarmonyPatch(typeof(PillarOfCreation), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(PillarOfCreation __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(3, ValueProp.Unpowered)
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

        if (__instance is not PillarOfCreation)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_PILLAR_OF_CREATION.description");
        return false;
    }

    [HarmonyPatch(typeof(PillarOfCreation), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(PillarOfCreation __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(PillarOfCreation), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(PillarOfCreation __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Block.UpgradeValueBy(1);
        return false;
    }
}