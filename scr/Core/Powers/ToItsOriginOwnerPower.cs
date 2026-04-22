using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace RebalancedSpire.scr.Core.Powers;

public sealed class ToItsOriginOwnerPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool IsVisibleInternal => false;

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
        CardModel? card;
        var allCards = Owner.Player?.PlayerCombatState?.AllCards;
        if (Owner.Player == null || allCards == null)
        {
            card = null;
        }
        else
        {
            CardModel? tempCard = null;
            foreach (CardModel cardModel in allCards)
            {
                if (cardModel is not ByrdonisEgg)
                {
                    continue;
                }

                tempCard = cardModel;
                break;
            }
            card = tempCard;
        }
        if (card == null)
        {
            return;
        }

        await CardPileCmd.RemoveFromCombat(card);
        if (card.DeckVersion == null)
        {
            return;
        }

        await CardPileCmd.RemoveFromDeck(card.DeckVersion);
    }
}