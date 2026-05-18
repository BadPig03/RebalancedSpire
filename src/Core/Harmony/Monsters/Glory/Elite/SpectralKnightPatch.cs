namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Elite;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SpectralKnightPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.KnightsConfig;

    private const int HexPowerAmount = 1;
    private const int IntangiblePowerAmount = 1;
    private const int SoulFlameCount = 3;

    private static int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 12, 10);
    private static int SoulFlameDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    private static int SoulSlashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);

    private static async Task HexMove(SpectralKnight instance, IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(instance.Creature, "Cast", 0.3f);
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/spectral_knight/spectral_knight_hex");
        foreach (var target in targets)
        {
            await PowerCmd.Apply<HexPower>(new ThrowingPlayerChoiceContext(), target, HexPowerAmount, instance.Creature, null);
        }
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), instance.Creature, IntangiblePowerAmount, instance.Creature, null);
    }

    private static async Task SoulSlashMove(SpectralKnight instance)
    {
        await DamageCmd.Attack(SoulSlashDamage).FromMonster(instance).WithAttackerAnim("AttackSword", 0.25f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/spectral_knight/spectral_knight_soul_slash").WithHitFx("vfx/vfx_attack_slash").Execute(null);
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), instance.Creature, IntangiblePowerAmount, instance.Creature, null);
    }

    private static async Task SoulFlameMove(SpectralKnight instance)
    {
        await DamageCmd.Attack(SoulFlameDamage).WithHitCount(SoulFlameCount).FromMonster(instance).OnlyPlayAnimOnce().WithAttackerAnim("AttackFlame", 0.25f).WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/spectral_knight/spectral_knight_soul_flame").Execute(null);
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), instance.Creature, IntangiblePowerAmount, instance.Creature, null);
    }

    private static async Task AfterAddedToRoom(SpectralKnight instance)
    {
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), instance.Creature, IntangiblePowerAmount, instance.Creature, null);
        var count = instance.CombatState.Players.Count;
        if (count <= 1)
        {
            return;
        }

        await CreatureCmd.SetMaxAndCurrentHp(instance.Creature, (int) Math.Sqrt(instance.Creature.MaxHp * MinInitialHp));
    }

    [HarmonyPatch(typeof(SpectralKnight), "MinInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = MinInitialHp;
    }

    [HarmonyPatch(typeof(MonsterModel), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(MonsterModel __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SpectralKnight knight)
        {
            return true;
        }

        __result = AfterAddedToRoom(knight);
        return false;
    }

    [HarmonyPatch(typeof(SpectralKnight), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(SpectralKnight __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("HEX", t => HexMove(__instance, t), new DebuffIntent(), new BuffIntent());
        MoveState moveState2 = new MoveState("SOUL_SLASH", _ => SoulSlashMove(__instance), new SingleAttackIntent(SoulSlashDamage), new BuffIntent());
        MoveState moveState3 = new MoveState("SOUL_FLAME", _ => SoulFlameMove(__instance), new MultiAttackIntent(SoulFlameDamage, SoulFlameCount), new BuffIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}