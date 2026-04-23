namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class GremlinMercPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.GremlinMercConfig;

    private static async Task DoubleSmashMove(GremlinMerc instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.AttackSfx);
        VfxCmd.PlayOnCreatureCenters(targets, "vfx/vfx_coin_explosion_regular");
        await DamageCmd.Attack(instance.DoubleSmashDamage).WithHitCount(instance.DoubleSmashRepeat).FromMonster(instance).WithAttackerAnim("AttackDouble", 0.15f).OnlyPlayAnimOnce().WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
        foreach (ThieveryPower powerInstance in instance.Creature.GetPowerInstances<ThieveryPower>())
        {
            await powerInstance.Steal();
        }
        await PowerCmd.Apply<WeakPower>(targets, 1, instance.Creature, null);
    }

    [HarmonyPatch(typeof(GremlinMerc), nameof(GremlinMerc.DoubleSmashDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceDoubleSmashDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 1;
    }

    [HarmonyPatch(typeof(GremlinMerc), nameof(GremlinMerc.DoubleSmashMove))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_DoubleSmashMove(GremlinMerc __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = DoubleSmashMove(__instance, targets);
        return false;
    }
}