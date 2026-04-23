namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SlitheringStranglerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SlitheringStranglerConfig;

    private static int ConstrictPowerAmount => 2;

    private static async Task ConstrictMove(SlitheringStrangler instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/slithering_strangler/slithering_strangler_cast");
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.6f);
        await PowerCmd.Apply<ConstrictPower>(targets, ConstrictPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(SlitheringStrangler), nameof(SlitheringStrangler.ConstrictMove))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix(SlitheringStrangler __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = ConstrictMove(__instance, targets);
        return false;
    }
}