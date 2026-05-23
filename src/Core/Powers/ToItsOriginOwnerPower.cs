namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

public sealed class ToItsOriginOwnerPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-to_its_origin_owner_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-to_its_origin_owner_power.png";

    public override bool ShouldPlayVfx => false;

    protected override bool IsVisibleInternal => false;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var player = applier?.Player;
        if (applier == null || player == null)
        {
            return Task.CompletedTask;
        }

        var room = (CombatRoom?) player.RunState.CurrentRoom;
        room?.AddExtraReward(player, new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player));
        return Task.CompletedTask;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        var eggList = Owner.Player?.PlayerCombatState?.AllCards.Where(c => c is ByrdonisEgg).ToList();
        if (eggList == null || eggList.Count == 0)
        {
            return;
        }

        foreach (var egg in eggList)
        {
            await CardPileCmd.RemoveFromCombat(egg);
            if (egg.DeckVersion == null)
            {
                continue;
            }

            await CardPileCmd.RemoveFromDeck(egg.DeckVersion);
        }
    }
}