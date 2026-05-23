namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;

public sealed class ConsumingShadowPlusPower : CustomPowerModel
{
    private decimal _lastEvokedVal;

    private decimal LastEvokedVal
    {
        get => _lastEvokedVal;
        set
        {
            AssertMutable();
            _lastEvokedVal = value;
        }
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "res://images/powers/rebalancedspire-consuming_shadow_plus_power.png";

    public override string CustomBigIconPath => "res://images/powers/big/rebalancedspire-consuming_shadow_plus_power.png";

    public override int DisplayAmount => DynamicVars["Percent"].IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new IntVar("Percent", 0)
    }.AsReadOnly();

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this || power.Owner != Owner)
        {
            return Task.CompletedTask;
        }

        DynamicVars["Percent"].BaseValue += amount * 50;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if (orb.Owner != Owner.Player || orb is not DarkOrb darkOrb)
        {
            return Task.CompletedTask;
        }

        darkOrb._evokeVal += LastEvokedVal;
        return Task.CompletedTask;
    }

    public override Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if (orb.Owner != Owner.Player || orb is not DarkOrb darkOrb)
        {
            return Task.CompletedTask;
        }

        LastEvokedVal = darkOrb.EvokeVal * Amount * 0.5m;
        return Task.CompletedTask;
    }
}