namespace RebalancedSpire.Core.Cards;

using Configs;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterCard(typeof(SilentCardPool))]
[UsedImplicitly]
public sealed class CorpseExplosion() : ModCardTemplate(1, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy, !Disabled)
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DustyTome;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new PowerVar<PoisonPower>(6),
        new PowerVar<CorpseExplosionPower>(1)
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ]).AsReadOnly();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
        {
            return;
        }

        await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, DynamicVars.Poison.BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<CorpseExplosionPower>(choiceContext, cardPlay.Target, DynamicVars["CorpseExplosionPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(3);
    }
}