namespace RebalancedSpire.scr.Core.Harmony.Monsters.Overgrowth.Boss;

using Core.Powers;
using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class KinFollowerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheKinConfig;

    private static int MinionPowerAmount => 1;
    private static int QuickSlashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private static int BoomerangDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    private static int BoomerangCount => 2;
    private static int GuardBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 13);
    private static int RevengeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 6);
    private static int GuardFakeBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);
    private static int HealAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 6);

    private static async Task AfterAddedToRoom(KinFollower instance)
    {
        var currentMaxHp = instance.Creature.MaxHp;
        if (instance.StartsWithDance)
        {
            await PowerCmd.Apply<MinionFakePower>(instance.Creature, MinionPowerAmount, instance.Creature, null);
            await CreatureCmd.SetMaxAndCurrentHp(instance.Creature, currentMaxHp * 0.5m);
        }
        else
        {
            await CreatureCmd.SetMaxAndCurrentHp(instance.Creature, currentMaxHp * 1.5m);
        }
        NRunMusicController.Instance?.UpdateMusicParameter("the_kin_progress", 0f);
    }

    private static async Task QuickSlashMove(KinFollower instance)
    {
        await DamageCmd.Attack(QuickSlashDamage).FromMonster(instance).WithAttackerAnim("SlashTrigger", 0.2f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_quick_slash").WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task BoomerangMove(KinFollower instance, IReadOnlyList<Creature> targets)
    {
        if (TestMode.IsOff)
        {
            Vector2? vector = null;
            foreach (Creature target in targets)
            {
                NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(target);
                if (!vector.HasValue || vector.Value.X > creatureNode?.GlobalPosition.X)
                {
                    vector = creatureNode?.GlobalPosition;
                }
            }
            NCreature? creatureNode2 = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
            Node2D? specialNode = creatureNode2?.GetSpecialNode<Node2D>("Visuals/AttackDistanceControl");
            if (vector != null && creatureNode2 != null && specialNode != null)
            {
                specialNode.Position = Vector2.Left * (creatureNode2.GlobalPosition.X - vector.Value.X) / creatureNode2.Body.Scale;
            }
        }
        await DamageCmd.Attack(BoomerangDamage).WithHitCount(BoomerangCount).FromMonster(instance).WithAttackerAnim("BoomerangTrigger", 0.2f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_boomerang_slashh").WithHitFx("vfx/vfx_attack_slash").OnlyPlayAnimOnce().Execute(null);
    }

    private static async Task GuardMove(KinFollower instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_buff");
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.9f);
        await CreatureCmd.GainBlock(instance.Creature, new BlockVar(GuardBlock, ValueProp.Move), null);
        await PowerCmd.Apply<GuardPower>(instance.Creature, 1, instance.Creature, null);
    }

    private static async Task PowerDanceMove(KinFollower instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_buff");
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.9f);
        await PowerCmd.Apply<StrengthPower>(instance.Creature, instance.DanceStrength, instance.Creature, null);
    }

    private static async Task RevengeDanceMove(KinFollower instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_buff");
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.9f);
        await PowerCmd.Apply<StrengthPower>(instance.Creature, RevengeDamage, instance.Creature, null);
    }

    private static async Task RevengeMove(KinFollower instance)
    {
        await DamageCmd.Attack(RevengeDamage).FromMonster(instance).WithAttackerAnim("SlashTrigger", 0.2f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_quick_slash").WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task QuickSlashFakeMove(KinFollower instance)
    {
        await DamageCmd.Attack(0).FromMonster(instance).WithAttackerAnim("SlashTrigger", 0.2f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_quick_slash").WithHitFx("vfx/vfx_attack_slash").Execute(null);
    }

    private static async Task BoomerangFakeMove(KinFollower instance, IReadOnlyList<Creature> targets)
    {
        if (TestMode.IsOff)
        {
            Vector2? vector = null;
            foreach (Creature target in targets)
            {
                NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(target);
                if (!vector.HasValue || vector.Value.X > creatureNode?.GlobalPosition.X)
                {
                    vector = creatureNode?.GlobalPosition;
                }
            }
            NCreature? creatureNode2 = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
            Node2D? specialNode = creatureNode2?.GetSpecialNode<Node2D>("Visuals/AttackDistanceControl");
            if (vector != null && creatureNode2 != null && specialNode != null)
            {
                specialNode.Position = Vector2.Left * (creatureNode2.GlobalPosition.X - vector.Value.X) / creatureNode2.Body.Scale;
            }
        }
        await DamageCmd.Attack(0).WithHitCount(BoomerangCount).FromMonster(instance).WithAttackerAnim("BoomerangTrigger", 0.2f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_boomerang_slashh").WithHitFx("vfx/vfx_attack_slash").OnlyPlayAnimOnce().Execute(null);
    }

    private static async Task GuardFakeMove(KinFollower instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_buff");
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.9f);
        await CreatureCmd.GainBlock(instance.Creature, new BlockVar(GuardFakeBlock, ValueProp.Move), null);
        TalkCmd.Play(MonsterModel.L10NMonsterLookup("KIN_FOLLOWER_FAKE.randomLine" + instance.RunRng.Shuffle.NextInt(1, 6)), instance.Creature, VfxColor.Purple, VfxDuration.Standard);
    }

    private static async Task PowerDanceFakeMove(KinFollower instance)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_kin_minion/the_kin_minion_buff");
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.9f);
        await CreatureCmd.Heal(instance.Creature, HealAmount);
    }

    private static async Task EscapeMove(KinFollower instance)
    {
        var power = instance.Creature.GetPower<MinionFakePower>();
        if (power == null)
        {
            return;
        }

        await Cmd.Wait(0.5f);
        foreach (Player player in instance.CombatState.Players)
        {
            var room = (CombatRoom?) player.RunState.CurrentRoom;
            if (room == null)
            {
                return;
            }

            room.AddExtraReward(player, new PotionReward(player));
            room.AddExtraReward(player, new RelicReward(RelicRarity.Common, player));
        }
        instance.Creature.RemoveAllPowersInternalExcept();
        CombatManager.Instance.RemoveCreature(instance.Creature);
        instance.Creature.CombatState?.RemoveCreature(instance.Creature);
    }

    [HarmonyPatch(typeof(KinFollower), nameof(KinFollower.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(KinFollower __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(KinFollower), nameof(KinFollower.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(KinFollower __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("QUICK_SLASH_MOVE", _ => QuickSlashMove(__instance), new SingleAttackIntent(QuickSlashDamage));
        MoveState moveState2 = new MoveState("BOOMERANG_MOVE", t => BoomerangMove(__instance, t), new MultiAttackIntent(BoomerangDamage, BoomerangCount));
        MoveState moveState3 = new MoveState("GUARD_MOVE", _ => GuardMove(__instance), new DefendIntent());
        MoveState moveState4 = new MoveState("POWER_DANCE_MOVE", _ => PowerDanceMove(__instance), new BuffIntent());
        MoveState moveState5 = new MoveState("REVENGE_DANCE_MOVE", _ => RevengeDanceMove(__instance), new BuffIntent());
        MoveState moveState6 = new MoveState("REVENGE_MOVE", _ => RevengeMove(__instance), new SingleAttackIntent(RevengeDamage));
        MoveState moveState_ = new MoveState("QUICK_SLASH_FAKE_MOVE", _ => QuickSlashFakeMove(__instance), new SingleAttackIntent(0));
        MoveState moveState_2 = new MoveState("BOOMERANG_FAKE_MOVE", t => BoomerangFakeMove(__instance, t), new MultiAttackIntent(0, BoomerangCount));
        MoveState moveState_3 = new MoveState("GUARD_FAKE_MOVE", _ => GuardFakeMove(__instance), new DefendIntent());
        MoveState moveState_4 = new MoveState("POWER_DANCE_FAKE_MOVE", _ => PowerDanceFakeMove(__instance), new BuffIntent());
        MoveState moveState7 = new MoveState("ESCAPE_MOVE", _ => EscapeMove(__instance), new EscapeIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState;
        moveState5.FollowUpState = moveState6;
        moveState6.FollowUpState = moveState6;
        moveState_.FollowUpState = moveState_2;
        moveState_2.FollowUpState = moveState_3;
        moveState_3.FollowUpState = moveState_4;
        moveState_4.FollowUpState = moveState_;
        moveState7.FollowUpState = moveState7;
        ConditionalBranchState branchState = new ConditionalBranchState("KinFollower");
        branchState.AddState(moveState_, () => __instance.StartsWithDance);
        branchState.AddState(moveState, () => !__instance.StartsWithDance);
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        list.Add(moveState_);
        list.Add(moveState_2);
        list.Add(moveState_3);
        list.Add(moveState_4);
        list.Add(moveState7);
        __result = new MonsterMoveStateMachine(list, branchState);
        return false;
    }

    [HarmonyPatch(typeof(MonsterModel), nameof(MonsterModel.Title), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Title(MonsterModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return;
        }

        if (__instance is not KinFollower { StartsWithDance: true })
        {
            return;
        }

        __result = MonsterModel.L10NMonsterLookup("KIN_FOLLOWER_FAKE.name");
    }
}