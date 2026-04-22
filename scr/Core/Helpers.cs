using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace RebalancedSpire.scr.Core;

public static class Helpers
{
    public static Func<T, IReadOnlyList<Creature>, Task> GetDelegate<T>(string name) where T : MonsterModel
    {
        return (Func<T, IReadOnlyList<Creature>, Task>) Delegate.CreateDelegate(typeof(Func<T, IReadOnlyList<Creature>, Task>), null, AccessTools.Method(typeof(T), name, [typeof(IReadOnlyList<Creature>)]));
    }

    public static EventOption RelicOption(AncientEventModel instance, RelicModel relic, string pageName = "INITIAL", string? customDonePage = null)
    {
        relic.AssertMutable();
        relic.Owner = instance.Owner!;
        var textKey = $"{StringHelper.Slugify(instance.GetType().Name)}.pages.{pageName}.options.{relic.Id.Entry}";
        return EventOption.FromRelic(relic, instance, OnChosen, textKey);

        async Task OnChosen()
        {
            await RelicCmd.Obtain(relic, instance.Owner!);
            instance.CustomDonePage = customDonePage;
            instance.Done();
        }
    }

    public static EventOption RelicOption<T>(AncientEventModel instance, string pageName = "INITIAL", string? customDonePage = null) where T : RelicModel
    {
        RelicModel relic = ModelDb.Relic<T>().ToMutable();
        return RelicOption(instance, relic, pageName, customDonePage);
    }
}