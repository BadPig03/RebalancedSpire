namespace RebalancedSpire.Core.GameActions;

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;

public sealed class UseSealOfGoldAction(Player player, int index) : GameAction
{
    public override ulong OwnerId => Player.NetId;

    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    private Player Player => player;

    private int Index => index;

    protected override async Task ExecuteAction()
    {
        if (!CombatManager.Instance.IsInProgress || index < 0 || Player.Relics.Count < index + 1)
        {
            return;
        }

        var sealOfGold = Player.Relics[index];
        if (sealOfGold is not SealOfGold || sealOfGold.Status != RelicStatus.Active)
        {
            return;
        }

        var gold = sealOfGold.DynamicVars.Gold.IntValue;
        if (Player.Gold < gold)
        {
            return;
        }

        sealOfGold.Flash();
        sealOfGold.Status = RelicStatus.Normal;
        await PlayerCmd.GainEnergy(sealOfGold.DynamicVars.Energy.BaseValue, Player);
        await PlayerCmd.LoseGold(gold, Player);
    }

    public override INetAction ToNetAction()
    {
        return new NetUseSealOfGoldAction
        {
            Index = Index
        };
    }
}