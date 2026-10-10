namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class MasterPlannerPlusPower : ModPowerTemplate
{
    private int _pendingDraws;
    private int _slyInFlight;
    private bool _drawing;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_master_planner_plus_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_master_planner_plus_power.png"
    );

    public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.Creature != Owner || !card.IsSlyThisTurn)
        {
            return Task.CompletedTask;
        }

        _pendingDraws += Amount;
        return Task.CompletedTask;
    }

    public override Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type)
    {
        if (type != AutoPlayType.SlyDiscard || card.Owner.Creature != Owner)
        {
            return Task.CompletedTask;
        }

        _slyInFlight++;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_slyInFlight > 0)
        {
            _slyInFlight--;
        }

        var player = Owner.Player;
        if (_slyInFlight > 0 || _drawing || _pendingDraws <= 0 || player == null)
        {
            return;
        }

        var pendingDraws = _pendingDraws;
        _pendingDraws = 0;
        _drawing = true;
        Flash();
        await CardPileCmd.Draw(choiceContext, pendingDraws, player);
        _drawing = false;
    }
}