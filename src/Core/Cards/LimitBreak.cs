namespace RebalancedSpire.Core.Cards;

using BaseLib.Abstracts;
using BaseLib.Utils;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;

[Pool(typeof(IroncladCardPool))]
[UsedImplicitly]
public sealed class LimitBreak() : CustomCardModel(1, CardType.Skill, CardRarity.Ancient, TargetType.Self, !Disabled)
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DustyTomeConfig;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new List<CardKeyword>(
    [
        CardKeyword.Exhaust
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ]).AsReadOnly();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var power = Owner.Creature.GetPower<StrengthPower>();
        if (power == null)
        {
            return;
        }

        NPowerUpVfx.CreateNormal(Owner.Creature);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, power.Amount, Owner.Creature, play.Card);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}