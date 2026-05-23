namespace RebalancedSpire.Core.Enchantments;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

public sealed class Poisonous : CustomEnchantmentModel
{
    public override bool HasExtraCardText => true;

    public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new DamageVar(1, ValueProp.Move),
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
            var enemies = Card.CombatState?.HittableEnemies;
            if (enemies != null)
            {
                targets.AddRange(enemies);
            }
        }
        await PowerCmd.Apply<PoisonPower>(choiceContext, targets, DynamicVars.Poison.BaseValue, Card.Owner.Creature, Card);
    }

    public override decimal EnchantDamageAdditive(decimal originalDamage, ValueProp props)
    {
        return !props.IsPoweredAttack() ? 0 : DynamicVars.Damage.BaseValue;
    }
}