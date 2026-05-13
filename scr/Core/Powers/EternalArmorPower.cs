namespace RebalancedSpire.scr.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

public sealed class EternalArmorPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}