namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

public sealed class SicEmPlusPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.Static(StaticHoverTip.SummonStatic)
    ]).AsReadOnly();

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer?.Monster is not Osty osty || osty.Creature.PetOwner == null || Applier == null || osty.Creature.PetOwner.Creature != Applier || target != Owner)
        {
            return;
        }

        await OstyCmd.Summon(choiceContext, osty.Creature.PetOwner, Amount, this);
    }
}