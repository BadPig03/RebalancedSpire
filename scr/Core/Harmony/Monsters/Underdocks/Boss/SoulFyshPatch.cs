namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Boss;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SoulFyshPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SoulFyshConfig;

    private static readonly Func<SoulFysh, IReadOnlyList<Creature>, Task>? _beckonMoveDelegate = Helpers.GetDelegate<SoulFysh>("BeckonMove");
    private static readonly Func<SoulFysh, IReadOnlyList<Creature>, Task>? _deGasMoveDelegate = Helpers.GetDelegate<SoulFysh>("DeGasMove");
    private static readonly Func<SoulFysh, IReadOnlyList<Creature>, Task>? _fadeMoveDelegate = Helpers.GetDelegate<SoulFysh>("FadeMove");
    private static readonly Func<SoulFysh, IReadOnlyList<Creature>, Task>? _screamMoveDelegate = Helpers.GetDelegate<SoulFysh>("ScreamMove");

    private static async Task BeckonMove(SoulFysh instance, IReadOnlyList<Creature> targets)
    {
        if (_beckonMoveDelegate == null)
        {
            return;
        }

        await _beckonMoveDelegate(instance, targets);
    }

    private static async Task DeGasMove(SoulFysh instance, IReadOnlyList<Creature> targets)
    {
        if (_deGasMoveDelegate == null)
        {
            return;
        }

        await _deGasMoveDelegate(instance, targets);
    }

    private static async Task GazeMove(SoulFysh instance)
    {
        await DamageCmd.Attack(instance.GazeDamage).FromMonster(instance).WithAttackerAnim("AttackBeckon", 0.6f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/soul_fysh/soul_fysh_beckon").WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task FadeMove(SoulFysh instance, IReadOnlyList<Creature> targets)
    {
        if (_fadeMoveDelegate == null)
        {
            return;
        }

        await _fadeMoveDelegate(instance, targets);
    }

    private static async Task ScreamMove(SoulFysh instance, IReadOnlyList<Creature> targets)
    {
        if (_screamMoveDelegate == null)
        {
            return;
        }

        await _screamMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(SoulFysh), nameof(SoulFysh.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(SoulFysh __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("BECKON_MOVE", t => BeckonMove(__instance, t), new StatusIntent(__instance.BeckonMoveAmount));
        MoveState moveState2 = new MoveState("DE_GAS_MOVE", t => DeGasMove(__instance, t), new SingleAttackIntent(__instance.DeGasDamage));
        MoveState moveState3 = new MoveState("GAZE_MOVE", _ => GazeMove(__instance), new SingleAttackIntent(__instance.GazeDamage));
        MoveState moveState4 = new MoveState("FADE_MOVE", t => FadeMove(__instance, t), new BuffIntent());
        MoveState moveState5 = new MoveState("SCREAM_MOVE", t => ScreamMove(__instance, t), new SingleAttackIntent(__instance.ScreamDamage), new DebuffIntent());
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