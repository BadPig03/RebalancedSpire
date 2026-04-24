namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Elite;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BygoneEffigyPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BygoneEffigyConfig;

    private static int StrengthPowerAmount => 3;

    private static readonly Func<BygoneEffigy, IReadOnlyList<Creature>, Task>? _initialSleepMoveDelegate = Helpers.GetDelegate<BygoneEffigy>("InitialSleepMove");
    private static readonly Func<BygoneEffigy, IReadOnlyList<Creature>, Task>? _slashMoveDelegate = Helpers.GetDelegate<BygoneEffigy>("SlashMove");

    private static async Task InitialSleepMove(BygoneEffigy instance, IReadOnlyList<Creature> targets)
    {
        if (_initialSleepMoveDelegate == null)
        {
            return;
        }

        await _initialSleepMoveDelegate(instance, targets);
    }

    private static async Task WakeMove(BygoneEffigy instance)
    {
        if (TestMode.IsOff)
        {
            NRunMusicController.Instance?.TriggerEliteSecondPhase();
        }
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, StrengthPowerAmount, instance.Creature, null);
        LocString line = MonsterModel.L10NMonsterLookup("BYGONE_EFFIGY.moves.SLEEP.speakLine2");
        TalkCmd.Play(line, instance.Creature, VfxColor.DarkGray, VfxDuration.Long);
        await Cmd.Wait(0.5f);
    }

    private static async Task SlashMove(BygoneEffigy instance, IReadOnlyList<Creature> targets)
    {
        if (_slashMoveDelegate == null)
        {
            return;
        }

        await _slashMoveDelegate(instance, targets);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, StrengthPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(BygoneEffigy), nameof(BygoneEffigy.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(BygoneEffigy __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("SLEEP_MOVE", t => InitialSleepMove(__instance, t), new SleepIntent());
        MoveState moveState2 = new MoveState("WAKE_MOVE", _ => WakeMove(__instance), new BuffIntent());
        MoveState moveState3 = new MoveState("SLASHES_MOVE", t => SlashMove(__instance, t), new SingleAttackIntent(__instance.SlashDamage), new BuffIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState3;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}