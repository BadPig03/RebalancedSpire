namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Combat.HandSize;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class InfiniteBladesPlusPower : ModPowerTemplate, IMaxHandSizeModifier, IPowerExtraIconAmountLabelSpecsProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_infinite_blades_plus_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_infinite_blades_plus_power.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new CardsVar(0)
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.FromCard<Shiv>()
    ]).AsReadOnly();

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner.Player)
        {
            return;
        }

        Flash();
        await Shiv.CreateInHand(Owner.Player, Amount, combatState);
    }

    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        if (player != Owner.Player)
        {
            return currentMaxHandSize;
        }

        var shivs = Math.Min(DynamicVars.Cards.IntValue, PileType.Hand.GetPile(player).Cards.OfType<Shiv>().Count());
        return currentMaxHandSize + shivs;
    }

    public IReadOnlyList<ExtraIconAmountLabelSpec> GetPowerExtraIconAmountLabelSpecs()
    {
        return new List<ExtraIconAmountLabelSpec>([
            ExtraIconAmountLabelSpec.Plain(ExtraIconAmountLabelCorner.TopRight, DynamicVars.Cards.IntValue.ToString())
        ]).AsReadOnly();
    }
}