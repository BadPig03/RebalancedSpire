namespace RebalancedSpire.Core.Powers;

using Afflictions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

public sealed class WitheringPresencePlusPower : CustomPowerModel
{
    private const int MaxEnergy = 12;
    private const int WithersCount = 4;
    private const int WithersInitUpgradeLevel = 2;

    private bool _added;

    public bool Added
    {
        get => _added;
        set
        {
            AssertMutable();
            _added = value;
        }
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-scrutiny_plus_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-scrutiny_plus_power.png";

    public override int DisplayAmount => DynamicVars.Energy.IntValue;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
[
        new EnergyVar(12),
        new DynamicVar("PerLevel", 3)
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var list = new List<IHoverTip>();
            Wither wither = (Wither)ModelDb.Card<Wither>().MutableClone();
            for (var i = 0; i < WithersInitUpgradeLevel; i++)
            {
                wither.FakeUpgrade();
            }
            list.Add(HoverTipFactory.FromCard(wither));
            return list;
        }
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (!Added)
        {
            return;
        }

        foreach (var player in CombatState.Players)
        {
            var count = player.PlayerCombatState?.AllCards.Count(c => c is Wither);
            if (count is null or >= WithersCount)
            {
                continue;
            }

            var statusCards = new List<CardPileAddResult>();
            for (var i = 0; i < WithersCount - count; i++)
            {
                var wither = CombatState.CreateCard<Wither>(player);
                for (var j = 0; j < WithersInitUpgradeLevel; j++)
                {
                    wither.FakeUpgrade();
                }
                await CardCmd.Afflict<Withering>(wither, 1);
                statusCards.Add(await CardPileCmd.AddGeneratedCardToCombat(wither, PileType.Discard, null, CardPilePosition.Random));
            }
            if (!LocalContext.IsMe(player))
            {
                continue;
            }

            CardCmd.PreviewCardPileAdd(statusCards);
            await Cmd.Wait(1.2f);
        }
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner != Target?.Player || card.Type == CardType.Status || amount <= 0)
        {
            return;
        }

        DynamicVars.Energy.BaseValue -= amount;
        InvokeDisplayAmountChanged();
        if (DynamicVars.Energy.IntValue > 0)
        {
            return;
        }

        var player = card.Owner;
        var cards = player.PlayerCombatState?.AllCards;
        if (cards == null)
        {
            return;
        }

        Flash();
        var withers = new List<CardModel>();
        foreach (var allCard in cards)
        {
            if (allCard is not Wither wither)
            {
                continue;
            }

            wither.FakeUpgrade();
            withers.Add(wither);
        }
        DynamicVars.Energy.BaseValue += MaxEnergy;
        InvokeDisplayAmountChanged();
        if (!LocalContext.IsMe(player))
        {
            return;
        }

        CardCmd.Preview(withers);
        await Cmd.Wait(1.2f);
    }
}