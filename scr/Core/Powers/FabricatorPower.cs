using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace RebalancedSpire.scr.Core.Powers;

using Harmony.Monsters.Glory;

public sealed class FabricatorPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new HpLossVar(0)
    ]);

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        DynamicVars.HpLoss.BaseValue = GetSpawnBotDamage(Owner);
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner)
        {
            return Task.CompletedTask;
        }

        DynamicVars.HpLoss.BaseValue = GetSpawnBotDamage(Owner);
        if (IsHpRemainingEnough(creature))
        {
            return Task.CompletedTask;
        }

        var state = (MoveState?) creature.Monster?.MoveStateMachine?.States["DISINTEGRATE_MOVE"];
        if (state == null)
        {
            return Task.CompletedTask;
        }

        creature.Monster?.SetMoveImmediate(state);
        return Task.CompletedTask;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.GetPower<MinionPower>() == null || !IsHpRemainingEnough(Owner) || wasRemovalPrevented)
        {
            return Task.CompletedTask;
        }

        var fabricator = (Fabricator?) Owner.Monster;
        if (fabricator is not { CanFabricate: true } || !fabricator.IntendsToAttack)
        {
            return Task.CompletedTask;
        }

        var state = (MoveState?) fabricator.MoveStateMachine?.States["FABRICATE_MOVE"];
        if (state == null)
        {
            return Task.CompletedTask;
        }

        fabricator.SetMoveImmediate(state);
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == null || target != Owner || dealer is not { IsPlayer: true })
        {
            return 1;
        }

        return CombatState.Enemies.Any(c => c.IsAlive && c.GetPower<MinionPower>() != null) ? 0.5m : 1;
    }

    public static int GetSpawnBotDamage(Creature creature)
    {
        return (int) (creature.MaxHp * FabricatorPatch.SpawnBotDamageRatio);
    }

    public static bool IsHpRemainingEnough(Creature creature)
    {
        return creature.CurrentHp > 2 * GetSpawnBotDamage(creature);
    }
}