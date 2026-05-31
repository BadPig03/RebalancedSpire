namespace RebalancedSpire.Core.Singletons;

using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;
using IMaxHandSizeModifier = STS2RitsuLib.Combat.HandSize.IMaxHandSizeModifier;

[RegisterSingleton]
[UsedImplicitly]
public sealed class FiddleSingleton() : HookedSingletonModel(HookType.Combat), IMaxHandSizeModifier
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