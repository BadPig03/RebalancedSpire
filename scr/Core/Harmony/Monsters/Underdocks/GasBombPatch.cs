namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks;

using Core.Powers;
using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Vfx;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryUnderdocks)]
// ReSharper disable InconsistentNaming
public static class GasBombPatch
{
    [HarmonyPatch(typeof(GasBomb), nameof(GasBomb.ExplodeMove))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExplodeMove(GasBomb __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        __result = ExplodeMove(__instance);
        return false;
    }

    private static async Task ExplodeMove(GasBomb instance)
    {
        instance.HasExploded = true;
        await DamageCmd.Attack(instance.ExplodeDamage).FromMonster(instance).WithAttackerAnim("ExplodeTrigger", 0.1f) .WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_explode").WithHitVfxNode(_ => NGaseousImpactVfx.Create(CombatSide.Player, instance.CombatState, new Color("#402f45"))).Execute(null);
        await PowerCmd.Remove<PingPongPower>(instance.Creature);
        await CreatureCmd.Kill(instance.Creature);
    }
}