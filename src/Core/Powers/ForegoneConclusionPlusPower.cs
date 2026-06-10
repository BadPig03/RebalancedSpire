namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class ForegoneConclusionPlusPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_foregone_conclusion_plus_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_foregone_conclusion_plus_power.png"
    );

    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null || !Hook.ShouldFlush(combatState, player))
        {
            return;
        }

        var prompt = new LocString("card_selection", "REBALANCEDSPIRE_TO_PUT_ON_TOP");
        var selected = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(Owner.Player), Owner.Player, new CardSelectorPrefs(prompt, Amount))).ToList();
        await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top, null, true);
    }
}