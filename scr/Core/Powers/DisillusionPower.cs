namespace RebalancedSpire.scr.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

public sealed class DisillusionPower : CustomPowerModel
{
    private static int StrengthPowerAmount => 5;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    public override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<StrengthPower>(StrengthPowerAmount)
    };

    public override async Task BeforeDeath(Creature creature)
    {
        if (creature != Owner)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(Owner, -DynamicVars.Strength.IntValue, creature, null);
    }
}