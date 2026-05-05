namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Elite;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class MechaKnightPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.MechaKnightConfig;

    private static int BurnAmount => 2;

    private static async Task FlamethrowerMove(MechaKnight instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_flamethrower");
        await CreatureCmd.TriggerAnim(instance.Creature, "flamethrower", 1.5f);
        await CardPileCmd.AddToCombatAndPreview<Burn>(targets, PileType.Hand, BurnAmount, false);
    }

    [HarmonyPatch(typeof(MechaKnight), nameof(MechaKnight.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(MechaKnight __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("CHARGE_MOVE", __instance.ChargeMove, new SingleAttackIntent(MechaKnight.ChargeDamage));
        MoveState moveState2 = new MoveState("FLAMETHROWER_MOVE", t => FlamethrowerMove(__instance, t), new StatusIntent(BurnAmount));
        MoveState moveState3 = new MoveState("WINDUP_MOVE", __instance.WindupMove, new DefendIntent(), new BuffIntent());
        MoveState moveState4 = new MoveState("HEAVY_CLEAVE_MOVE", __instance.HeavyCleaveMove, new SingleAttackIntent(MechaKnight.HeavyCleaveDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}