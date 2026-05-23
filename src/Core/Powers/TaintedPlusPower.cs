namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;

public sealed class TaintedPlusPower : CustomPowerModel
{
    private static readonly SpireField<Tainted, bool> AppliedExhaust = new(() => false);

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-tainted_plus_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-tainted_plus_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new StringVar("AfflictionTitle", ModelDb.Affliction<Tainted>().Title.GetFormattedText())
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromAffliction<Tainted>();

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player != Owner.Player || cardPlay.Card.Type is not (CardType.Attack or CardType.Skill))
        {
            return;
        }

        var cards = PileType.Hand.GetPile(player).Cards.Where(c => !c.Keywords.Contains(CardKeyword.Unplayable) && c.Type is CardType.Attack or CardType.Skill && c.Affliction == null);
        var selected = player.RunState.Rng.CombatCardSelection.NextItem(cards);
        if (selected == null)
        {
            return;
        }

        Flash();
        var tainted = await CardCmd.Afflict<Tainted>(selected, 1);
        if (tainted == null || selected.Keywords.Contains(CardKeyword.Exhaust))
        {
            return;
        }

        CardCmd.ApplyKeyword(selected, CardKeyword.Exhaust);
        AppliedExhaust.Set(tainted, true);
    }

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner.Creature != Owner || card.Affliction is not Tainted)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = 0;
        return true;
    }

    public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
        {
            return Task.CompletedTask;
        }

        var allCards = Owner.Player?.PlayerCombatState?.AllCards;
        if (allCards == null)
        {
            return Task.CompletedTask;
        }

        foreach (var card in allCards)
        {
            var tainted = (Tainted?) card.Affliction;
            if (tainted == null)
            {
                continue;
            }

            if (AppliedExhaust.Get(tainted))
            {
                CardCmd.RemoveKeyword(card, CardKeyword.Exhaust);
            }
            CardCmd.ClearAffliction(card);
        }
        return Task.CompletedTask;
    }
}