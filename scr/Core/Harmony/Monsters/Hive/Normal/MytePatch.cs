namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MytePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.MyteConfig;

    private static int ToxicAmount => 1;

    private static readonly Func<Myte, IReadOnlyList<Creature>, Task>? _biteMoveDelegate = Helpers.GetDelegate<Myte>("BiteMove");
    private static readonly Func<Myte, IReadOnlyList<Creature>, Task>? _suckMoveDelegate = Helpers.GetDelegate<Myte>("SuckMove");

    private static async Task ToxicMove(Myte instance, IReadOnlyList<Creature> targets)
    {
        if (TestMode.IsOff)
        {
            NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
            Creature? target = LocalContext.GetMe(instance.CombatState)?.Creature;
            creatureNode?.GetSpecialNode<NMyteVfx>("%NMyteVfx")?.SetTarget(target);
        }
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/mite/mite_cast");
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.6f);
        await CardPileCmd.AddToCombatAndPreview<Toxic>(targets, PileType.Hand, ToxicAmount, addedByPlayer: false);
    }

    private static async Task BiteMove(Myte instance, IReadOnlyList<Creature> targets)
    {
        if (_biteMoveDelegate == null)
        {
            return;
        }

        await _biteMoveDelegate(instance, targets);
    }

    private static async Task SuckMove(Myte instance, IReadOnlyList<Creature> targets)
    {
        if (_suckMoveDelegate == null)
        {
            return;
        }

        await _suckMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(Myte), nameof(Myte.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix(Myte __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("TOXIC_MOVE", t => ToxicMove(__instance, t), new StatusIntent(1));
        MoveState moveState2 = new MoveState("BITE_MOVE", t => BiteMove(__instance, t), new SingleAttackIntent(__instance.BiteDamage));
        MoveState moveState3 = new MoveState("SUCK_MOVE", t => SuckMove(__instance, t), new SingleAttackIntent(__instance.SuckDamage), new BuffIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("INIT_MOVE");
        branchState.AddState(moveState, () => __instance.Creature.SlotName == "first");
        branchState.AddState(moveState3, () => __instance.Creature.SlotName == "second");
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, branchState);
        return false;
    }
}