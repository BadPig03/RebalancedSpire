namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class WitheringPresencePlusPower : ModPowerTemplate
{
    private const int MaxEnergy = 12;
    private const int WithersInitUpgradeLevel = 2;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_scrutiny_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_scrutiny_power.png"
    );

    public override int DisplayAmount => DynamicVars.Energy.IntValue;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new EnergyVar(12),
        new DynamicVar("PerLevel", 3)
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> AdditionalHoverTips
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
        var cards = player.PlayerCombatState?.AllCards.ToList();
        if (cards == null)
        {
            return;
        }

        var count = 0;
        while (DynamicVars.Energy.IntValue <= 0)
        {
            count++;
            DynamicVars.Energy.BaseValue += MaxEnergy;
        }

        var withers = new List<CardModel>();
        foreach (var allCard in cards)
        {
            if (allCard is not Wither wither)
            {
                continue;
            }

            for (var i = 0; i < count; i++)
            {
                wither.FakeUpgrade();
            }
            withers.Add(wither);
        }

        Flash();
        InvokeDisplayAmountChanged();
        if (!LocalContext.IsMe(player))
        {
            return;
        }

        CardCmd.Preview(withers);
        await Cmd.Wait(1.2f);
    }
}