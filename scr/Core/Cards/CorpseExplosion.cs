namespace RebalancedSpire.scr.Core.Cards;

using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using BaseLib.Utils;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Powers;

[Pool(typeof(SilentCardPool))]
[UsedImplicitly]
public sealed class CorpseExplosion() : CustomCardModel(2, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy, !Disabled)
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DustyTomeConfig;

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new PowerVar<PoisonPower>(6),
        new PowerVar<CorpseExplosionPower>(1)
    ]);

    public override IEnumerable<IHoverTip> ExtraHoverTips => new ReadOnlyCollection<IHoverTip>(
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ]);

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
        {
            return;
        }

        await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, DynamicVars.Poison.IntValue, Owner.Creature, this);
        await PowerCmd.Apply<CorpseExplosionPower>(choiceContext, cardPlay.Target, DynamicVars["CorpseExplosionPower"].IntValue, Owner.Creature, this);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(3);
    }
}