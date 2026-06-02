namespace RebalancedSpire.Core.Enchantments;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterEnchantment]
public sealed class Poisonous : ModEnchantmentTemplate
{
    public override bool HasExtraCardText => true;

    public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new PowerVar<PoisonPower>(2)
    ]).AsReadOnly();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ]).AsReadOnly();

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        var targets = new List<Creature>();
        if (Card.TargetType != TargetType.AllEnemies && cardPlay?.Target != null)
        {
            targets.Add(cardPlay.Target);
        }
        else
        {
            targets.AddRange(Card.CombatState?.HittableEnemies ?? []);
        }
        await PowerCmd.Apply<PoisonPower>(choiceContext, targets, DynamicVars.Poison.BaseValue, Card.Owner.Creature, Card);
    }
}