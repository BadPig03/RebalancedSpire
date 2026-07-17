namespace RebalancedSpire.Core.Powers;

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
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

[RegisterPower]
public sealed class TaintedPlusPower : ModPowerTemplate
{
    private static readonly AttachedState<Tainted, bool> AppliedExhaust = new(() => false);

    private int _cardsPlayed = 3;

    private int CardsPlayed
    {
        get => _cardsPlayed;
        set
        {
            AssertMutable();
            _cardsPlayed = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_tainted_plus_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_tainted_plus_power.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new StringVar("AfflictionTitle", ModelDb.Affliction<Tainted>().Title.GetFormattedText()),
        new CardsVar(3)
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromAffliction<Tainted>();

    public override int DisplayAmount => CardsPlayed;

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player != Owner.Player || cardPlay.Card.Type is not (CardType.Attack or CardType.Skill) || CardsPlayed <= 0)
        {
            return;
        }

        CardsPlayed--;
        Flash();
        var cards = PileType.Hand.GetPile(player).Cards.Where(c => !c.Keywords.Contains(CardKeyword.Unplayable) && c.Type is CardType.Attack or CardType.Skill && c.Affliction == null);
        var selected = player.RunState.Rng.CombatCardSelection.NextItem(cards);
        if (selected == null)
        {
            return;
        }

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

        var allCards = Owner.Player?.PlayerCombatState?.AllCards.ToList();
        if (allCards == null)
        {
            return Task.CompletedTask;
        }

        Flash();
        CardsPlayed = DynamicVars.Cards.IntValue;
        foreach (var card in allCards)
        {
            var tainted = (Tainted?) card.Affliction;
            if (tainted == null)
            {
                continue;
            }

            if (AppliedExhaust.GetValueOrDefault(tainted, false))
            {
                CardCmd.RemoveKeyword(card, CardKeyword.Exhaust);
            }
            CardCmd.ClearAffliction(card);
        }
        return Task.CompletedTask;
    }
}