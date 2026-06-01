namespace RebalancedSpire.Core.Encounters;

using Afflictions;
using Configs;
using Godot;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using Monsters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterActEncounter(typeof(Glory))]
[UsedImplicitly]
public sealed class DoormakerBoss : ModEncounterTemplate
{
	private static readonly bool Enabled = RebalancedSpireSettingsStore.Settings.Doormaker;

	public override RoomType RoomType => RoomType.Boss;

	public override MegaSkeletonDataResource? BossNodeSpineResource => null;

	public override string CustomRunHistoryIconOutlinePath => ImageHelper.GetImagePath("ui/run_history/rebalanced_spire_doormaker_boss_outline.png");

	public override string CustomRunHistoryIconPath => ImageHelper.GetImagePath("ui/run_history/rebalanced_spire_doormaker_boss.png");

	public override EncounterAssetProfile AssetProfile => new(
		EncounterScenePath: "res://scenes/encounters/doormaker_boss.tscn"
	);

	public override string BossNodePath => "res://images/map/placeholder/rebalanced_spire_doormaker_boss_icon";

	public override string CustomBgm => "event:/music/act3_boss_queen";

	public override bool HasScene => true;

	public override bool FullyCenterPlayers => true;

	public override bool IsValidForAct(ActModel act)
	{
		return act is Glory && Enabled;
	}

	public override IEnumerable<MonsterModel> AllPossibleMonsters => new List<MonsterModel>(
	[
		ModelDb.Monster<DoormakerLeft>(),
		ModelDb.Monster<DoormakerRight>()
	]).AsReadOnly();

	public override IEnumerable<string> ExtraAssetPaths => new List<string>(
	[
		"res://scenes/creature_visuals/rebalanced_spire_monster_doormaker_boss.tscn",
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

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return new List<(MonsterModel, string?)>(
		[
			(ModelDb.Monster<DoormakerLeft>().ToMutable(), "doormaker_left"),
			(ModelDb.Monster<DoormakerRight>().ToMutable(), "doormaker_right")
		]).AsReadOnly();
	}
}
