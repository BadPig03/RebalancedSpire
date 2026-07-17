namespace RebalancedSpire.Core.Powers;

using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class LongDistancePower : ModPowerTemplate
{
    private const int MaxAmount = 11;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_long_distance_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_long_distance_power.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new StringVar("TheInsatiable", ModelDb.Monster<TheInsatiable>().Title.GetFormattedText())
    ]).AsReadOnly();

    private static decimal CalculatePlayerMultiplier(int amount)
    {
        return Math.Max(0.2m, 1.3m - 0.1m * amount - 0.1m * Math.Max(0, amount - 5) + 0.2m * Math.Max(0, amount - 8));
    }

    private static decimal CalculateEnemyMultiplier(int amount)
    {
        return Math.Max(0.2m, 1.4m - 0.1m * amount - 0.1m * Math.Max(0, amount - 6) + 0.2m * Math.Max(0, amount - 9));
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == Owner && target.Player != null && props.IsPoweredAttack())
        {
            return CalculatePlayerMultiplier(Amount);
        }
        if ((dealer == Owner || dealer?.PetOwner == Owner.Player) && target?.Monster is TheInsatiable)
        {
            return CalculateEnemyMultiplier(Amount);
        }
        return 1;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner))
        {
            return;
        }

        await PowerCmd.TickDownDuration(this);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card is not FranticEscape)
        {
            return;
        }

        await PowerCmd.ModifyAmount(context, this, 1, Applier, null);
        if (Amount < MaxAmount)
        {
            return;
        }

        var room = (CombatRoom?) CombatState.RunState.CurrentRoom;
        if (room == null)
        {
            return;
        }

        var players = CombatState.Players.ToList();
        foreach (var player in players)
        {
            Node2D? body = NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.Body;
            if (body != null)
            {
                body.Scale *= new Vector2(-1f, 1f);
            }
            room.AddExtraReward(player, new PotionReward(player));
            room.AddExtraReward(player, new RelicReward(RelicRarity.Rare, player));
        }

        await Cmd.Wait(1f);
        var enemies = CombatState.Enemies.Where(c => c.Monster is TheInsatiable).ToList();
        foreach (var enemy in enemies)
        {
            enemy.RemoveAllPowersInternalExcept();
            CombatManager.Instance.RemoveCreature(enemy);
            enemy.CombatState?.RemoveCreature(enemy);
        }
    }
}