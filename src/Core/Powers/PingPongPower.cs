namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class PingPongPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_ping_pong_power.png",
        BigIconPath: "res://images/powers/big/rebalanced_spire_power_ping_pong_power.png"
    );

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    public override bool ShouldPlayVfx => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new StringVar("LivingFog", ModelDb.Monster<LivingFog>().Title.GetFormattedText())
    ]).AsReadOnly();

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        Amount = Owner.MaxHp;
        return Task.CompletedTask;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != Owner || Applier == null)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, Applier, creature.MaxHp, DamageProps.monsterMove, Applier);
    }
}