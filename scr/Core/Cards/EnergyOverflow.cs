namespace RebalancedSpire.scr.Core.Cards;

using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

[Pool(typeof(NecrobinderCardPool))]
public sealed class EnergyOverflow() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, !Disabled)
{
    private static readonly bool Disabled = !RebalancedSpireConfig.EnergyOverflowConfig;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new ReadOnlyCollection<CardKeyword>(
    [
        CardKeyword.Exhaust
    ]);

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new EnergyVar(1)
    ]);

    public override IEnumerable<IHoverTip> ExtraHoverTips => new ReadOnlyCollection<IHoverTip>(
    [
        EnergyHoverTip
    ]);

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = cardPlay.Target?.GetPower<DoomPower>();
        if (power == null)
        {
            return;
        }

        var energy = power.Amount / 6;
        await PowerCmd.Remove(power);
        await PlayerCmd.GainEnergy(energy, cardPlay.Card.Owner);
    }

    public override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}