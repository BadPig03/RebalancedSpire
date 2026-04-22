using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace RebalancedSpire.scr.Core.Powers;

using MegaCrit.Sts2.Core.GameActions.Multiplayer;

public sealed class SoulWitherPower : CustomPowerModel
{
    private const int MaxHitCount = 12;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => HitCount;

    public override bool IsInstanced => true;

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new StringVar("SoulNexus", ModelDb.Monster<SoulNexus>().Title.GetFormattedText()),
        new CalculationBaseVar(MaxHitCount)
    ]);

    private int _hitCount;

    private int HitCount
    {
        get => _hitCount;
        set
        {
            AssertMutable();
            _hitCount = value;
            DynamicVars.CalculationBase.BaseValue = Math.Max(0, MaxHitCount - value);
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != Owner || !target.IsPlayer || !props.IsPoweredAttack() || result.UnblockedDamage <= 0)
        {
            return Task.CompletedTask;
        }

        Flash();
        HitCount += 1;
        return Task.CompletedTask;
    }

    public bool ExceedLimit()
    {
        return HitCount >= MaxHitCount;
    }

    public void Reset()
    {
        HitCount = 0;
    }
}