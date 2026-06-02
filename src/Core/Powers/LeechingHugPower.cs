namespace RebalancedSpire.Core.Powers;

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
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class LeechingHugPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_leeching_hug_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_leeching_hug_power.png"
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new List<IHoverTip>(
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
        var scale = CombatState.Players.Count;
        var enemies = CombatState.Enemies.Where(c => c.Monster is SlimedBerserker).ToList();
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), enemy, DynamicVars.Strength.BaseValue, cardPlay.Card.Owner.Creature, null);
            await CreatureCmd.Heal(enemy, DynamicVars.Heal.BaseValue * scale);
        }
    }
}