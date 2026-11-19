namespace RebalancedSpire.Core.Cards;

using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterCard(typeof(EventCardPool))]
[UsedImplicitly]
public sealed class GodByrdAwakened() : ModCardTemplate(0, CardType.Power, CardRarity.Event, TargetType.Self, false)
{
    public override string Title => ModelDb.Card<GodByrd>().TitleLocString.GetFormattedText() + (IsUpgraded ? "+" : "");

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: ModelDb.Card<GodByrd>().PortraitPath,
        BetaPortraitPath: ModelDb.Card<GodByrd>().BetaPortraitPath
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new DamageVar(30, ValueProp.Unpowered)
    ]).AsReadOnly();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GodByrdAwakenedPower>(choiceContext, Owner.Creature, DynamicVars.Damage.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
    }
}
