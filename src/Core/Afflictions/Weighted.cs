namespace RebalancedSpire.Core.Afflictions;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using Monsters;
using Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterAffliction]
public sealed class Weighted : ModAfflictionTemplate
{
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != Card || card.Owner.Creature.HasPower<ScrutinyPower>())
        {
            return Task.CompletedTask;
        }

        CardCmd.ClearAffliction(Card);
        return Task.CompletedTask;
    }
}