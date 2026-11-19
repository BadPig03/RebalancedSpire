namespace RebalancedSpire.Core.Harmony.Blitz;

using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

internal static class BlitzEventWhitelist
{
    private static readonly HashSet<Type> Events =
    [
        typeof(AbyssalBaths),
        typeof(Amalgamator),
        typeof(AromaOfChaos),
        typeof(BrainLeech),
        typeof(Bugslayer),
        typeof(ByrdonisNest),
        typeof(ColorfulPhilosophers),
        typeof(DollRoom),
        typeof(DoorsOfLightAndDark),
        typeof(DrowningBeacon),
        typeof(FieldOfManSizedHoles),
        typeof(GraveOfTheForgotten),
        typeof(HungryForMushrooms),
        typeof(InfestedAutomaton),
        typeof(MuseumOfTorture),
        typeof(Reflections),
        typeof(RelicTrader),
        typeof(RoomFullOfCheese),
        typeof(RoundTeaParty),
        typeof(SapphireSeed),
        typeof(SelfHelpBook),
        typeof(ShrineOfTheWarlord),
        typeof(SlipperyBridge),
        typeof(SpiralingWhirlpool),
        typeof(SpiritGrafter),
        typeof(StoneOfAllTime),
        typeof(Symbiote),
        typeof(TabletOfTruth),
        typeof(TheFutureOfPotions),
        typeof(TinkerTime),
        typeof(UnrestSite),
        typeof(Wellspring),
        typeof(WoodCarvings),
        typeof(WuppyInTrouble)
    ];

    internal static void Apply(ActModel act)
    {
        act._rooms.events.RemoveAll(evt => !Events.Contains(evt.GetType()));
    }
}
