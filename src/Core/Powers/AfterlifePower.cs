namespace RebalancedSpire.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

public sealed class AfterlifePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new HealVar(Amount),
        new SummonVar(Amount - 1)
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon)
    ]).AsReadOnly();

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this || power.Owner != Owner)
        {
            return Task.CompletedTask;
        }

        DynamicVars.Heal.BaseValue = Amount;
        DynamicVars.Summon.BaseValue = Amount - 1;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterEnergyResetLate(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        if (player.IsOstyMissing || player.Osty == null)
        {
            await OstyCmd.Summon(new ThrowingPlayerChoiceContext(), player, DynamicVars.Summon.BaseValue, this);
        }
        else
        {
            await CreatureCmd.Heal(player.Osty, DynamicVars.Heal.BaseValue);
        }
    }
}