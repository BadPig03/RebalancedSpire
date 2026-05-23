namespace RebalancedSpire.Core.Monsters;

using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using Powers;

public abstract class DoormakerBase : CustomMonsterModel
{
    public const string ClosedState = "monsters/beta/door_maker_placeholder_1.png";
    public const string EyeState = "monsters/beta/door_maker_placeholder_2.png";
    public const string MouthState = "monsters/beta/door_maker_placeholder_3.png";

    private DoormakerBase? _otherDoormaker;

    private int _originalMaxHp;

    private int _originalHp;

    private MoveState? _dramaticOpenState;

    private readonly List<PowerModel> _powerModels = [];

    protected DoormakerBase OtherDoormaker
    {
        get => _otherDoormaker ?? throw new InvalidOperationException();
        set
        {
            AssertMutable();
            _otherDoormaker = value;
        }
    }

    protected MoveState DramaticOpenState
    {
        get => _dramaticOpenState ?? throw new InvalidOperationException();
        set
        {
            AssertMutable();
            _dramaticOpenState = value;
        }
    }

    private int OriginalMaxHp
    {
        get => _originalMaxHp;
        set
        {
            AssertMutable();
            _originalMaxHp = value;
        }
    }

    private int OriginalHp
    {
        get => _originalHp;
        set
        {
            AssertMutable();
            _originalHp = value;
        }
    }

    public override int MinInitialHp => 400;

    public override int MaxInitialHp => MinInitialHp;

    public override bool CanChangeScale => false;

    public override LocString Title
    {
        get
        {
            if (_creature == null)
            {
                return L10NMonsterLookup("DOOR.name");
            }
            return Creature.HpDisplay == HpDisplay.InfiniteWithoutNumbers ? L10NMonsterLookup("DOOR.name") : L10NMonsterLookup("DOORMAKER.name");
        }
    }

    public override string CustomVisualPath => "scenes/creature_visuals/doormaker_boss.tscn";

    public override NCreatureVisuals CreateCustomVisuals()
    {
        return NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/doormaker_boss.tscn");
    }

    public override async Task AfterAddedToRoom()
    {
        UpdateVisual(ClosedState);
        OriginalMaxHp = Creature.MaxHp;
        OriginalHp = Creature.CurrentHp;
        await CreatureCmd.SetMaxAndCurrentHp(Creature, 999999999);
        Creature.HpDisplay = HpDisplay.InfiniteWithoutNumbers;
    }

    public override bool ShouldAllowHitting(Creature creature)
    {
        if (_otherDoormaker == null || creature.Monster is not DoormakerBase doormakerBase)
        {
            return base.ShouldAllowHitting(creature);
        }

        return doormakerBase.OtherDoormaker.Creature.HpDisplay == HpDisplay.InfiniteWithoutNumbers || doormakerBase.OtherDoormaker.Creature.IsDead;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature.Monster is not DoormakerBase doormakerBase)
        {
            return;
        }

        if (!CombatState.Enemies.Any(c => c is { Monster: DoormakerBase, IsAlive: true })) {
            NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 5f);
            return;
        }

        if (creature != Creature)
        {
            return;
        }

        await PowerCmd.Remove<HungerPower>(Creature);
        await PowerCmd.Remove<ScrutinyPlusPower>(Creature);
        var doomPower = creature.GetPower<DoomPower>();
        if (doomPower != null && doomPower.IsOwnerDoomed())
        {
            return;
        }

        var state = (MoveState?) doormakerBase.OtherDoormaker.MoveStateMachine?.States["DRAMATIC_OPEN_MOVE"];
        if (state == null)
        {
            return;
        }

        doormakerBase.OtherDoormaker.SetMoveImmediate(state);
    }

    protected virtual async Task Open()
    {
        await CreatureCmd.SetMaxHp(Creature, OriginalMaxHp);
        await CreatureCmd.SetCurrentHp(Creature, OriginalHp);
        Creature.HpDisplay = HpDisplay.Normal;
        foreach (var power in Creature.Powers.ToList())
        {
            await PowerCmd.Remove(power);
        }
        foreach (var oldPower in _powerModels)
        {
            var powerModel = PowerCmd.FindExistingInstanceForStacking(oldPower, Creature, oldPower.Applier);
            if (powerModel != null)
            {
                if (powerModel is ITemporaryPower temporaryPower)
                {
                    temporaryPower.IgnoreNextInstance();
                }
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), powerModel, oldPower.Amount, oldPower.Applier, null);
            }
            else
            {
                var power = (PowerModel)oldPower.ClonePreservingMutability();
                if (power is ITemporaryPower temporaryPower)
                {
                    temporaryPower.IgnoreNextInstance();
                }
                await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), power, Creature, oldPower.Amount, oldPower.Applier, null);
            }
        }
        _powerModels.Clear();
    }

    protected void UpdateVisual(string path, bool reverse = false)
    {
        var nCreature = NCombatRoom.Instance?.GetCreatureNode(Creature);
        if (nCreature == null)
        {
            return;
        }

        ((Sprite2D) nCreature.Visuals.GetCurrentBody()).Texture = PreloadManager.Cache.GetTexture2D(ImageHelper.GetImagePath(path));
        var scale = nCreature.Visuals.GetCurrentBody().Scale;
        var tween = nCreature.CreateTween();
        if (reverse)
        {
            tween.TweenProperty(nCreature.Visuals.GetCurrentBody(), "scale", scale, 1.2).From(scale * 2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        }
        else
        {
            tween.TweenProperty(nCreature.Visuals.GetCurrentBody(), "scale", scale, 1.2).From(scale * 0.5f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        }
        tween.Parallel().TweenProperty(nCreature.Visuals.GetCurrentBody(), "modulate", Colors.White, 0.5).From(Colors.Black);
    }

    protected bool ShouldWakeUp()
    {
        return OtherDoormaker.Creature.HpDisplay == HpDisplay.InfiniteWithoutNumbers || !OtherDoormaker.Creature.IsAlive;
    }

    protected Task ReadyToSummon()
    {
        if (OtherDoormaker.Creature is not { IsAlive: true, HpDisplay: HpDisplay.InfiniteWithoutNumbers })
        {
            return Task.CompletedTask;
        }

        MoveState moveState = new MoveState("READY_TO_SUMMON", _ => Task.CompletedTask, new SummonIntent())
        {
            FollowUpState = DramaticOpenState
        };
        OtherDoormaker.SetMoveImmediate(moveState);
        return Task.CompletedTask;
    }

    protected async Task Close()
    {
        OriginalMaxHp = Creature.MaxHp;
        OriginalHp = Creature.CurrentHp;
        await CreatureCmd.SetMaxAndCurrentHp(Creature, 999999999);
        Creature.HpDisplay = HpDisplay.InfiniteWithoutNumbers;
        foreach (var power in Creature.Powers.ToList())
        {
            _powerModels.Add((PowerModel) power.ClonePreservingMutability());
            await PowerCmd.Remove(power);
        }
        UpdateVisual(ClosedState, true);
        await Cmd.CustomScaledWait(0.2f, 0.6f);
        if (CombatState.Enemies.Count <= 1)
        {
            return;
        }

        ((CombatRoom?) CombatState.RunState.CurrentRoom)?.CombatState._enemies.Sort((_, _) => 1);
    }
}