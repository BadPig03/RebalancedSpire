namespace RebalancedSpire.scr.Core.Singletons;

using BaseLib.Abstracts;
using BaseLib.Hooks;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;

[UsedImplicitly]
public sealed class FiddleSingleton() : CustomSingletonModel(true, false), IMaxHandSizeModifier
{
    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        var fiddle = player.GetRelic<Fiddle>();
        if (fiddle == null)
        {
            return currentMaxHandSize;
        }

        return currentMaxHandSize - fiddle.DynamicVars.Cards.IntValue * player.Relics.OfType<Fiddle>().Count();
    }
}