namespace RebalancedSpire.scr.Core.Powers;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

public sealed class CorpseExplosionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != Owner)
        {
            return;
        }

        var enemies = creature.CombatState?.HittableEnemies.ToList();
        if (enemies == null)
        {
            return;
        }

        var damageVar = new DamageVar(creature.MaxHp * Amount, DamageProps.cardUnpowered);
        foreach (var enemy in enemies)
        {
            await CreatureCmd.Damage(choiceContext, enemy, damageVar, Applier, null);
        }
    }
}