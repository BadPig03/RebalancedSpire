namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Boss;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SoulFyshPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SoulFyshConfig;

    private static async Task GazeMove(SoulFysh instance)
    {
        await DamageCmd.Attack(instance.GazeDamage).FromMonster(instance).WithAttackerAnim("AttackBeckon", 0.6f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/soul_fysh/soul_fysh_beckon").WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    [HarmonyPatch(typeof(SoulFysh), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(SoulFysh __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("BECKON_MOVE", __instance.BeckonMove, new StatusIntent(__instance.BeckonMoveAmount));
        MoveState moveState2 = new MoveState("DE_GAS_MOVE", __instance.DeGasMove, new SingleAttackIntent(__instance.DeGasDamage));
        MoveState moveState3 = new MoveState("GAZE_MOVE", _ => GazeMove(__instance), new SingleAttackIntent(__instance.GazeDamage));
        MoveState moveState4 = new MoveState("FADE_MOVE", __instance.FadeMove, new BuffIntent());
        MoveState moveState5 = new MoveState("SCREAM_MOVE", __instance.ScreamMove, new SingleAttackIntent(__instance.ScreamDamage), new DebuffIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState5);
        list.Add(moveState4);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}