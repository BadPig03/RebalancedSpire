namespace RebalancedSpire.Core.Cards;

using Configs;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterCard(typeof(IroncladCardPool))]
[UsedImplicitly]
public sealed class LimitBreak() : ModCardTemplate(1, CardType.Skill, CardRarity.Ancient, TargetType.Self, !Disabled)
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DustyTome;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new List<CardKeyword>(
    [
        CardKeyword.Exhaust
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new List<IHoverTip>(
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
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, power.Amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}