namespace RebalancedSpire.Core.Registry;

using Afflictions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Cards.Transforms;

internal sealed class RebalancedSpireCardTransformationRegistry
{
    internal static void Initialize()
    {
        var registry = ModCardTransformRegistry.For(RebalancedSpireMain.ModId);
        registry.Register<Wither, CardModel>("REBALANCED_SPIRE_CARD_WITHER", async (w, c) =>
        {
            var combatState = c.CombatState;
            if (combatState == null)
            {
                return;
            }

            var wither = combatState.CreateCard<Wither>(c.Owner);
            for (var i = 0; i < w.FakeUpgradeLevel; i++)
            {
                wither.FakeUpgrade();
            }
            await CardCmd.Afflict<Withering>(wither, 1);
            var result = await CardPileCmd.AddGeneratedCardToCombat(wither, PileType.Discard, null, CardPilePosition.Random);
            if (!LocalContext.IsMe(c.Owner))
            {
                return;
            }

            CardCmd.PreviewCardPileAdd(result, 0f);
        });
    }
}