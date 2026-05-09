namespace RebalancedSpire.scr.Core.Powers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Afflictions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

public sealed class HungerPower : CustomPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromAffliction<Devoured>(Amount);

	public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		var players = Owner.CombatState?.Players.ToList();
		if (players == null)
		{
			return;
		}

		foreach (var card in players.Select(player => player.PlayerCombatState?.AllCards.Where(c => c.Type is CardType.Attack or CardType.Skill).ToList()).OfType<List<CardModel>>().SelectMany(l => l))
		{
			await Afflict(card);
		}
	}

	public override async Task AfterCardEnteredCombat(CardModel card)
	{
		if (card.Affliction != null)
		{
			return;
		}

		await Afflict(card);
	}

	public override Task AfterRemoved(Creature oldOwner)
	{
		var players = oldOwner.CombatState?.Players.ToList();
		if (players == null)
		{
			return Task.CompletedTask;
		}

		foreach (var card in players.Select(player => player.PlayerCombatState?.AllCards.Where(c => c.Affliction is Devoured).ToList()).OfType<List<CardModel>>().SelectMany(l => l))
		{
			if (((Devoured?)card.Affliction)?.AppliedExhaust == true)
			{
				CardCmd.RemoveKeyword(card, CardKeyword.Exhaust);
			}

			CardCmd.ClearAffliction(card);
		}

		return Task.CompletedTask;
	}

	private async Task Afflict(CardModel card)
	{
		if (card.Affliction != null)
		{
			return;
		}

		var devoured = await CardCmd.Afflict<Devoured>(card, Amount);
		if (devoured == null || card.Keywords.Contains(CardKeyword.Exhaust))
		{
			return;
		}

		CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
		devoured.AppliedExhaust = true;
	}
}
