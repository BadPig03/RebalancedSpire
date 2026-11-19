namespace RebalancedSpire.Core.Configs;

using MegaCrit.Sts2.Core.Models;

internal static class RebalancedSpireSettingsModelCache
{
    private static readonly Dictionary<string, string[]> Targets = new()
    {
        [nameof(RebalancedSpireSettings.Accelerant)] = ["Accelerant"],
        [nameof(RebalancedSpireSettings.Acrobatics)] = ["Acrobatics"],
        [nameof(RebalancedSpireSettings.Aeonglass)] = ["Aeonglass", "Wither"],
        [nameof(RebalancedSpireSettings.Afterlife)] = ["Afterlife"],
        [nameof(RebalancedSpireSettings.AlchemicalCoffer)] = ["AlchemicalCoffer"],
        [nameof(RebalancedSpireSettings.Axebot)] = ["Axebot"],
        [nameof(RebalancedSpireSettings.BansheesCry)] = ["BansheesCry"],
        [nameof(RebalancedSpireSettings.BeautifulBracelet)] = ["BeautifulBracelet"],
        [nameof(RebalancedSpireSettings.BiiigHug)] = ["Soot"],
        [nameof(RebalancedSpireSettings.BladeOfInk)] = ["BladeOfInk", "Inky"],
        [nameof(RebalancedSpireSettings.BladeSymphony)] = ["BladeSymphony"],
        [nameof(RebalancedSpireSettings.BloodSoakedRose)] = ["Enthralled"],
        [nameof(RebalancedSpireSettings.BloodWall)] = ["BloodWall"],
        [nameof(RebalancedSpireSettings.Bolas)] = ["Bolas"],
        [nameof(RebalancedSpireSettings.BoomingConch)] = ["BoomingConch"],
        [nameof(RebalancedSpireSettings.BouncingFlask)] = ["BouncingFlask"],
        [nameof(RebalancedSpireSettings.BowlbugRock)] = ["BowlbugRock"],
        [nameof(RebalancedSpireSettings.BrilliantScarf)] = ["BrilliantScarf"],
        [nameof(RebalancedSpireSettings.BygoneEffigy)] = ["BygoneEffigy"],
        [nameof(RebalancedSpireSettings.Byrdonis)] = ["Byrdonis", "ByrdonisEgg"],
        [nameof(RebalancedSpireSettings.CalcifiedCultist)] = ["CalcifiedCultist"],
        [nameof(RebalancedSpireSettings.CeremonialBeast)] = ["CeremonialBeast"],
        [nameof(RebalancedSpireSettings.ChoicesParadox)] = ["ChoicesParadox"],
        [nameof(RebalancedSpireSettings.Chomper)] = ["Chomper"],
        [nameof(RebalancedSpireSettings.Colossus)] = ["Colossus"],
        [nameof(RebalancedSpireSettings.ConsumingShadow)] = ["ConsumingShadow"],
        [nameof(RebalancedSpireSettings.Coolant)] = ["Coolant"],
        [nameof(RebalancedSpireSettings.CorpseSlug)] = ["CorpseSlug"],
        [nameof(RebalancedSpireSettings.Crossbow)] = ["Crossbow"],
        [nameof(RebalancedSpireSettings.CubexConstruct)] = ["CubexConstruct"],
        [nameof(RebalancedSpireSettings.DampCultist)] = ["DampCultist"],
        [nameof(RebalancedSpireSettings.DarvAncientChoices)] = ["Darv"],
        [nameof(RebalancedSpireSettings.Debilitate)] = ["Debilitate"],
        [nameof(RebalancedSpireSettings.Decimillipede)] = ["DecimillipedeSegment"],
        [nameof(RebalancedSpireSettings.Defragment)] = ["Defragment"],
        [nameof(RebalancedSpireSettings.Defy)] = ["Defy"],
        [nameof(RebalancedSpireSettings.DiamondDiadem)] = ["DiamondDiadem"],
        [nameof(RebalancedSpireSettings.DistinguishedCape)] = ["DistinguishedCape", "Vakuu"],
        [nameof(RebalancedSpireSettings.Dominate)] = ["Dominate"],
        [nameof(RebalancedSpireSettings.Doormaker)] = ["DoormakerBoss", "Glory"],
        [nameof(RebalancedSpireSettings.DustyTome)] = ["CorpseExplosion", "DustyTome", "IroncladCardPool", "LimitBreak", "SilentCardPool"],
        [nameof(RebalancedSpireSettings.Entomancer)] = ["Entomancer"],
        [nameof(RebalancedSpireSettings.EternalArmor)] = ["EternalArmor", "PlatingPower"],
        [nameof(RebalancedSpireSettings.Exoskeleton)] = ["Exoskeleton", "HardToKillPower"],
        [nameof(RebalancedSpireSettings.ExpectAFight)] = ["ExpectAFight"],
        [nameof(RebalancedSpireSettings.Fabricator)] = ["Fabricator", "Guardbot", "Noisebot", "Stabbot", "Zapbot"],
        [nameof(RebalancedSpireSettings.Fiddle)] = ["Fiddle"],
        [nameof(RebalancedSpireSettings.FishingRod)] = ["FishingRod"],
        [nameof(RebalancedSpireSettings.Flyconid)] = ["Flyconid"],
        [nameof(RebalancedSpireSettings.Fogmog)] = ["EyeWithTeeth", "Fogmog"],
        [nameof(RebalancedSpireSettings.ForegoneConclusion)] = ["ForegoneConclusion"],
        [nameof(RebalancedSpireSettings.ForgottenRitual)] = ["ForgottenRitual"],
        [nameof(RebalancedSpireSettings.FossilStalker)] = ["FossilStalker"],
        [nameof(RebalancedSpireSettings.FrogKnight)] = ["FrogKnight"],
        [nameof(RebalancedSpireSettings.Fuel)] = ["Fuel"],
        [nameof(RebalancedSpireSettings.FuzzyWurmCrawler)] = ["FuzzyWurmCrawler"],
        [nameof(RebalancedSpireSettings.Genesis)] = ["Genesis"],
        [nameof(RebalancedSpireSettings.Glasswork)] = ["Glasswork"],
        [nameof(RebalancedSpireSettings.GlobeHead)] = ["GlobeHead"],
        [nameof(RebalancedSpireSettings.Glow)] = ["Glow"],
        [nameof(RebalancedSpireSettings.GodByrd)] = ["GodByrd"],
        [nameof(RebalancedSpireSettings.GraveWarden)] = ["GraveWarden"],
        [nameof(RebalancedSpireSettings.GremlinMerc)] = ["GremlinMerc"],
        [nameof(RebalancedSpireSettings.HandTrick)] = ["HandTrick"],
        [nameof(RebalancedSpireSettings.HauntedShip)] = ["HauntedShip"],
        [nameof(RebalancedSpireSettings.Haze)] = ["Haze"],
        [nameof(RebalancedSpireSettings.HeirloomHammer)] = ["HeirloomHammer"],
        [nameof(RebalancedSpireSettings.HiddenDaggers)] = ["HiddenDaggers"],
        [nameof(RebalancedSpireSettings.HungryForMushrooms)] = ["BigMushroom", "FragrantMushroom"],
        [nameof(RebalancedSpireSettings.HunterKiller)] = ["HunterKiller"],
        [nameof(RebalancedSpireSettings.InfestedPrism)] = ["InfestedPrism", "Tainted"],
        [nameof(RebalancedSpireSettings.InfiniteBlades)] = ["InfiniteBlades"],
        [nameof(RebalancedSpireSettings.Inklet)] = ["Inklet"],
        [nameof(RebalancedSpireSettings.KaiserCrab)] = ["Crusher", "Rocket"],
        [nameof(RebalancedSpireSettings.Knights)] = ["MagiKnight", "SpectralKnight"],
        [nameof(RebalancedSpireSettings.KnowledgeDemon)] = ["Disintegration", "KnowledgeDemon"],
        [nameof(RebalancedSpireSettings.LargeCapsule)] = ["LargeCapsule"],
        [nameof(RebalancedSpireSettings.LavaRock)] = ["LavaRock"],
        [nameof(RebalancedSpireSettings.LeafSlimeS)] = ["LeafSlimeS"],
        [nameof(RebalancedSpireSettings.Leap)] = ["Leap"],
        [nameof(RebalancedSpireSettings.LivingFog)] = ["GasBomb", "LivingFog"],
        [nameof(RebalancedSpireSettings.LoomingFruit)] = ["LoomingFruit"],
        [nameof(RebalancedSpireSettings.LordsParasol)] = ["LordsParasol"],
        [nameof(RebalancedSpireSettings.LostWisp)] = ["LostWisp"],
        [nameof(RebalancedSpireSettings.MasterPlanner)] = ["MasterPlanner"],
        [nameof(RebalancedSpireSettings.MechaKnight)] = ["MechaKnight"],
        [nameof(RebalancedSpireSettings.Mirage)] = ["Mirage"],
        [nameof(RebalancedSpireSettings.MorphicGrove)] = ["MorphicGrove"],
        [nameof(RebalancedSpireSettings.MuseumOfTorture)] = ["MuseumOfTorture"],
        [nameof(RebalancedSpireSettings.Myte)] = ["Myte"],
        [nameof(RebalancedSpireSettings.NeowAncientChoices)] = ["ArcaneScroll", "GoldenPearl", "Neow", "NeowsTalisman", "NewLeaf", "NutritiousOyster", "Pomander", "PreciseScissors", "SmallCapsule", "StoneHumidifier"],
        [nameof(RebalancedSpireSettings.NeowsTalisman)] = ["NeowsTalisman"],
        [nameof(RebalancedSpireSettings.NeutronAegis)] = ["NeutronAegis"],
        [nameof(RebalancedSpireSettings.Nibbit)] = ["Nibbit"],
        [nameof(RebalancedSpireSettings.NutritiousOyster)] = ["NutritiousOyster"],
        [nameof(RebalancedSpireSettings.OrobasAncientChoices)] = ["Orobas"],
        [nameof(RebalancedSpireSettings.Ovicopter)] = ["Ovicopter", "ToughEgg"],
        [nameof(RebalancedSpireSettings.OwlMagistrate)] = ["OwlMagistrate"],
        [nameof(RebalancedSpireSettings.PaelsHorn)] = ["Relax"],
        [nameof(RebalancedSpireSettings.PaelsLegion)] = ["Pael"],
        [nameof(RebalancedSpireSettings.PhantasmalGardener)] = ["SkittishPower"],
        [nameof(RebalancedSpireSettings.PhrogParasite)] = ["PhrogParasite"],
        [nameof(RebalancedSpireSettings.PillarOfCreation)] = ["PillarOfCreation"],
        [nameof(RebalancedSpireSettings.PoisonedStab)] = ["PoisonedStab"],
        [nameof(RebalancedSpireSettings.Pomander)] = ["Pomander"],
        [nameof(RebalancedSpireSettings.PreservedFog)] = ["Folly", "PreservedFog"],
        [nameof(RebalancedSpireSettings.PullAggro)] = ["PullAggro"],
        [nameof(RebalancedSpireSettings.PunchOff)] = ["PunchConstruct", "PunchOffEventEncounter"],
        [nameof(RebalancedSpireSettings.Queen)] = ["TorchHeadAmalgam"],
        [nameof(RebalancedSpireSettings.ReaperForm)] = ["ReaperForm"],
        [nameof(RebalancedSpireSettings.Reflections)] = ["Reflections"],
        [nameof(RebalancedSpireSettings.Refract)] = ["Refract"],
        [nameof(RebalancedSpireSettings.Regalite)] = ["Regalite"],
        [nameof(RebalancedSpireSettings.RightHandHand)] = ["RightHandHand"],
        [nameof(RebalancedSpireSettings.RocketPunch)] = ["RocketPunch"],
        [nameof(RebalancedSpireSettings.RollingBoulder)] = ["RollingBoulder", "RollingBoulderPower"],
        [nameof(RebalancedSpireSettings.RubyRaiders)] = ["AssassinRubyRaider", "AxeRubyRaider", "BruteRubyRaider", "RubyRaidersNormal"],
        [nameof(RebalancedSpireSettings.Salvo)] = ["Salvo"],
        [nameof(RebalancedSpireSettings.ScrollOfBiting)] = ["ScrollOfBiting"],
        [nameof(RebalancedSpireSettings.Seance)] = ["Seance"],
        [nameof(RebalancedSpireSettings.Seapunk)] = ["Seapunk"],
        [nameof(RebalancedSpireSettings.SereTalon)] = ["SereTalon", "Vakuu", "Wish"],
        [nameof(RebalancedSpireSettings.SewerClam)] = ["SewerClam"],
        [nameof(RebalancedSpireSettings.Shadowmeld)] = ["Shadowmeld"],
        [nameof(RebalancedSpireSettings.Shatter)] = ["Shatter"],
        [nameof(RebalancedSpireSettings.ShrineOfTheWarlord)] = ["MightyShield", "MightyWeapon", "MightyWings", "ShrineOfTheWarlord"],
        [nameof(RebalancedSpireSettings.SicEm)] = ["SicEm"],
        [nameof(RebalancedSpireSettings.SkulkingColony)] = ["SkulkingColony"],
        [nameof(RebalancedSpireSettings.SlimedBerserker)] = ["SlimedBerserker"],
        [nameof(RebalancedSpireSettings.SlitheringStrangler)] = ["SlitheringStrangler"],
        [nameof(RebalancedSpireSettings.SludgeSpinner)] = ["SludgeSpinner"],
        [nameof(RebalancedSpireSettings.SnappingJaxfruit)] = ["SnappingJaxfruit"],
        [nameof(RebalancedSpireSettings.SoulFysh)] = ["SoulFysh"],
        [nameof(RebalancedSpireSettings.SoulNexus)] = ["SoulNexus"],
        [nameof(RebalancedSpireSettings.Spinner)] = ["Spinner"],
        [nameof(RebalancedSpireSettings.SpiralingWhirlpool)] = ["SpiralingWhirlpool"],
        [nameof(RebalancedSpireSettings.SpiritGrafter)] = ["SpiritGrafter"],
        [nameof(RebalancedSpireSettings.Splash)] = ["Splash"],
        [nameof(RebalancedSpireSettings.Spur)] = ["Spur"],
        [nameof(RebalancedSpireSettings.SunkenStatue)] = ["SwordOfJade", "SwordOfStone"],
        [nameof(RebalancedSpireSettings.Synchronize)] = ["Synchronize"],
        [nameof(RebalancedSpireSettings.Tank)] = ["Tank"],
        [nameof(RebalancedSpireSettings.TeaMaster)] = ["EventRelicPool", "TeaMaster"],
        [nameof(RebalancedSpireSettings.TerrorEel)] = ["TerrorEel"],
        [nameof(RebalancedSpireSettings.TestSubject)] = ["TestSubject"],
        [nameof(RebalancedSpireSettings.TheInsatiable)] = ["TheInsatiable"],
        [nameof(RebalancedSpireSettings.TheKin)] = ["KinFollower", "KinPriest", "TheKinBoss"],
        [nameof(RebalancedSpireSettings.TheLanternKey)] = ["MysteriousKnight"],
        [nameof(RebalancedSpireSettings.TheLostAndForgotten)] = ["TheForgotten", "TheLost"],
        [nameof(RebalancedSpireSettings.TheObscura)] = ["IllusionPower", "Parafright"],
        [nameof(RebalancedSpireSettings.ThermaeNonupeipe)] = ["ThermaeNonupeipe"],
        [nameof(RebalancedSpireSettings.TheSealedThrone)] = ["TheSealedThrone"],
        [nameof(RebalancedSpireSettings.ThievingHopper)] = ["ThievingHopper"],
        [nameof(RebalancedSpireSettings.Toadpole)] = ["Toadpole"],
        [nameof(RebalancedSpireSettings.TrashToTreasure)] = ["TrashToTreasure"],
        [nameof(RebalancedSpireSettings.Trial)] = ["Trial"],
        [nameof(RebalancedSpireSettings.Tunneler)] = ["Tunneler"],
        [nameof(RebalancedSpireSettings.TurretOperator)] = ["LivingShield"],
        [nameof(RebalancedSpireSettings.TwigSlimeM)] = ["TwigSlimeM"],
        [nameof(RebalancedSpireSettings.TwoTailedRat)] = ["TwoTailedRat"],
        [nameof(RebalancedSpireSettings.Untouchable)] = ["Untouchable"],
        [nameof(RebalancedSpireSettings.UpMySleeve)] = ["UpMySleeve"],
        [nameof(RebalancedSpireSettings.VakuuAncientChoices)] = ["Vakuu"],
        [nameof(RebalancedSpireSettings.VakuuFixedArt)] = ["Vakuu"],
        [nameof(RebalancedSpireSettings.Vantom)] = ["Vantom"],
        [nameof(RebalancedSpireSettings.VineShambler)] = ["VineShambler"],
        [nameof(RebalancedSpireSettings.Voltaic)] = ["Voltaic"],
        [nameof(RebalancedSpireSettings.WarHammer)] = ["WarHammer"],
        [nameof(RebalancedSpireSettings.WarHistorianRepy)] = ["HistoryCourse"],
        [nameof(RebalancedSpireSettings.WaterfallGiant)] = ["WaterfallGiant"],
        [nameof(RebalancedSpireSettings.WaterfallOfObviousSecrets)] = ["WaterfallOfObviousSecrets"],
        [nameof(RebalancedSpireSettings.WelcomeToWongos)] = ["WelcomeToWongos"],
        [nameof(RebalancedSpireSettings.WellLaidPlans)] = ["WellLaidPlans"],
        [nameof(RebalancedSpireSettings.WhisperingEarring)] = ["WhisperingEarring"],
        [nameof(RebalancedSpireSettings.Wisp)] = ["Wisp"],
        [nameof(RebalancedSpireSettings.WuppyInTrouble)] = ["WuppyInTrouble"],
    };

    internal static void Refresh(RebalancedSpireSettings previous, RebalancedSpireSettings current)
    {
        var names = new HashSet<string>();
        foreach (var property in RebalancedSpireSettingsSession.Properties)
        {
            if (!Equals(property.GetValue(previous), property.GetValue(current)) && Targets.TryGetValue(property.Name, out var targets))
            {
                names.UnionWith(targets);
            }
        }
        foreach (var model in ModelDb.All.ToArray())
        {
            if (!model.IsCanonical || !names.Contains(model.GetType().Name))
            {
                continue;
            }

            switch (model)
            {
                case CardModel card:
                {
                    card._dynamicVars = null;
                    card._energyCost = null;
                    card._keywords = null;
                    card._tags = null;
                    card._starCostSet = false;
                    break;
                }
                case RelicModel relic:
                {
                    relic._dynamicVars = null;
                    break;
                }
                case EventModel evt:
                {
                    evt._dynamicVars = null;
                    break;
                }
                case EnchantmentModel enchantment:
                {
                    enchantment._dynamicVars = null;
                    break;
                }
                case PowerModel power:
                {
                    power._dynamicVars = null;
                    break;
                }
                case EncounterModel encounter:
                {
                    encounter._monstersWithSlots = null;
                    break;
                }
                case RelicPoolModel pool:
                {
                    pool._relics = null;
                    pool._allRelicIds = null;
                    break;
                }
            }
        }
    }
}
