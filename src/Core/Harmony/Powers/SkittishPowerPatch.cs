namespace RebalancedSpire.Core.Harmony.Powers;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SkittishPowerPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.PhantasmalGardener;

    private const int StrengthPowerAmount = -1;

    private static async Task AfterAttack(SkittishPower instance)
    {
        instance.HasGainedBlockThisTurn = true;
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/phantasmal_gardeners/phantasmal_gardeners_retract");
        await CreatureCmd.TriggerAnim(instance.Owner, "BlockStart", 0.3f);
        await CreatureCmd.GainBlock(instance.Owner, instance.Amount, ValueProp.Unpowered, null);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Owner, StrengthPowerAmount, instance.Owner, null);
    }

    [HarmonyPatch(typeof(SkittishPower), "AfterAttack")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAttack(SkittishPower __instance, AttackCommand command, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance.HasGainedBlockThisTurn || !command.DamageProps.HasFlag(ValueProp.Move) || command.ModelSource is not CardModel)
        {
            return true;
        }

        var damageResult = command.Results.SelectMany(r => r).FirstOrDefault(r => r.Receiver == __instance.Owner);
        if (damageResult == null || damageResult.UnblockedDamage == 0)
        {
            return true;
        }

        __result = AfterAttack(__instance);
        return false;
    }

    [HarmonyPatch(typeof(PowerModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(PowerModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SkittishPower)
        {
            return true;
        }

        __result = new LocString("powers", "REBALANCED_SPIRE_POWER_SKITTISH_POWER.description");
        return false;
    }

    [HarmonyPatch(typeof(PowerModel), "SmartDescriptionLocKey", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_SmartDescriptionLocKey(PowerModel __instance, ref string __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SkittishPower)
        {
            return true;
        }

        __result = "REBALANCED_SPIRE_POWER_SKITTISH_POWER.smartDescription";
        return false;
    }
}