namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class WellLaidPlansPlusPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_well_laid_plans_plus_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_well_laid_plans_plus_power.png"
    );

    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (player != Owner.Player || !Hook.ShouldFlush(combatState, player))
        {
            return;
        }

        var list = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount), context: choiceContext, player: Owner.Player, filter: c => !c.ShouldRetainThisTurn, source: this)).ToList();
        if (list.Count == 0)
        {
            return;
        }

        foreach (CardModel item in list)
        {
            item.GiveSingleTurnRetain();
        }
    }
}