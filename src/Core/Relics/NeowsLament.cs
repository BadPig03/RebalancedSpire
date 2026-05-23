namespace RebalancedSpire.Core.Relics;

using BaseLib.Abstracts;
using BaseLib.Utils;
using Configs;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

[Pool(typeof(SharedRelicPool))]
public sealed class NeowsLament() : CustomRelicModel(!Disabled)
{
    private static readonly bool Disabled = !RebalancedSpireConfig.NeowsLamentConfig;

    private const int MaxCount = 3;

    private int _timesUsed;

    [SavedProperty]
    private int TimesUsed
    {
        get => _timesUsed;
        set
        {
            AssertMutable();
            _timesUsed = value;
            DynamicVars["Rooms"].BaseValue = MaxCount - _timesUsed;
            InvokeDisplayAmountChanged();
            CheckIfUsedUp();
        }
    }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool IsUsedUp => TimesUsed >= MaxCount;

    public override bool ShowCounter => !IsUsedUp;

    public override int DisplayAmount => MaxCount - TimesUsed;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new DynamicVar("Rooms", MaxCount)
    ]).AsReadOnly();

    public override async Task BeforeCombatStart()
    {
        if (IsUsedUp)
        {
            return;
        }

        var enemies = Owner.Creature.CombatState?.HittableEnemies.ToList();
        if (enemies == null)
        {
            return;
        }

        Flash();
        VfxCmd.PlayOnCreatureCenters(enemies, "vfx/vfx_bite");
        foreach (var creature in enemies)
        {
            await CreatureCmd.SetCurrentHp(creature, 1);
        }
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (IsUsedUp)
        {
            return Task.CompletedTask;
        }

        Flash();
        TimesUsed++;
        return Task.CompletedTask;
    }

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (IsUsedUp || creature.Side != CombatSide.Enemy)
        {
            return;
        }

        Flash();
        VfxCmd.PlayOnCreatureCenter(creature, "vfx/vfx_bite");
        await CreatureCmd.SetCurrentHp(creature, 1);
    }

    private void CheckIfUsedUp()
    {
        if (!IsUsedUp)
        {
            return;
        }

        Status = RelicStatus.Disabled;
    }
}