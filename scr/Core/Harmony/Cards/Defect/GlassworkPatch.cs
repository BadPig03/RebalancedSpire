namespace RebalancedSpire.scr.Core.Harmony.Cards.Defect;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class GlassworkPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.GlassworkConfig;

    private static async Task OnPlay(Glasswork instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(instance.Owner.Creature, instance.DynamicVars.Block, cardPlay);
        await OrbCmd.Channel<GlassOrb>(choiceContext, instance.Owner);
        var orbs = instance.Owner.PlayerCombatState?.OrbQueue.Orbs.OfType<GlassOrb>().ToList();
        if (orbs == null)
        {
            return;
        }

        foreach (var orb in orbs)
        {
            orb._passiveVal += instance.DynamicVars["Value"].BaseValue;
        }
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

        if (__instance is not Glasswork)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-GLASSWORK.description");
        return false;
    }

    [HarmonyPatch(typeof(Glasswork), nameof(Glasswork.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Glasswork __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(5, ValueProp.Move),
            new("Value", 2)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Glasswork), nameof(Glasswork.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Glasswork __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(Glasswork), nameof(Glasswork.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Glasswork __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Block.UpgradeValueBy(3);
        __instance.DynamicVars["Value"].UpgradeValueBy(1);
        return false;
    }
}