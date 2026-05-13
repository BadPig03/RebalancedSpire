namespace RebalancedSpire.scr.Core.Encounters;

using Afflictions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Monsters;
using Powers;

[UsedImplicitly]
public sealed class DoormakerBoss() : CustomEncounterModel(RoomType.Boss, !Disabled)
{
	private static readonly bool Disabled = !RebalancedSpireConfig.DoormakerConfig;

	public override RoomType RoomType => RoomType.Boss;

	public override MegaSkeletonDataResource? BossNodeSpineResource => null;

	public override string CustomRunHistoryIconOutlinePath => ImageHelper.GetImagePath("ui/run_history/rebalancedspire-doormaker_boss_outline.png");

	public override string CustomRunHistoryIconPath => ImageHelper.GetImagePath("ui/run_history/rebalancedspire-doormaker_boss.png");

	public override string CustomScenePath => "res://scenes/encounters/doormaker_boss.tscn";

	public override string BossNodePath => "res://images/map/placeholder/rebalancedspire-doormaker_boss_icon";

	public override string CustomBgm => "event:/music/act3_boss_queen";

	public override bool HasScene => true;

	public override bool FullyCenterPlayers => true;

	public override bool IsValidForAct(ActModel act)
	{
		return act.ActNumber() == 3;
	}

	public override IEnumerable<MonsterModel> AllPossibleMonsters => new List<MonsterModel>(
	[
		ModelDb.Monster<DoormakerLeft>(),
		ModelDb.Monster<DoormakerRight>()
	]).AsReadOnly();

	public override IEnumerable<string> ExtraAssetPaths => new List<string>(
	[
		"res://scenes/creature_visuals/doormaker_boss.tscn",
		"res://images/" + DoormakerBase.ClosedState,
		"res://images/" + DoormakerBase.EyeState,
		"res://images/" + DoormakerBase.MouthState,
		ModelDb.Affliction<Devoured>().OverlayPath,
		ModelDb.Affliction<Weighted>().OverlayPath
	]).AsReadOnly();

	public override IReadOnlyList<string> Slots => new List<string>(
	[
		"doormaker_right", "doormaker_left"
	]).AsReadOnly();

	public override float GetCameraScaling()
	{
		return 0.75f;
	}

	public override Vector2 GetCameraOffset()
	{
		return Vector2.Down * 35f;
	}

	public override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return new List<(MonsterModel, string?)>(
		[
			(ModelDb.Monster<DoormakerLeft>().ToMutable(), "doormaker_left"),
			(ModelDb.Monster<DoormakerRight>().ToMutable(), "doormaker_right")
		]).AsReadOnly();
	}
}
