namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SludgeSpinnerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SludgeSpinnerConfig;

    private static int StrengthPowerAmount => 2;
    private static int BlockAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private static async Task RageMove(SludgeSpinner instance)
    {
        await CreatureCmd.GainBlock(instance.Creature, new BlockVar(BlockAmount, ValueProp.Move), null);
        await PowerCmd.Apply<StrengthPower>(instance.Creature, StrengthPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(SludgeSpinner), nameof(SludgeSpinner.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(SludgeSpinner __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("OIL_SPRAY_MOVE", __instance.OilSprayMove, new SingleAttackIntent(__instance.OilSprayDamage), new DebuffIntent());
        MoveState moveState2 = new MoveState("SLAM_MOVE", __instance.SlamMove, new SingleAttackIntent(__instance.SlamDamage));
        MoveState moveState3 = new MoveState("RAGE_MOVE", _ => RageMove(__instance), new DefendIntent(), new BuffIntent());
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