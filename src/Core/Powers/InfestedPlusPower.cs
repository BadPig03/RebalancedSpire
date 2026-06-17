namespace RebalancedSpire.Core.Powers;

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class InfestedPlusPower : ModPowerTemplate
{
    private const int MaxAmount = 4;
    private const float IncreasedSize = 0.2f;
    private const float InitSize = 0.4f;
    private const float MaxHpRatio = 0.2f;

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

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(Owner);
        if (node == null)
        {
            return Task.CompletedTask;
        }

        node._scaleTween = node.CreateTween();
        node._scaleTween.TweenProperty(node.Visuals, "scale", Vector2.One * InitSize, 0).SetTrans(Tween.TransitionType.Expo);
        node._scaleTween.TweenCallback(Callable.From(() => { node.UpdateBounds(node.Visuals); }));
        return Task.CompletedTask;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this || applier != Owner)
        {
            return;
        }

        var node = NCombatRoom.Instance?.GetCreatureNode(Owner);
        if (node == null)
        {
            return;
        }

        node._scaleTween = node.CreateTween();
        node._scaleTween.TweenProperty(node.Visuals, "scale", Vector2.One * (InitSize + (Amount - 1) * IncreasedSize), 0.75f).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        node._scaleTween.TweenCallback(Callable.From(() => { node.UpdateBounds(node.Visuals); }));
        if (Amount <= 2)
        {
            return;
        }

        await CreatureCmd.GainMaxHp(Owner, (int) (Owner.MaxHp * MaxHpRatio));
    }

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