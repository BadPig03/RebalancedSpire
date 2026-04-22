namespace RebalancedSpire.scr.Core.Powers;

using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Monsters;

public sealed class OmnidynamicsPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public enum Direction
    {
        Right,
        Left
    }

    private Direction _facing;

    public Direction Facing
    {
        get => _facing;
        private set
        {
            AssertMutable();
            _facing = value;
        }
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Target == null || cardPlay.Card.Owner != Owner.Player)
        {
            return;
        }

        await UpdateDirection(cardPlay.Target);
    }

    public override async Task BeforePotionUsed(PotionModel potion, Creature? target)
    {
        if (!CombatManager.Instance.IsInProgress || target == null || potion.Owner != Owner.Player)
        {
            return;
        }

        await UpdateDirection(target);
    }

    private async Task UpdateDirection(Creature target)
    {
        switch (Facing)
        {
            case Direction.Right:
                if (target.Monster is DoormakerLeft)
                {
                    await FaceDirection(Direction.Left);
                }
                break;
            case Direction.Left:
                if (target.Monster is DoormakerRight)
                {
                    await FaceDirection(Direction.Right);
                }
                break;
            default:
                await FaceDirection(Direction.Right);
                break;
        }
    }

    private async Task FaceDirection(Direction direction)
    {
        Facing = direction;
        Creature owner = Owner;
        var pets = Owner.Pets;
        var num = 0;
        var creatures = new Creature[1 + pets.Count];
        creatures[num] = owner;
        num++;
        foreach (Creature creature in pets)
        {
            creatures[num] = creature;
            num++;
        }
        foreach (var body in creatures.Select(c => NCombatRoom.Instance?.GetCreatureNode(c)?.Body))
        {
            await FlipScale(body);
        }
    }

    private Task FlipScale(Node2D? body)
    {
        if (body == null)
        {
            return Task.CompletedTask;
        }

        var x = body.Scale.X;
        if ((Facing == Direction.Right && x < 0f) || (Facing == Direction.Left && x > 0f))
        {
            body.Scale *= new Vector2(-1f, 1f);
        }
        return Task.CompletedTask;
    }
}