namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class ChomperPatch
{
    private static int DazedAmount => 2;
    private static int ClampDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private static int ClampCount => 2;

    private static async Task ClampMove(Chomper instance)
    {
        await DamageCmd.Attack(ClampDamage).WithHitCount(ClampCount).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithAttackerFx(null, instance.AttackSfx).WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task ScreechMove(Chomper instance, IReadOnlyList<Creature> targets)
    {
        LocString line = MonsterModel.L10NMonsterLookup("CHOMPER.moves.SCREECH.title");
        TalkCmd.Play(line, instance.Creature, VfxColor.Cyan);
        SfxCmd.Play(instance.CastSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 1f);
        await CardPileCmd.AddToCombatAndPreview<Dazed>(targets, PileType.Discard, DazedAmount, addedByPlayer: false);
    }

    [HarmonyPatch(typeof(Chomper), nameof(Chomper.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Chomper __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("CLAMP_MOVE", _ => ClampMove(__instance), new MultiAttackIntent(ClampDamage, ClampCount));
        MoveState moveState2 = new MoveState("SCREECH_MOVE", t => ScreechMove(__instance, t), new StatusIntent(DazedAmount));
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState;
        list.Add(moveState);
        list.Add(moveState2);
        __result = new MonsterMoveStateMachine(list, __instance._screamFirst ? moveState2 : moveState);
        return false;
    }
}