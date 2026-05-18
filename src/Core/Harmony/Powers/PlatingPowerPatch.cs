namespace RebalancedSpire.Core.Harmony.Powers;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PlatingPowerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.EternalArmorConfig;

    private static async Task AfterTurnEnd(PlatingPower instance, PlayerChoiceContext choiceContext)
    {
        if (instance.Owner.Side == CombatSide.Enemy)
        {
            await PowerCmd.ModifyAmount(choiceContext, instance, -instance.DynamicVars["Decrement"].BaseValue, null, null);
        }
        else if (!instance.Owner.HasPower<EternalArmorPower>())
        {
            await PowerCmd.Decrement(instance);
        }
    }

    [HarmonyPatch(typeof(PlatingPower), "AfterTurnEnd")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterTurnEnd(PlatingPower __instance, PlayerChoiceContext choiceContext, CombatSide side, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (side != CombatSide.Enemy)
        {
            return true;
        }

        __result = AfterTurnEnd(__instance, choiceContext);
        return false;
    }
}