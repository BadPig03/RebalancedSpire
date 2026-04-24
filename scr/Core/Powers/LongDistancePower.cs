using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace RebalancedSpire.scr.Core.Powers;

public sealed class LongDistancePower : CustomPowerModel
{
    private static int MaxSandpitAmount => 11;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new StringVar("TheInsatiable", ModelDb.Monster<TheInsatiable>().Title.GetFormattedText())
    ]);

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && props.IsPoweredAttack())
        {
            return CalculatePlayerMultiplier(Amount);
        }
        if ((dealer == Owner || dealer?.PetOwner == Owner.Player) && target?.Monster is TheInsatiable && props.IsPoweredAttack())
        {
            return CalculateEnemyMultiplier(Amount);
        }
        return 1;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Enemy)
        {
            return;
        }

        await PowerCmd.TickDownDuration(this);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not FranticEscape || cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, 1, Applier, null);
        if (Amount < MaxSandpitAmount)
        {
            return;
        }

        await Cmd.Wait(1f);
        var room = (CombatRoom?) CombatState.RunState.CurrentRoom;
        if (room == null)
        {
            return;
        }

        foreach (Player player in CombatState.Players)
        {
            Node2D? body = NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.Body;
            if (body != null)
            {
                body.Scale *= new Vector2(-1f, 1f);
            }
            room.AddExtraReward(player, new PotionReward(player));
            room.AddExtraReward(player, new RelicReward(RelicRarity.Rare, player));
        }
        foreach (Creature enemy in CombatState.Enemies)
        {
            if (enemy.Monster is not TheInsatiable)
            {
                continue;
            }

            enemy.RemoveAllPowersInternalExcept();
            CombatManager.Instance.RemoveCreature(enemy);
            enemy.CombatState?.RemoveCreature(enemy);
        }
    }

    private static decimal CalculatePlayerMultiplier(int amount)
    {
        return (decimal) Math.Max(0.2, 1.3 - 0.1 * amount - 0.1 * Math.Max(0, amount - 5) + 0.2 * Math.Max(0, amount - 8));
    }

    private static decimal CalculateEnemyMultiplier(int amount)
    {
        return (decimal) Math.Max(0.2, 1.4 - 0.1 * amount - 0.1 * Math.Max(0, amount - 6) + 0.2 * Math.Max(0, amount - 9));
    }
}