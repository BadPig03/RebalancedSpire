namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class ConsumingShadowPlusPower : ModPowerTemplate
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

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_consuming_shadow_plus_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_consuming_shadow_plus_power.png"
    );

    public override int DisplayAmount => DynamicVars["Percent"].IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new ("Percent", 0)
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

        Flash();
        darkOrb._evokeVal += LastEvokedVal;
        return Task.CompletedTask;
    }

    public override Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if (orb.Owner != Owner.Player || orb is not DarkOrb darkOrb)
        {
            return Task.CompletedTask;
        }

        Flash();
        LastEvokedVal = darkOrb.EvokeVal * Amount * 0.5m;
        return Task.CompletedTask;
    }
}