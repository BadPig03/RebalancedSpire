using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace RebalancedSpire.scr.Core.Powers;

public sealed class PingPongPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new StringVar("LivingFog", ModelDb.Monster<LivingFog>().Title.GetFormattedText())
    ]);

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

        await CreatureCmd.Damage(choiceContext, Applier, creature.MaxHp, DamageProps.monsterMove, null, null);
    }
}