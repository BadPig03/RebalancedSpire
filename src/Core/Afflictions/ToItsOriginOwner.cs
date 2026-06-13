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
        if (Card is not ByrdonisEgg || target?.Monster is not Byrdonis)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(target, "NotAngry", 1f);
        var players = Card.CombatState?.Players.ToList();
        if (players == null)
        {
            return;
        }

        foreach (var player in players)
        {
            await PowerCmd.Apply<ToItsOriginOwnerPower>(choiceContext, player.Creature, ToItsOriginOwnerPowerAmount, player.Creature, null);
        }
        await Cmd.Wait(0.3f);
        target.RemoveAllPowersInternalExcept();
        CombatManager.Instance.RemoveCreature(target);
        CombatState.RemoveCreature(target);
    }
}