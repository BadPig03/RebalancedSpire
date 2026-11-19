namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Utils;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MightyWingsPatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.ShrineOfTheWarlord;
    private static readonly SavedAttachedState<MightyWings, int> TurnsTaken = new("REBALANCED_SPIRE_RELIC_MIGHTY_WINGS", _ => 0);

    private static async Task AfterPlayerTurnStart(MightyWings instance, PlayerChoiceContext choiceContext, Player player)
    {
        if (player != instance.Owner || !CombatManager.Instance.IsInProgress)
        {
            return;
        }

        var turns = TurnsTaken.GetValueOrDefault(instance, 0) + 1;
        var triggered = turns >= instance.DynamicVars["Turns"].IntValue;
        TurnsTaken.Set(instance, triggered ? 0 : turns);
        instance.InvokeDisplayAmountChanged();
        if (triggered)
        {
            instance.Flash();
            await PowerCmd.Apply<FlightPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["FlightPower"].BaseValue, instance.Owner.Creature, null);
        }
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterPlayerTurnStart")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterPlayerTurnStart(AbstractModel __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (Disabled || __instance is not MightyWings wings)
        {
            return true;
        }

        __result = AfterPlayerTurnStart(wings, choiceContext, player);
        return false;
    }

    [HarmonyPatch(typeof(MightyWings), "AfterDamageGiven")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterDamageGiven(ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(MightyWings), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar> { new("Turns", 4), new PowerVar<FlightPower>(1) }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled || __instance is not MightyWings)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_MIGHTY_WINGS.description");
        return false;
    }

    [HarmonyPatch(typeof(MightyWings), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = [HoverTipFactory.FromPower<FlightPower>()];
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "DisplayAmount", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_DisplayAmount(RelicModel __instance, ref int __result)
    {
        if (Disabled || __instance is not MightyWings wings)
        {
            return true;
        }

        __result = TurnsTaken.GetValueOrDefault(wings, 0);
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "ShowCounter", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShowCounter(RelicModel __instance, ref bool __result)
    {
        if (Disabled || __instance is not MightyWings)
        {
            return true;
        }

        __result = true;
        return false;
    }
}
