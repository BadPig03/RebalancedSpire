namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Elite;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MagiKnightPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.KnightsConfig;

    private static readonly Func<MagiKnight, IReadOnlyList<Creature>, Task>? _dampenMoveDelegate = Helpers.GetDelegate<MagiKnight>("DampenMove");
    private static readonly Func<MagiKnight, IReadOnlyList<Creature>, Task>? _magicBombMoveDelegate = Helpers.GetDelegate<MagiKnight>("MagicBombMove");

    private static async Task PowerShieldMove(MagiKnight instance)
    {
        await CreatureCmd.GainBlock(instance.Creature, instance.PowerShieldBlock, BlockProps.monsterMove, null);
    }

    private static async Task DampenMove(MagiKnight instance, IReadOnlyList<Creature> targets)
    {
        if (_dampenMoveDelegate == null)
        {
            return;
        }

        await _dampenMoveDelegate(instance, targets);
    }

    private static async Task PrepMove(MagiKnight instance)
    {
        TalkCmd.Play(new LocString("monsters", "MAGI_KNIGHT.PREP.banter"), instance.Creature, VfxColor.Orange, VfxDuration.Long);
        await Cmd.Wait(0.5f);
    }

    private static async Task Prep2Move(MagiKnight instance)
    {
        TalkCmd.Play(new LocString("monsters", "MAGI_KNIGHT.PREP_2.banter"), instance.Creature, VfxColor.Orange, VfxDuration.Long);
        await Cmd.Wait(0.5f);
    }

    private static async Task MagicBombMove(MagiKnight instance, IReadOnlyList<Creature> targets)
    {
        if (_magicBombMoveDelegate == null)
        {
            return;
        }

        await _magicBombMoveDelegate(instance, targets);
    }

    [HarmonyPatch(typeof(MagiKnight), nameof(MagiKnight.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(MagiKnight __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("POWER_SHIELD_MOVE", _ => PowerShieldMove(__instance), new DefendIntent());
        MoveState moveState2 = new MoveState("DAMPEN_MOVE", t => DampenMove(__instance, t), new DebuffIntent());
        MoveState moveState3 = new MoveState("PREP_MOVE", _ => PrepMove(__instance), new UnknownIntent());
        MoveState moveState4 = new MoveState("PREP_2_MOVE", _ => Prep2Move(__instance), new UnknownIntent());
        MoveState moveState5 = new MoveState("MAGIC_BOMB", t => MagicBombMove(__instance, t), new SingleAttackIntent(__instance.BombDamage));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState3;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}