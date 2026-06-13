namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner.Player);
        var selected = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(Owner.Player), Owner.Player, new CardSelectorPrefs(new LocString("card_selection", "REBALANCEDSPIRE_TO_PUT_ON_TOP"), Amount))).ToList();
        await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top, null, true);
    }
}