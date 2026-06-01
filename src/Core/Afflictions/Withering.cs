namespace RebalancedSpire.Core.Afflictions;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterAffliction]
public sealed class Withering : ModAfflictionTemplate
{
    private const int SandsOfTimePowerAmount = 1;
    private const int InitUpgradeLevel = 2;

    public override bool HasExtraCardText => true;

    public override bool CanAfflict(CardModel card)
    {
        return card is Wither;
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Card is not Wither wither)
        {
            return;
        }

        var level = wither.FakeUpgradeLevel;
        if (level == 0)
        {
            for (var i = 0; i < InitUpgradeLevel; i++)
            {
                wither.FakeUpgrade();
            }
            wither.EnergyCost.AddThisCombat(1);
            await PowerCmd.Apply<SandsOfTimePower>(new ThrowingPlayerChoiceContext(), wither.Owner.Creature, SandsOfTimePowerAmount, wither.Owner.Creature, null);
        }
        else
        {
            wither.FakeUpgradeLevel--;
            wither.DynamicVars.Damage.UpgradeValueBy(-wither.DynamicVars["PerLevel"].BaseValue);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card is not Wither wither)
        {
            return;
        }

        await CardPileCmd.Add(wither, PileType.Discard, CardPilePosition.Random);
    }
}