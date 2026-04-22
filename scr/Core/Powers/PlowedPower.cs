using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace RebalancedSpire.scr.Core.Powers;

public sealed class PlowedPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool IsVisibleInternal => false;
}