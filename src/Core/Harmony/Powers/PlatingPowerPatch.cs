namespace RebalancedSpire.Core.Harmony.Powers;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PlatingPowerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.EternalArmorConfig;

    private static async Task AfterSideTurnStart(PlatingPower instance)
    {
        if (instance.Owner.Side == CombatSide.Enemy)
        {
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), instance, -instance.DynamicVars["Decrement"].BaseValue, null, null);
        }
        else if (!instance.Owner.HasPower<EternalArmorPower>())
        {
            await PowerCmd.Decrement(instance);
        }
    }

    [HarmonyPatch(typeof(PlatingPower), "AfterSideTurnStart")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterSideTurnStart(PlatingPower __instance, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (!participants.Contains(__instance.Owner) || __instance.Owner.Player?.PlayerCombatState?.TurnNumber == 1 || (__instance.Owner.Side == CombatSide.Enemy && combatState.RoundNumber == 1))
        {
            return true;
        }

        __result = AfterSideTurnStart(__instance);
        return false;
    }
}