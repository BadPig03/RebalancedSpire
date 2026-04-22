namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class ThievingHopperPatch
{
	private static int WeakPowerAmount => 2;
	private static int EscapeArtistPowerAmount => 6;
	private static int AttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 13);

    private static readonly Func<ThievingHopper, IReadOnlyList<Creature>, Task>? _flutterMoveDelegate = Helpers.GetDelegate<ThievingHopper>("FlutterMove");
    private static readonly Func<ThievingHopper, IReadOnlyList<Creature>, Task>? _hatTrickMoveDelegate = Helpers.GetDelegate<ThievingHopper>("HatTrickMove");
    private static readonly Func<ThievingHopper, IReadOnlyList<Creature>, Task>? _nabMoveDelegate = Helpers.GetDelegate<ThievingHopper>("NabMove");
    private static readonly Func<ThievingHopper, IReadOnlyList<Creature>, Task>? _escapeMoveDelegate = Helpers.GetDelegate<ThievingHopper>("EscapeMove");

    private static async Task AttackMove(ThievingHopper instance, IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(AttackDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
	    await PowerCmd.Apply<WeakPower>(targets, WeakPowerAmount, instance.Creature, null);
    }

    private static async Task ThieveryMove(ThievingHopper instance, IReadOnlyList<Creature> targets)
    {
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
		if (creatureNode != null)
		{
			Creature creature = LocalContext.GetMe(targets) ?? targets[0];
			NCreature? creatureNode2 = NCombatRoom.Instance?.GetCreatureNode(creature);
			Node2D? specialNode = creatureNode.GetSpecialNode<Node2D>("Visuals/SpineBoneNode");
			if (specialNode != null && creatureNode2 != null)
			{
				specialNode.Position = Vector2.Right * (creatureNode2.GlobalPosition.X - creatureNode.GlobalPosition.X);
			}
		}
		await CreatureCmd.TriggerAnim(instance.Creature, "Steal", 0.25f);
		SfxCmd.Play("event:/sfx/enemy/enemy_attacks/thieving_hopper/thieving_hopper_steal");
		List<CardModel> cardsToSteal = [];
		foreach (Creature target in targets)
		{
			Player? victim = target.Player ?? target.PetOwner;
			if (victim == null)
			{
				continue;
			}

			List<CardModel> list = (from c in CardPile.GetCards(victim, PileType.Draw, PileType.Discard) where c.DeckVersion != null select c).ToList();
			IEnumerable<CardModel> items = list;
			foreach (var predicate in ThievingHopper._stealPriorities)
			{
				var matchingItems = list.Where(predicate).ToList();
				if (matchingItems.Count == 0)
				{
					continue;
				}

				items = matchingItems;
				break;
			}

			CardModel? cardToSteal = instance.RunRng.CombatCardGeneration.NextItem(items);
			if (cardToSteal == null)
			{
				return;
			}

			await CardPileCmd.RemoveFromCombat(cardToSteal);
			cardsToSteal.Add(cardToSteal);
		}
		await Cmd.Wait(0.6f);
		foreach (CardModel item in cardsToSteal)
		{
			if (creatureNode != null && LocalContext.IsMine(item))
			{
				Marker2D? specialNode2 = creatureNode.GetSpecialNode<Marker2D>("%StolenCardPos");
				if (specialNode2 != null)
				{
					NCard? nCard = NCard.Create(item);
					if (nCard == null)
					{
						continue;
					}

					specialNode2.AddChildSafely(nCard);
					nCard.Position += nCard.Size * 0.5f;
					nCard.UpdateVisuals(PileType.Deck, CardPreviewMode.Normal);
				}
			}
			SwipePower swipe = (SwipePower)ModelDb.Power<SwipePower>().ToMutable();
			await swipe.Steal(item);
			await PowerCmd.Apply(swipe, instance.Creature, 1m, instance.Creature, null);
		}
    }

    private static async Task FlutterMove(ThievingHopper instance, IReadOnlyList<Creature> targets)
    {
        if (_flutterMoveDelegate == null)
        {
            return;
        }

        await _flutterMoveDelegate(instance, targets);
    }

    private static async Task HatTrickMove(ThievingHopper instance, IReadOnlyList<Creature> targets)
    {
        if (_hatTrickMoveDelegate == null)
        {
            return;
        }

        await _hatTrickMoveDelegate(instance, targets);
    }

    private static async Task NabMove(ThievingHopper instance, IReadOnlyList<Creature> targets)
    {
        if (_nabMoveDelegate == null)
        {
            return;
        }

        await _nabMoveDelegate(instance, targets);
    }

    private static async Task EscapeMove(ThievingHopper instance, IReadOnlyList<Creature> targets)
    {
        if (_escapeMoveDelegate == null)
        {
            return;
        }

        await _escapeMoveDelegate(instance, targets);
    }

    private static async Task AfterAddedToRoom(ThievingHopper instance)
    {
	    await PowerCmd.Apply<EscapeArtistPower>(instance.Creature, EscapeArtistPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(ThievingHopper), nameof(ThievingHopper.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(ThievingHopper __instance, ref Task __result)
    {
	    __result = AfterAddedToRoom(__instance);
	    return false;
    }

    [HarmonyPatch(typeof(ThievingHopper), nameof(ThievingHopper.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(ThievingHopper __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("ATTACK_MOVE", t => AttackMove(__instance, t), new SingleAttackIntent(AttackDamage), new DebuffIntent());
        MoveState moveState2 = new MoveState("THIEVERY_MOVE", t => ThieveryMove(__instance, t), new CardDebuffIntent());
        MoveState moveState3 = new MoveState("FLUTTER_MOVE", t => FlutterMove(__instance, t), new BuffIntent());
        MoveState moveState4 = new MoveState("HAT_TRICK_MOVE", t => HatTrickMove(__instance, t), new SingleAttackIntent(__instance.HatTrickDamage));
        MoveState moveState5 = new MoveState("NAB_MOVE", t => NabMove(__instance, t), new SingleAttackIntent(__instance.NabDamage));
        MoveState moveState6 = new MoveState("ESCAPE_MOVE", t => EscapeMove(__instance, t), new EscapeIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState5;
        moveState5.FollowUpState = moveState6;
        moveState6.FollowUpState = moveState6;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        list.Add(moveState5);
        list.Add(moveState6);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}