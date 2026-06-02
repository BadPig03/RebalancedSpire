namespace RebalancedSpire.Core.Afflictions;

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterAffliction]
public sealed class ToItsOriginOwner : ModAfflictionTemplate
{
    private const int ToItsOriginOwnerPowerAmount = 1;

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

        var enemies = Card.CombatState?.Enemies.ToList();
        if (enemies == null)
        {
            return;
        }

        List<Creature> byrdonis = [];
        var byrdonisEnemies = enemies.Where(c => c.Monster is Byrdonis).ToList();
        foreach (var enemy in byrdonisEnemies)
        {
            await CreatureCmd.TriggerAnim(enemy, "NotAngry", 1f);
            byrdonis.Add(enemy);
        }
        if (byrdonis.Count == 0)
        {
            return;
        }

        var players = Card.CombatState?.Players.ToList();
        if (players == null)
        {
            return;
        }

        foreach (var player in players)
        {
            await PowerCmd.Apply<ToItsOriginOwnerPower>(choiceContext, player.Creature, ToItsOriginOwnerPowerAmount, player.Creature, null);
        }
        await Cmd.Wait(0.6f);
        foreach (var enemy in byrdonis)
        {
            enemy.RemoveAllPowersInternalExcept();
            CombatManager.Instance.RemoveCreature(enemy);
            CombatState.RemoveCreature(enemy);
        }
    }
}