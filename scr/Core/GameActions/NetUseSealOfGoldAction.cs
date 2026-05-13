namespace RebalancedSpire.scr.Core.GameActions;

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

public struct NetUseSealOfGoldAction : INetAction
{
    public int Index;

    public GameAction ToGameAction(Player player)
    {
        return new UseSealOfGoldAction(player, Index);
    }

    public void Serialize(PacketWriter writer)
    {
        writer.WriteInt(Index);
    }

    public void Deserialize(PacketReader reader)
    {
        Index = reader.ReadInt();
    }
}