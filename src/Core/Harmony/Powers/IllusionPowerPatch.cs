namespace RebalancedSpire.Core.Harmony.Powers;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class IllusionPowerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheObscuraConfig;

    private const int StrengthPowerAmount = -5;

    private static async Task ReviveMove(IllusionPower instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner, "WakeUpTrigger", 0f);
        instance.GetInternalData<IllusionPower.Data>().isReviving = false;
        await CreatureCmd.Heal(instance.Owner, instance.Owner.MaxHp - instance.Owner.CurrentHp);
        if (instance.Owner.Monster is not Parafright)
        {
            return;
        }

        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/obscura/obscura_hologram_heal");
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Owner, StrengthPowerAmount, instance.Owner, null);
    }

    [HarmonyPatch(typeof(IllusionPower), "ReviveMove")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ReviveMove(IllusionPower __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = ReviveMove(__instance);
        return false;
    }
}