namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TheForgottenPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheLostAndForgottenConfig;

    private static int MiasmaBlockAmount => 8;

    private static async Task MiasmaMove(TheForgotten instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.CastSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.5f);
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), targets, -instance.DebilitatingSmogDexStealAmount, instance.Creature, null);
        await CreatureCmd.GainBlock(instance.Creature, MiasmaBlockAmount + instance.Creature.GetPowerAmount<DexterityPower>(), BlockProps.monsterMove, null);
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), instance.Creature, instance.DebilitatingSmogDexStealAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(TheForgotten), nameof(TheForgotten.DreadDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceDreadDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 13);
    }

    [HarmonyPatch(typeof(TheForgotten), nameof(TheForgotten.MiasmaMove))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_MiasmaMove(TheForgotten __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = MiasmaMove(__instance, targets);
        return false;
    }
}