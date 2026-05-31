namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class ToItsOriginOwnerPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_to_its_origin_owner_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_to_its_origin_owner_power.png"
    );

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