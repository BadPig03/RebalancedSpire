namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class PullAggroPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.PullAggro;

    private static async Task OnPlay(PullAggro instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await OstyCmd.Summon(choiceContext, instance.Owner, instance.DynamicVars.Summon.BaseValue, instance);
        await CreatureCmd.GainBlock(instance.Owner.Creature, instance.DynamicVars.Block, cardPlay);
    }

    [HarmonyPatch(typeof(PullAggro), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(PullAggro __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new SummonVar(8),
            new BlockVar(4, ValueProp.Move)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(PullAggro), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(PullAggro __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(PullAggro), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(PullAggro __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Summon.UpgradeValueBy(1);
        __instance.DynamicVars.Block.UpgradeValueBy(2);
        return false;
    }
}