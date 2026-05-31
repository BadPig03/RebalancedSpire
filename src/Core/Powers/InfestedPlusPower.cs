namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.TestSupport;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class InfestedPlusPower : ModPowerTemplate
{
    private const int MaxAmount = 4;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://images/powers/rebalanced_spire_power_infested_plus_power.png",
        BigIconPath: "res://images/powers/rebalanced_spire_power_infested_plus_power.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new StringVar("PhrogParasite", ModelDb.Monster<PhrogParasite>().Title.GetFormattedText()),
    ]).AsReadOnly();

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || Owner != target)
        {
            return;
        }

        await Cmd.CustomScaledWait(deathAnimLength, deathAnimLength);
        if (TestMode.IsOff)
        {
            NRunMusicController.Instance?.TriggerEliteSecondPhase();
        }
        for (var i = 0; i < Math.Min(MaxAmount, Amount); i++)
        {
            Wriggler wriggler = (Wriggler) ModelDb.Monster<Wriggler>().ToMutable();
            wriggler.StartStunned = true;
            await CreatureCmd.Add(wriggler, CombatState, Owner.Side, PhrogParasiteElite.GetWrigglerSlotName(i));
        }
    }

    public override bool ShouldStopCombatFromEnding()
    {
        return true;
    }
}