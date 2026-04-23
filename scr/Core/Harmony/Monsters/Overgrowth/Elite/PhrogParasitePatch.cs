namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Elite;

using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PhrogParasitePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.PhrogParasiteConfig;

    private static int InitInfectedAmount => 2;
    private static int InfectedAmount => 1;
    private static float InitSize => 0.3f;
    private static float IncreasedSize => 0.3f;
    private static float MaxHpRatio => 0.2f;
    private static int LashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private static async Task ProliferationMove(PhrogParasite instance)
    {
        SfxCmd.Play(instance.CastSfx);
        NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.ScaleTo(InitSize + GetInfestedAmount(instance) * IncreasedSize, 0.75f);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.75f);
        await CreatureCmd.GainMaxHp(instance.Creature, (int) (instance.Creature.MaxHp * MaxHpRatio));
        await PowerCmd.Apply<InfestedPlusPower>(instance.Creature, InfectedAmount, instance.Creature, null);
    }

    private static async Task InfectMove(PhrogParasite instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(instance.CastSfx);
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.75f);
        foreach (Creature target in targets)
        {
            NWormyImpactVfx? nWormyImpactVfx = NWormyImpactVfx.Create(target);
            if (nWormyImpactVfx != null)
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nWormyImpactVfx);
            }
        }
        await CardPileCmd.AddToCombatAndPreview<Infection>(targets, PileType.Discard, GetInfestedAmount(instance), addedByPlayer: false);
    }

    private static async Task LashMove(PhrogParasite instance)
    {
        await DamageCmd.Attack(LashDamage).WithHitCount(GetInfestedAmount(instance)).FromMonster(instance).WithAttackerAnim("Attack", 0.55f).OnlyPlayAnimOnce().WithAttackerFx(null, instance.AttackSfx).WithHitVfxNode(NWormyImpactVfx.Create).Execute(null);
    }

    private static async Task AfterAddedToRoom(PhrogParasite instance)
    {
        NCombatRoom.Instance?.GetCreatureNode(instance.Creature)?.ScaleTo(InitSize + GetInfestedAmount(instance) * IncreasedSize, 0);
        await PowerCmd.Apply<InfestedPlusPower>(instance.Creature, InitInfectedAmount, instance.Creature, null);
    }

    private static int GetInfestedAmount(PhrogParasite instance)
    {
        if (instance._creature == null || instance.Creature.GetPower<InfestedPlusPower>() == null)
        {
            return 0;
        }

        return instance.Creature.GetPowerAmount<InfestedPlusPower>();
    }

    [HarmonyPatch(typeof(PhrogParasite), nameof(PhrogParasite.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(PhrogParasite __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(PhrogParasite), nameof(PhrogParasite.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(PhrogParasite __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("LASH_MOVE", _ => LashMove(__instance), new MultiAttackIntent(LashDamage, 2));
        MoveState moveState2 = new MoveState("PROLIFERATION_MOVE", _ => ProliferationMove(__instance), new BuffIntent());
        MoveState moveState3 = new MoveState("INFECT_MOVE", t => InfectMove(__instance, t), new StatusIntent(3));
        MoveState moveState4 = new MoveState("LASH_2_MOVE", _ => LashMove(__instance), new MultiAttackIntent(LashDamage, 3));
        MoveState moveState5 = new MoveState("PROLIFERATION_2_MOVE", _ => ProliferationMove(__instance), new BuffIntent());
        MoveState moveState6 = new MoveState("INFECT_2_MOVE", t => InfectMove(__instance, t), new StatusIntent(4));
        MoveState moveState7 = new MoveState("LASH_3_MOVE", _ => LashMove(__instance), new MultiAttackIntent(LashDamage, 4));
        MoveState moveState8 = new MoveState("PROLIFERATION_3_MOVE", _ => ProliferationMove(__instance), new BuffIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState6;
        moveState6.FollowUpState = moveState7;
        moveState7.FollowUpState = moveState8;
        moveState8.FollowUpState = moveState6;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(moveState7);
        list.Add(moveState8);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}