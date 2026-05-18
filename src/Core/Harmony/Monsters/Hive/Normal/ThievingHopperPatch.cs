namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Normal;

using Configs;
using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ThievingHopperPatch
{
	private static readonly bool Disabled = !RebalancedSpireConfig.ThievingHopperConfig;

	private const int WeakPowerAmount = 2;
	private const int SwipePowerAmount = 1;
	private const int EscapeArtistPowerAmount = 6;

	private static int AttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 13);

	private static async Task AfterAddedToRoom(ThievingHopper instance)
	{
		await PowerCmd.Apply<EscapeArtistPower>(new ThrowingPlayerChoiceContext(), instance.Creature, EscapeArtistPowerAmount, instance.Creature, null);
	}

    private static async Task AttackMove(ThievingHopper instance, IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(AttackDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.3f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
	    await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, WeakPowerAmount, instance.Creature, null);
    }

    private static async Task ThieveryMove(ThievingHopper instance, IReadOnlyList<Creature> targets)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(instance.Creature);
		if (creatureNode != null)
		{
			Creature creature = LocalContext.GetMe(targets) ?? targets[0];
			var creatureNode2 = NCombatRoom.Instance?.GetCreatureNode(creature);
			var specialNode = creatureNode.GetSpecialNode<Node2D>("Visuals/SpineBoneNode");
			if (specialNode != null && creatureNode2 != null)
			{
				specialNode.Position = Vector2.Right * (creatureNode2.GlobalPosition.X - creatureNode.GlobalPosition.X);
			}
		}
		await CreatureCmd.TriggerAnim(instance.Creature, "Steal", 0.25f);
		SfxCmd.Play("event:/sfx/enemy/enemy_attacks/thieving_hopper/thieving_hopper_steal");
		List<CardModel> cardsToSteal = [];
		foreach (var target in targets)
		{
			var player = target.Player ?? target.PetOwner;
			if (player == null)
			{
				continue;
			}

			var list = CardPile.GetCards(player, PileType.Draw, PileType.Discard).Where(c => c.DeckVersion != null).ToList();
			var items = list;
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

			var cardToSteal = instance.RunRng.CombatCardGeneration.NextItem(items);
			if (cardToSteal == null)
			{
				return;
			}

			await CardPileCmd.RemoveFromCombat(cardToSteal);
			cardsToSteal.Add(cardToSteal);
		}
		await Cmd.Wait(0.6f);
		foreach (var item in cardsToSteal)
		{
			if (creatureNode != null && LocalContext.IsMine(item))
			{
				var specialNode2 = creatureNode.GetSpecialNode<Marker2D>("%StolenCardPos");
				if (specialNode2 != null)
				{
					var nCard = NCard.Create(item);
					if (nCard == null)
					{
						continue;
					}

					specialNode2.AddChildSafely(nCard);
					nCard.Position += nCard.Size * 0.5f;
					nCard.UpdateVisuals(PileType.Deck, CardPreviewMode.Normal);
				}
			}
			SwipePower swipe = (SwipePower) ModelDb.Power<SwipePower>().ToMutable();
			await swipe.Steal(item);
			await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), swipe, instance.Creature, SwipePowerAmount, instance.Creature, null);
		}
    }

    [HarmonyPatch(typeof(ThievingHopper), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(ThievingHopper __instance, ref Task __result)
    {
	    if (Disabled)
	    {
		    return true;
	    }

	    __result = AfterAddedToRoom(__instance);
	    return false;
    }

    [HarmonyPatch(typeof(ThievingHopper), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(ThievingHopper __instance, ref MonsterMoveStateMachine __result)
    {
	    if (Disabled)
	    {
		    return true;
	    }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("ATTACK_MOVE", t => AttackMove(__instance, t), new SingleAttackIntent(AttackDamage), new DebuffIntent());
        MoveState moveState2 = new MoveState("THIEVERY_MOVE", t => ThieveryMove(__instance, t), new CardDebuffIntent());
        MoveState moveState3 = new MoveState("FLUTTER_MOVE", __instance.FlutterMove, new BuffIntent());
        MoveState moveState4 = new MoveState("HAT_TRICK_MOVE", __instance.HatTrickMove, new SingleAttackIntent(__instance.HatTrickDamage));
        MoveState moveState5 = new MoveState("NAB_MOVE", __instance.NabMove, new SingleAttackIntent(__instance.NabDamage));
        MoveState moveState6 = new MoveState("ESCAPE_MOVE", __instance.EscapeMove, new EscapeIntent());
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