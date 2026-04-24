using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RebalancedSpire.scr.Core.Powers;

public sealed class PlowPlusPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override IEnumerable<IHoverTip> ExtraHoverTips => new ReadOnlyCollection<IHoverTip>(
    [
        HoverTipFactory.Static(StaticHoverTip.Stun),
        HoverTipFactory.FromPower<StrengthPower>()
    ]);

    public override bool ShouldScaleInMultiplayer => true;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0 || target.CurrentHp > Amount || Owner.Monster is not CeremonialBeast monster)
        {
            return;
        }

        Flash();
        await PowerCmd.Remove<StrengthPower>(Owner);
        await monster.SetStunned();
        await CreatureCmd.Stun(Owner, monster.StunnedMove, target.GetPower<PlowedPower>() != null ? "BEAST_CRY_MOVE" : "SECOND_STAMP_MOVE");
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<PlowedPower>(new ThrowingPlayerChoiceContext(), target, 1, target, null);
    }
}