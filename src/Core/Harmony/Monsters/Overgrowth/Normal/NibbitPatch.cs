namespace RebalancedSpire.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NibbitPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.NibbitConfig;

    private static int ButtDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 11);
    private static int SliceDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private static int SliceBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

    private static async Task ButtMove(Nibbit instance)
    {
        await DamageCmd.Attack(ButtDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.15f).WithAttackerFx(null, SrcHelpers.GetSfx(instance, "AttackSfx")).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task SliceMove(Nibbit instance)
    {
        await DamageCmd.Attack(SliceDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.15f).WithAttackerFx(null, SrcHelpers.GetSfx(instance, "AttackSfx")).WithHitFx("vfx/vfx_attack_slash").Execute(null);
        await CreatureCmd.GainBlock(instance.Creature, SliceBlock, ValueProp.Move, null);
    }

    private static bool IsAlone(Nibbit instance)
    {
        return instance.Creature.Monster is not Nibbit nibbit || nibbit.IsAlone;
    }

    private static bool IsFront(Nibbit instance)
    {
        return instance.Creature.Monster is not Nibbit nibbit || nibbit.IsFront;
    }

    [HarmonyPatch(typeof(Nibbit), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Nibbit __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("BUTT_MOVE", _ => ButtMove(__instance), new SingleAttackIntent(ButtDamage));
        MoveState moveState2 = new MoveState("SLICE_MOVE", _ => SliceMove(__instance), new SingleAttackIntent(SliceDamage), new DefendIntent());
        MoveState moveState3 = new MoveState("HISS_MOVE", __instance.HissMove, new BuffIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("INIT_MOVE");
        if (__instance._isAlone)
        {
            branchState.AddState(moveState, () => IsAlone(__instance));
        }
        else
        {
            branchState.AddState(moveState2, () => IsFront(__instance));
            branchState.AddState(moveState3, () => !IsFront(__instance));
        }
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(branchState);
        __result = new MonsterMoveStateMachine(list, branchState);
        return false;
    }
}