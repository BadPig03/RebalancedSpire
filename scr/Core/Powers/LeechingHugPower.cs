using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RebalancedSpire.scr.Core.Powers;

public sealed class LeechingHugPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override IEnumerable<IHoverTip> ExtraHoverTips => new ReadOnlyCollection<IHoverTip>(
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ]);

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new StringVar("Slimed", ModelDb.Card<Slimed>().Title),
        new StringVar("SlimedBerserker", ModelDb.Monster<SlimedBerserker>().Title.GetFormattedText()),
        new PowerVar<StrengthPower>(1),
        new HealVar(5)
    ]);

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Slimed)
        {
            return;
        }

        var enumerable = CombatState.Enemies.Where(c => c.Monster is SlimedBerserker);
        foreach (Creature creature in enumerable)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), creature, DynamicVars.Strength.IntValue, creature, null);
            await CreatureCmd.Heal(creature, DynamicVars.Heal.IntValue);
        }
    }
}