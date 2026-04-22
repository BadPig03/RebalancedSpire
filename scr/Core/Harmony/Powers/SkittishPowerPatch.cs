using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RebalancedSpire.scr.Core.Harmony.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SkittishPowerPatch
{
    private static int ReducedAmount => -1;

    [HarmonyPatch(typeof(SkittishPower), nameof(SkittishPower.AfterAttack))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix(SkittishPower __instance, AttackCommand command, ref Task __result)
    {
        if (__instance.HasGainedBlockThisTurn || !command.DamageProps.HasFlag(ValueProp.Move) || command.ModelSource is not CardModel)
        {
            return true;
        }

        DamageResult? damageResult = command.Results.FirstOrDefault(r => r.Receiver == __instance.Owner);
        if (damageResult == null || damageResult.UnblockedDamage == 0)
        {
            return true;
        }

        __result = AfterAttack(__instance);
        return false;
    }

    private static async Task AfterAttack(SkittishPower instance)
    {
        instance.HasGainedBlockThisTurn = true;
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/phantasmal_gardeners/phantasmal_gardeners_retract");
        await CreatureCmd.TriggerAnim(instance.Owner, "BlockStart", 0.3f);
        await CreatureCmd.GainBlock(instance.Owner, instance.Amount, ValueProp.Unpowered, null);
        await PowerCmd.Apply<StrengthPower>(instance.Owner, ReducedAmount, instance.Owner, null);
    }
}