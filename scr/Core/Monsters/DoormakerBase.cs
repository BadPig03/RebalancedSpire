namespace RebalancedSpire.scr.Core.Monsters;

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
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

public abstract class DoormakerBase : CustomMonsterModel
{
    private const string ClosedState = "monsters/beta/door_maker_placeholder_1.png";
    protected const string EyeState = "monsters/beta/door_maker_placeholder_2.png";
    protected const string MouthState = "monsters/beta/door_maker_placeholder_3.png";

    public override int MinInitialHp => 400;

    public override int MaxInitialHp => MinInitialHp;

    private bool _isPortalOpen;

    private bool _isAboutToEscape;

    private int _originalMaxHp;

    private int _originalHp;

    private MoveState? _dramaticOpenState;

    private readonly List<PowerModel> _powerModels = [];

    public bool IsPortalOpen
    {
        get => _isPortalOpen;
        private set
        {
            AssertMutable();
            _isPortalOpen = value;
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

    public bool IsAboutToEscape
    {
        get => _isAboutToEscape;
        protected set
        {
            AssertMutable();
            _isAboutToEscape = value;
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

    public override LocString Title => IsPortalOpen ? L10NMonsterLookup("DOORMAKER.name") : L10NMonsterLookup("DOOR.name") ;

    public override NCreatureVisuals CreateCustomVisuals()
    {
        return NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/doormaker.tscn");
    }

    public override async Task AfterAddedToRoom()
    {
        UpdateVisual(ClosedState);
        OriginalMaxHp = Creature.MaxHp;
        OriginalHp = Creature.CurrentHp;
        await CreatureCmd.SetMaxAndCurrentHp(Creature, 999999999);
        Creature.ShowsInfiniteHp = true;
    }

    public override bool ShouldAllowHitting(Creature creature)
    {
        return creature != Creature ? base.ShouldAllowHitting(creature) : IsPortalOpen;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature.Monster is not DoormakerBase)
        {
            return Task.CompletedTask;
        }

        if (!CombatState.Enemies.Any(enemy => enemy.IsAlive)) {
            NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 5f);
        }
        else if (creature != Creature)
        {
            SetMoveImmediate(DramaticOpenState);
        }
        return Task.CompletedTask;
    }

    protected bool ShouldClose()
    {
        return CombatState.Enemies.Count > 1;
    }

    protected void UpdateVisual(string path, bool reverse = false)
    {
        NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(Creature);
        if (nCreature == null)
        {
            return;
        }

        ((Sprite2D) nCreature.Visuals.GetCurrentBody()).Texture = PreloadManager.Cache.GetTexture2D(ImageHelper.GetImagePath(path));
        Vector2 scale = nCreature.Visuals.GetCurrentBody().Scale;
        Tween tween = nCreature.CreateTween();
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

    protected async Task Open()
    {
        IsPortalOpen = true;
        await CreatureCmd.SetMaxHp(Creature, OriginalMaxHp);
        await CreatureCmd.SetCurrentHp(Creature, OriginalHp);
        Creature.ShowsInfiniteHp = false;
        foreach (PowerModel power in Creature.Powers.ToList())
        {
            await PowerCmd.Remove(power);
        }
        foreach (PowerModel power in _powerModels)
        {
            if (power is ITemporaryPower temporaryPower)
            {
                temporaryPower.IgnoreNextInstance();
            }
            await PowerCmd.Apply(power, Creature, power.Amount, Creature, null);
        }
        _powerModels.Clear();
    }

    protected async Task Close()
    {
        IsPortalOpen = false;
        IsAboutToEscape = false;
        OriginalMaxHp = Creature.MaxHp;
        OriginalHp = Creature.CurrentHp;
        await CreatureCmd.SetMaxAndCurrentHp(Creature, 999999999);
        Creature.ShowsInfiniteHp = true;
        var powers = Creature.Powers.ToList();
        foreach (PowerModel power in powers)
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

        CombatState._enemies.Sort((_, _) => 1);
    }
}