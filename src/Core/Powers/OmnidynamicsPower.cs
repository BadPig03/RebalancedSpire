namespace RebalancedSpire.Core.Powers;

using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Monsters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class OmnidynamicsPower : ModPowerTemplate
{
    private enum Direction
    {
        Right,
        Left
    }

    private Direction _facing;

    private Direction Facing
    {
        get => _facing;
        set
        {
            AssertMutable();
            _facing = value;
        }
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_omnidynamics_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_omnidynamics_power.png"
    );

    public override bool ShouldPlayVfx => false;

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
        var creatures = new Creature[1 + pets.Count];
        var num = 0;
        creatures[num] = owner;
        num++;
        foreach (var creature in pets)
        {
            creatures[num] = creature;
            num++;
        }

        var bodies = creatures.Select(c => NCombatRoom.Instance?.GetCreatureNode(c)?.Body).ToList();
        foreach (var body in bodies)
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