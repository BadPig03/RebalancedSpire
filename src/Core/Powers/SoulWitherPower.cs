namespace RebalancedSpire.Core.Powers;

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
public sealed class SoulWitherPower : ModPowerTemplate
{
    private const int MaxHitCount = 12;

    private int _hitCount;

    public int HitCount
    {
        get => _hitCount;
        set
        {
            AssertMutable();
            _hitCount = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_soul_wither_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_soul_wither_power.png"
    );

    public override int DisplayAmount => HitCount;

    public override bool ShouldPlayVfx => false;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new DynamicVar("MaxCount", MaxHitCount),
        new StringVar("SoulNexus", ModelDb.Monster<SoulNexus>().Title.GetFormattedText())
    ]).AsReadOnly();

    public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || result.UnblockedDamage <= 0)
        {
            return Task.CompletedTask;
        }

		var instances = Owner.GetPowerInstances<SoulWitherPower>().ToList();
        foreach (var power in instances)
        {
            if ((target.Player ?? target.PetOwner) != power.Target?.Player)
            {
                continue;
            }

            Flash();
            HitCount += 1;
        }
        return Task.CompletedTask;
    }

    public bool IsLimitExceeded()
    {
        return HitCount >= MaxHitCount;
    }
}