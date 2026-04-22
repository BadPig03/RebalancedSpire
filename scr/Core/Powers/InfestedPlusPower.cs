using System.Collections.ObjectModel;
using BaseLib.Abstracts;
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

namespace RebalancedSpire.scr.Core.Powers;

public sealed class InfestedPlusPower : CustomPowerModel
{
    private static int MaxAmount => 4;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new StringVar("PhrogParasite", ModelDb.Monster<PhrogParasite>().Title.GetFormattedText()),
    ]);

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
            Wriggler wriggler = (Wriggler)ModelDb.Monster<Wriggler>().ToMutable();
            wriggler.StartStunned = true;
            await CreatureCmd.Add(wriggler, CombatState, Owner.Side, PhrogParasiteElite.GetWrigglerSlotName(i));
        }
    }

    public override bool ShouldStopCombatFromEnding()
    {
        return true;
    }
}