namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class CorpseExplosionPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_corpse_explosion_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_corpse_explosion_power.png"
    );

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != Owner)
        {
            return;
        }

        var enemies = Owner.CombatState?.HittableEnemies.ToList();
        if (enemies == null)
        {
            return;
        }

        foreach (var enemy in enemies)
        {
            await CreatureCmd.Damage(choiceContext, enemy, creature.MaxHp * Amount, DamageProps.nonCardHpLoss, null!);
        }
    }
}