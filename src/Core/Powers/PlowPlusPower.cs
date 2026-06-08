namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class PlowPlusPower : ModPowerTemplate
{
    private const int PlowedPowerAmount = 1;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_plow_plus_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_plow_plus_power.png"
    );

    public override bool ShouldPlayVfx => false;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.Static(StaticHoverTip.Stun),
        HoverTipFactory.FromPower<StrengthPower>()
    ]).AsReadOnly();

    public override bool ShouldScaleInMultiplayer => true;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0 || target.CurrentHp > Amount || Owner.Monster is not CeremonialBeast beast)
        {
            return;
        }

        Flash();
        await PowerCmd.Remove<StrengthPower>(Owner);
        await beast.SetStunned();
        await CreatureCmd.Stun(Owner, beast.StunnedMove, target.HasPower<PlowedPower>() ? "BEAST_CRY_MOVE" : "SECOND_STAMP_MOVE");
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<PlowedPower>(choiceContext, Owner, PlowedPowerAmount, Owner, null);
    }
}