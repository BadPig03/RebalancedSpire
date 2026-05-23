namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

public sealed class LeechingHugPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-leeching_hug_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-leeching_hug_power.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ]).AsReadOnly();

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new StringVar("Slimed", ModelDb.Card<Slimed>().Title),
        new StringVar("SlimedBerserker", ModelDb.Monster<SlimedBerserker>().Title.GetFormattedText()),
        new PowerVar<StrengthPower>(1),
        new HealVar(5)
    ]).AsReadOnly();

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Slimed)
        {
            return;
        }

        Flash();
        foreach (var creature in CombatState.Enemies.Where(c => c.Monster is SlimedBerserker))
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), creature, DynamicVars.Strength.BaseValue, cardPlay.Card.Owner.Creature, null);
            await CreatureCmd.Heal(creature, DynamicVars.Heal.BaseValue);
        }
    }
}