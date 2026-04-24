namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class VineShamblerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.VineShamblerConfig;

    private static int BlockAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 8);
    private static int TangledPowerAmount => 1;
    private static int SwipeCount => 2;

    private static readonly Func<VineShambler, IReadOnlyList<Creature>, Task>? _swipeMoveDelegate = Helpers.GetDelegate<VineShambler>("SwipeMove");
    private static readonly Func<VineShambler, IReadOnlyList<Creature>, Task>? _chompMoveDelegate = Helpers.GetDelegate<VineShambler>("ChompMove");

    private static async Task SwipeMove(VineShambler instance, IReadOnlyList<Creature> targets)
    {
        if (_swipeMoveDelegate == null)
        {
            return;
        }

        await _swipeMoveDelegate(instance, targets);
    }

    private static async Task GraspingVinesMove(VineShambler instance, IReadOnlyList<Creature> targets)
    {
        VfxCmd.PlayOnCreatures(targets, "vfx/monsters/vine_shambler_vines/vine_shambler_vines_vfx");
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/vine_shambler/vine_shambler_cast");
        await CreatureCmd.TriggerAnim(instance.Creature, "Vines", 0.5f);
        await PowerCmd.Apply<TangledPower>(new ThrowingPlayerChoiceContext(), targets, TangledPowerAmount, instance.Creature, null);
        await CreatureCmd.GainBlock(instance.Creature, new BlockVar(BlockAmount, ValueProp.Move), null);
    }

    private static async Task ChompMove(VineShambler instance, IReadOnlyList<Creature> targets)
    {
        if (_chompMoveDelegate == null)
        {
            return;
        }

        await _chompMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(VineShambler), nameof(VineShambler.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(VineShambler __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SWIPE_MOVE", t => SwipeMove(__instance, t), new MultiAttackIntent(__instance.SwipeDamage, SwipeCount));
        MoveState moveState2 = new MoveState("GRASPING_VINES_MOVE", t => GraspingVinesMove(__instance, t), new DefendIntent(), new CardDebuffIntent());
        MoveState moveState3 = new MoveState("CHOMP_MOVE", t => ChompMove(__instance, t), new SingleAttackIntent(__instance.ChompDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}