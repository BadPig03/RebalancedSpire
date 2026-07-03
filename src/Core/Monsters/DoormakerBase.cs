namespace RebalancedSpire.Core.Monsters;

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
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Scaffolding.Content;

public abstract class DoormakerBase : ModMonsterTemplate
{
    public const string ClosedState = "monsters/beta/door_maker_placeholder_1.png";
    public const string EyeState = "monsters/beta/door_maker_placeholder_2.png";
    public const string MouthState = "monsters/beta/door_maker_placeholder_3.png";

    private DoormakerBase? _otherDoormaker;

    private int _originalMaxHp;

    private int _originalHp;

    private MoveState? _dramaticOpenState;

    private readonly Dictionary<PowerModel, int> _powerModels = [];

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

    public override MonsterAssetProfile AssetProfile => new(
        VisualsScenePath: "res://scenes/creature_visuals/rebalanced_spire_monster_doormaker_boss.tscn"
    );

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

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature.Monster is not DoormakerBase doormakerBase)
        {
            return Task.CompletedTask;
        }

        if (!CombatState.Enemies.Any(c => c is { Monster: DoormakerBase, IsAlive: true })) {
            NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 5f);
            return Task.CompletedTask;
        }

        if (creature != Creature)
        {
            return Task.CompletedTask;
        }

        var doomPower = creature.GetPower<DoomPower>();
        if (doomPower != null && doomPower.IsOwnerDoomed())
        {
            return Task.CompletedTask;
        }

        var state = (MoveState?) doormakerBase.OtherDoormaker.MoveStateMachine?.States["DRAMATIC_OPEN_MOVE"];
        if (state == null)
        {
            return Task.CompletedTask;
        }

        doormakerBase.OtherDoormaker.SetMoveImmediate(state);
        return Task.CompletedTask;
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
        foreach (var pair in _powerModels)
        {
            if (pair.Value == 0)
            {
                continue;
            }

            var powerModel = PowerCmd.FindExistingInstanceForStacking(pair.Key, Creature, pair.Key.Applier);
            if (powerModel != null)
            {
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), powerModel, pair.Value, pair.Key.Applier, null);
                continue;
            }

            var power = (PowerModel)pair.Key.ClonePreservingMutability();
            await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), power, Creature, pair.Value, pair.Key.Applier, null);
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
        var dict = Creature.Powers.Select(p => ((PowerModel)p.ClonePreservingMutability(), p.Amount)).ToDictionary();
        foreach (var pair in dict)
        {
            _powerModels.Add(pair.Key, pair.Value);
        }
        foreach (var (key, value) in _powerModels)
        {
            if (key is not ITemporaryPower temporaryPower)
            {
                continue;
            }

            var pair = _powerModels.FirstOrDefault(p => p.Key.Id == temporaryPower.InternallyAppliedPower.Id);
            if (pair.Key == null)
            {
                continue;
            }

            _powerModels[pair.Key] += value;
        }

        var powers = Creature.Powers.ToList();
        foreach (var power in powers)
        {
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