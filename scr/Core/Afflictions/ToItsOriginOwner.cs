using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using RebalancedSpire.scr.Core.Powers;

namespace RebalancedSpire.scr.Core.Afflictions;

public sealed class ToItsOriginOwner : AfflictionModel
{
    public override bool HasExtraCardText => true;

    public override bool CanAfflict(CardModel card)
    {
        return card is ByrdonisEgg;
    }

    public override void AfterApplied()
    {
        CardCmd.RemoveKeyword(Card, CardKeyword.Unplayable);
        CardCmd.ApplyKeyword(Card, CardKeyword.Exhaust);
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Card is not ByrdonisEgg || target != null)
        {
            return;
        }

        var enemies = Card.CombatState?.Enemies;
        if (enemies == null)
        {
            return;
        }

        List<Creature> byrdonis = [];
        foreach (Creature enemy in enemies)
        {
            if (enemy.Monster is not Byrdonis)
            {
                continue;
            }

            await CreatureCmd.TriggerAnim(enemy, "NotAngry", 1f);
            byrdonis.Add(enemy);
        }

        if (byrdonis.Count == 0)
        {
            return;
        }

        var players = Card.CombatState?.Players;
        if (players == null)
        {
            return;
        }

        foreach (Player player in players)
        {
            await PowerCmd.Apply<ToItsOriginOwnerPower>(player.Creature, 1, player.Creature, null);
        }
        await Cmd.Wait(0.5f);
        foreach (Creature enemy in byrdonis)
        {
            enemy.RemoveAllPowersInternalExcept();
            CombatManager.Instance.RemoveCreature(enemy);
            CombatState.RemoveCreature(enemy);
        }
    }
}