namespace RebalancedSpire.Core.Dominions;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Dread;
using MegaCrit.Sts2.Core.Rooms;
using Powers;

public sealed class RebalancedSpireDoormakerDominion : DominionModel
{
    private const int ScrutinyAmount = 4;

    public override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromPowerWithPowerHoverTips<ScrutinyPower>(ScrutinyAmount);

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        return room is CombatRoom { Encounter.RoomType: RoomType.Boss } ? Task.CompletedTask : base.AfterRoomEntered(room);
    }

    protected override async Task AfterPlayersEnteredCombat(IReadOnlyList<Player> players)
    {
        await PowerCmd.Apply<ScrutinyPower>(new ThrowingPlayerChoiceContext(), players.Select(player => player.Creature), ScrutinyAmount, null, null);
    }
}
