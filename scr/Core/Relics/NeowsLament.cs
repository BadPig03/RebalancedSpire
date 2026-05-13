using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace RebalancedSpire.scr.Core.Relics;

using MegaCrit.Sts2.Core.Rooms;

[Pool(typeof(SharedRelicPool))]
public sealed class NeowsLament : CustomRelicModel
{
    private static int MaxUsableCount => 3;

    private int _timesUsed;

    [SavedProperty]
    private int TimesUsed
    {
        get => _timesUsed;
        set
        {
            AssertMutable();
            _timesUsed = value;
            DynamicVars["Rooms"].BaseValue = MaxUsableCount - _timesUsed;
            InvokeDisplayAmountChanged();
            CheckIfUsedUp();
        }
    }

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool IsUsedUp => TimesUsed >= MaxUsableCount;

    public override bool ShowCounter => !IsUsedUp;

    public override int DisplayAmount => MaxUsableCount - TimesUsed;

    public override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new DynamicVar("Rooms", MaxUsableCount)
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