namespace RebalancedSpire.Core.Configs;

using STS2RitsuLib.Settings;

internal sealed class RebalancedSpireSettingsUiBindings
{
    public IModSettingsValueBinding<bool> UniformIntroGeneration { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LessContinuousMonsters { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LessPriceIncrease { get; private init; } = null!;

    public IModSettingsValueBinding<bool> BloodWall { get; private init; } = null!;
    public IModSettingsValueBinding<bool> ExpectAFight { get; private init; } = null!;
    public IModSettingsValueBinding<bool> ForgottenRitual { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Acrobatics { get; private init; } = null!;
    public IModSettingsValueBinding<bool> BladeOfInk { get; private init; } = null!;
    public IModSettingsValueBinding<bool> BouncingFlask { get; private init; } = null!;
    public IModSettingsValueBinding<bool> FlickFlack { get; private init; } = null!;
    public IModSettingsValueBinding<bool> HandTrick { get; private init; } = null!;
    public IModSettingsValueBinding<bool> HiddenDaggers { get; private init; } = null!;
    public IModSettingsValueBinding<bool> InfiniteBlades { get; private init; } = null!;
    public IModSettingsValueBinding<bool> MasterPlanner { get; private init; } = null!;
    public IModSettingsValueBinding<bool> PoisonedStab { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Scare { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Shadowmeld { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Untouchable { get; private init; } = null!;
    public IModSettingsValueBinding<bool> UpMySleeve { get; private init; } = null!;
    public IModSettingsValueBinding<bool> WellLaidPlans { get; private init; } = null!;
    public IModSettingsValueBinding<bool> ForegoneConclusion { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Glow { get; private init; } = null!;
    public IModSettingsValueBinding<bool> HeirloomHammer { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TheSealedThrone { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Afterlife { get; private init; } = null!;
    public IModSettingsValueBinding<bool> BansheesCry { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Debilitate { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Defy { get; private init; } = null!;
    public IModSettingsValueBinding<bool> GraveWarden { get; private init; } = null!;
    public IModSettingsValueBinding<bool> PullAggro { get; private init; } = null!;
    public IModSettingsValueBinding<bool> RightHandHand { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Seance { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SicEm { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Spur { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Wisp { get; private init; } = null!;
    public IModSettingsValueBinding<bool> ConsumingShadow { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Coolant { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Defragment { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Glasswork { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Leap { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Refract { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Spinner { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Synchronize { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Voltaic { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Bolas { get; private init; } = null!;
    public IModSettingsValueBinding<bool> EternalArmor { get; private init; } = null!;
    public IModSettingsValueBinding<bool> RollingBoulder { get; private init; } = null!;

    public IModSettingsValueBinding<bool> BoomingConch { get; private init; } = null!;
    public IModSettingsValueBinding<bool> FishingRod { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LargeCapsule { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LavaRock { get; private init; } = null!;
    public IModSettingsValueBinding<bool> NeowsLament { get; private init; } = null!;
    public IModSettingsValueBinding<bool> NeowsTalisman { get; private init; } = null!;
    public IModSettingsValueBinding<bool> NutritiousOyster { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Pomander { get; private init; } = null!;
    public IModSettingsValueBinding<bool> NeowAncientChoices { get; private init; } = null!;
    public IModSettingsValueBinding<bool> AlchemicalCoffer { get; private init; } = null!;
    public IModSettingsValueBinding<bool> OrobasAncientChoices { get; private init; } = null!;
    public IModSettingsValueBinding<bool> PaelsHorn { get; private init; } = null!;
    public IModSettingsValueBinding<bool> BiiigHug { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SealOfGold { get; private init; } = null!;
    public IModSettingsValueBinding<bool> ToastyMittens { get; private init; } = null!;
    public IModSettingsValueBinding<bool> DustyTome { get; private init; } = null!;
    public IModSettingsValueBinding<bool> DarvAncientChoices { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LoomingFruit { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Crossbow { get; private init; } = null!;
    public IModSettingsValueBinding<bool> WarHammer { get; private init; } = null!;

    public IModSettingsValueBinding<bool> BloodSoakedRose { get; private init; } = null!;
    public IModSettingsValueBinding<bool> ChoicesParadox { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Fiddle { get; private init; } = null!;
    public IModSettingsValueBinding<bool> PreservedFog { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SereTalon { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LordsParasol { get; private init; } = null!;
    public IModSettingsValueBinding<bool> WhisperingEarring { get; private init; } = null!;
    public IModSettingsValueBinding<bool> VakuuAncientChoices { get; private init; } = null!;
    public IModSettingsValueBinding<bool> VakuuFixedArt { get; private init; } = null!;

    public IModSettingsValueBinding<bool> MorphicGrove { get; private init; } = null!;
    public IModSettingsValueBinding<bool> PunchOff { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SpiralingWhirlpool { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SunkenStatue { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LostWisp { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SpiritGrafter { get; private init; } = null!;
    public IModSettingsValueBinding<bool> WelcomeToWongos { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TheLanternKey { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TeaMaster { get; private init; } = null!;
    public IModSettingsValueBinding<bool> HungryForMushrooms { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Reflections { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Trial { get; private init; } = null!;

    public IModSettingsValueBinding<bool> CubexConstruct { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Fogmog { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Flyconid { get; private init; } = null!;
    public IModSettingsValueBinding<bool> FuzzyWurmCrawler { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Inklet { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LeafSlimeS { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Nibbit { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SlitheringStrangler { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SnappingJaxfruit { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TwigSlimeM { get; private init; } = null!;
    public IModSettingsValueBinding<bool> VineShambler { get; private init; } = null!;
    public IModSettingsValueBinding<bool> BygoneEffigy { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Byrdonis { get; private init; } = null!;
    public IModSettingsValueBinding<bool> PhrogParasite { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Vantom { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TheKin { get; private init; } = null!;
    public IModSettingsValueBinding<bool> CeremonialBeast { get; private init; } = null!;
    public IModSettingsValueBinding<bool> CorpseSlug { get; private init; } = null!;
    public IModSettingsValueBinding<bool> CalcifiedCultist { get; private init; } = null!;
    public IModSettingsValueBinding<bool> DampCultist { get; private init; } = null!;
    public IModSettingsValueBinding<bool> FossilStalker { get; private init; } = null!;
    public IModSettingsValueBinding<bool> LivingFog { get; private init; } = null!;
    public IModSettingsValueBinding<bool> GremlinMerc { get; private init; } = null!;
    public IModSettingsValueBinding<bool> HauntedShip { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Seapunk { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SewerClam { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SludgeSpinner { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Toadpole { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TwoTailedRat { get; private init; } = null!;
    public IModSettingsValueBinding<bool> PhantasmalGardener { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SkulkingColony { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TerrorEel { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SoulFysh { get; private init; } = null!;
    public IModSettingsValueBinding<bool> WaterfallGiant { get; private init; } = null!;
    public IModSettingsValueBinding<bool> BowlbugRock { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Chomper { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Exoskeleton { get; private init; } = null!;
    public IModSettingsValueBinding<bool> HunterKiller { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Myte { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Ovicopter { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TheObscura { get; private init; } = null!;
    public IModSettingsValueBinding<bool> ThievingHopper { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Tunneler { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Decimillipede { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Entomancer { get; private init; } = null!;
    public IModSettingsValueBinding<bool> InfestedPrism { get; private init; } = null!;
    public IModSettingsValueBinding<bool> KaiserCrab { get; private init; } = null!;
    public IModSettingsValueBinding<bool> KnowledgeDemon { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TheInsatiable { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Fabricator { get; private init; } = null!;
    public IModSettingsValueBinding<bool> FrogKnight { get; private init; } = null!;
    public IModSettingsValueBinding<bool> GlobeHead { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TurretOperator { get; private init; } = null!;
    public IModSettingsValueBinding<bool> OwlMagistrate { get; private init; } = null!;
    public IModSettingsValueBinding<bool> ScrollOfBiting { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SlimedBerserker { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TheLostAndForgotten { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Knights { get; private init; } = null!;
    public IModSettingsValueBinding<bool> MechaKnight { get; private init; } = null!;
    public IModSettingsValueBinding<bool> SoulNexus { get; private init; } = null!;
    public IModSettingsValueBinding<bool> TestSubject { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Aeonglass { get; private init; } = null!;
    public IModSettingsValueBinding<bool> Doormaker { get; private init; } = null!;

    public static RebalancedSpireSettingsUiBindings Create()
    {
        var defaults = new RebalancedSpireSettings();
        return new RebalancedSpireSettingsUiBindings
        {
            UniformIntroGeneration = B(s => s.UniformIntroGeneration, (s, v) => s.UniformIntroGeneration = v, () => defaults.UniformIntroGeneration),
            LessContinuousMonsters = B(s => s.LessContinuousMonsters, (s, v) => s.LessContinuousMonsters = v, () => defaults.LessContinuousMonsters),
            LessPriceIncrease = B(s => s.LessPriceIncrease, (s, v) => s.LessPriceIncrease = v, () => defaults.LessPriceIncrease),

            BloodWall = B(s => s.BloodWall, (s, v) => s.BloodWall = v, () => defaults.BloodWall),
            ExpectAFight = B(s => s.ExpectAFight, (s, v) => s.ExpectAFight = v, () => defaults.ExpectAFight),
            ForgottenRitual = B(s => s.ForgottenRitual, (s, v) => s.ForgottenRitual = v, () => defaults.ForgottenRitual),
            Acrobatics = B(s => s.Acrobatics, (s, v) => s.Acrobatics = v, () => defaults.Acrobatics),
            BladeOfInk = B(s => s.BladeOfInk, (s, v) => s.BladeOfInk = v, () => defaults.BladeOfInk),
            BouncingFlask = B(s => s.BouncingFlask, (s, v) => s.BouncingFlask = v, () => defaults.BouncingFlask),
            FlickFlack = B(s => s.FlickFlack, (s, v) => s.FlickFlack = v, () => defaults.FlickFlack),
            HandTrick = B(s => s.HandTrick, (s, v) => s.HandTrick = v, () => defaults.HandTrick),
            HiddenDaggers = B(s => s.HiddenDaggers, (s, v) => s.HiddenDaggers = v, () => defaults.HiddenDaggers),
            InfiniteBlades = B(s => s.InfiniteBlades, (s, v) => s.InfiniteBlades = v, () => defaults.InfiniteBlades),
            MasterPlanner = B(s => s.MasterPlanner, (s, v) => s.MasterPlanner = v, () => defaults.MasterPlanner),
            PoisonedStab = B(s => s.PoisonedStab, (s, v) => s.PoisonedStab = v, () => defaults.PoisonedStab),
            Scare = B(s => s.Scare, (s, v) => s.Scare = v, () => defaults.Scare),
            Shadowmeld = B(s => s.Shadowmeld, (s, v) => s.Shadowmeld = v, () => defaults.Shadowmeld),
            Untouchable = B(s => s.Untouchable, (s, v) => s.Untouchable = v, () => defaults.Untouchable),
            UpMySleeve = B(s => s.UpMySleeve, (s, v) => s.UpMySleeve = v, () => defaults.UpMySleeve),
            WellLaidPlans = B(s => s.WellLaidPlans, (s, v) => s.WellLaidPlans = v, () => defaults.WellLaidPlans),
            ForegoneConclusion = B(s => s.ForegoneConclusion, (s, v) => s.ForegoneConclusion = v, () => defaults.ForegoneConclusion),
            Glow = B(s => s.Glow, (s, v) => s.Glow = v, () => defaults.Glow),
            HeirloomHammer = B(s => s.HeirloomHammer, (s, v) => s.HeirloomHammer = v, () => defaults.HeirloomHammer),
            TheSealedThrone = B(s => s.TheSealedThrone, (s, v) => s.TheSealedThrone = v, () => defaults.TheSealedThrone),
            Afterlife = B(s => s.Afterlife, (s, v) => s.Afterlife = v, () => defaults.Afterlife),
            BansheesCry = B(s => s.BansheesCry, (s, v) => s.BansheesCry = v, () => defaults.BansheesCry),
            Debilitate = B(s => s.Debilitate, (s, v) => s.Debilitate = v, () => defaults.Debilitate),
            Defy = B(s => s.Defy, (s, v) => s.Defy = v, () => defaults.Defy),
            GraveWarden = B(s => s.GraveWarden, (s, v) => s.GraveWarden = v, () => defaults.GraveWarden),
            PullAggro = B(s => s.PullAggro, (s, v) => s.PullAggro = v, () => defaults.PullAggro),
            RightHandHand = B(s => s.RightHandHand, (s, v) => s.RightHandHand = v, () => defaults.RightHandHand),
            Seance = B(s => s.Seance, (s, v) => s.Seance = v, () => defaults.Seance),
            SicEm = B(s => s.SicEm, (s, v) => s.SicEm = v, () => defaults.SicEm),
            Spur = B(s => s.Spur, (s, v) => s.Spur = v, () => defaults.Spur),
            Wisp = B(s => s.Wisp, (s, v) => s.Wisp = v, () => defaults.Wisp),
            ConsumingShadow = B(s => s.ConsumingShadow, (s, v) => s.ConsumingShadow = v, () => defaults.ConsumingShadow),
            Coolant = B(s => s.Coolant, (s, v) => s.Coolant = v, () => defaults.Coolant),
            Defragment = B(s => s.Defragment, (s, v) => s.Defragment = v, () => defaults.Defragment),
            Glasswork = B(s => s.Glasswork, (s, v) => s.Glasswork = v, () => defaults.Glasswork),
            Leap = B(s => s.Leap, (s, v) => s.Leap = v, () => defaults.Leap),
            Refract = B(s => s.Refract, (s, v) => s.Refract = v, () => defaults.Refract),
            Spinner = B(s => s.Spinner, (s, v) => s.Spinner = v, () => defaults.Spinner),
            Synchronize = B(s => s.Synchronize, (s, v) => s.Synchronize = v, () => defaults.Synchronize),
            Voltaic = B(s => s.Voltaic, (s, v) => s.Voltaic = v, () => defaults.Voltaic),
            Bolas = B(s => s.Bolas, (s, v) => s.Bolas = v, () => defaults.Bolas),
            EternalArmor = B(s => s.EternalArmor, (s, v) => s.EternalArmor = v, () => defaults.EternalArmor),
            RollingBoulder = B(s => s.RollingBoulder, (s, v) => s.RollingBoulder = v, () => defaults.RollingBoulder),

            BoomingConch = B(s => s.BoomingConch, (s, v) => s.BoomingConch = v, () => defaults.BoomingConch),
            FishingRod = B(s => s.FishingRod, (s, v) => s.FishingRod = v, () => defaults.FishingRod),
            LargeCapsule = B(s => s.LargeCapsule, (s, v) => s.LargeCapsule = v, () => defaults.LargeCapsule),
            LavaRock = B(s => s.LavaRock, (s, v) => s.LavaRock = v, () => defaults.LavaRock),
            NeowsLament = B(s => s.NeowsLament, (s, v) => s.NeowsLament = v, () => defaults.NeowsLament),
            NeowsTalisman = B(s => s.NeowsTalisman, (s, v) => s.NeowsTalisman = v, () => defaults.NeowsTalisman),
            NutritiousOyster = B(s => s.NutritiousOyster, (s, v) => s.NutritiousOyster = v, () => defaults.NutritiousOyster),
            Pomander = B(s => s.Pomander, (s, v) => s.Pomander = v, () => defaults.Pomander),
            NeowAncientChoices = B(s => s.NeowAncientChoices, (s, v) => s.NeowAncientChoices = v, () => defaults.NeowAncientChoices),
            AlchemicalCoffer = B(s => s.AlchemicalCoffer, (s, v) => s.AlchemicalCoffer = v, () => defaults.AlchemicalCoffer),
            OrobasAncientChoices = B(s => s.OrobasAncientChoices, (s, v) => s.OrobasAncientChoices = v, () => defaults.OrobasAncientChoices),
            PaelsHorn = B(s => s.PaelsHorn, (s, v) => s.PaelsHorn = v, () => defaults.PaelsHorn),
            BiiigHug = B(s => s.BiiigHug, (s, v) => s.BiiigHug = v, () => defaults.BiiigHug),
            SealOfGold = B(s => s.SealOfGold, (s, v) => s.SealOfGold = v, () => defaults.SealOfGold),
            ToastyMittens = B(s => s.ToastyMittens, (s, v) => s.ToastyMittens = v, () => defaults.ToastyMittens),
            DustyTome = B(s => s.DustyTome, (s, v) => s.DustyTome = v, () => defaults.DustyTome),
            DarvAncientChoices = B(s => s.DarvAncientChoices, (s, v) => s.DarvAncientChoices = v, () => defaults.DarvAncientChoices),
            LoomingFruit = B(s => s.LoomingFruit, (s, v) => s.LoomingFruit = v, () => defaults.LoomingFruit),
            Crossbow = B(s => s.Crossbow, (s, v) => s.Crossbow = v, () => defaults.Crossbow),
            WarHammer = B(s => s.WarHammer, (s, v) => s.WarHammer = v, () => defaults.WarHammer),
            BloodSoakedRose = B(s => s.BloodSoakedRose, (s, v) => s.BloodSoakedRose = v, () => defaults.BloodSoakedRose),
            ChoicesParadox = B(s => s.ChoicesParadox, (s, v) => s.ChoicesParadox = v, () => defaults.ChoicesParadox),
            Fiddle = B(s => s.Fiddle, (s, v) => s.Fiddle = v, () => defaults.Fiddle),
            PreservedFog = B(s => s.PreservedFog, (s, v) => s.PreservedFog = v, () => defaults.PreservedFog),
            SereTalon = B(s => s.SereTalon, (s, v) => s.SereTalon = v, () => defaults.SereTalon),
            LordsParasol = B(s => s.LordsParasol, (s, v) => s.LordsParasol = v, () => defaults.LordsParasol),
            WhisperingEarring = B(s => s.WhisperingEarring, (s, v) => s.WhisperingEarring = v, () => defaults.WhisperingEarring),
            VakuuAncientChoices = B(s => s.VakuuAncientChoices, (s, v) => s.VakuuAncientChoices = v, () => defaults.VakuuAncientChoices),
            VakuuFixedArt = B(s => s.VakuuFixedArt, (s, v) => s.VakuuFixedArt = v, () => defaults.VakuuFixedArt),

            MorphicGrove = B(s => s.MorphicGrove, (s, v) => s.MorphicGrove = v, () => defaults.MorphicGrove),
            PunchOff = B(s => s.PunchOff, (s, v) => s.PunchOff = v, () => defaults.PunchOff),
            SpiralingWhirlpool = B(s => s.SpiralingWhirlpool, (s, v) => s.SpiralingWhirlpool = v, () => defaults.SpiralingWhirlpool),
            SunkenStatue = B(s => s.SunkenStatue, (s, v) => s.SunkenStatue = v, () => defaults.SunkenStatue),
            LostWisp = B(s => s.LostWisp, (s, v) => s.LostWisp = v, () => defaults.LostWisp),
            SpiritGrafter = B(s => s.SpiritGrafter, (s, v) => s.SpiritGrafter = v, () => defaults.SpiritGrafter),
            WelcomeToWongos = B(s => s.WelcomeToWongos, (s, v) => s.WelcomeToWongos = v, () => defaults.WelcomeToWongos),
            TheLanternKey = B(s => s.TheLanternKey, (s, v) => s.TheLanternKey = v, () => defaults.TheLanternKey),
            TeaMaster = B(s => s.TeaMaster, (s, v) => s.TeaMaster = v, () => defaults.TeaMaster),
            HungryForMushrooms = B(s => s.HungryForMushrooms, (s, v) => s.HungryForMushrooms = v, () => defaults.HungryForMushrooms),
            Reflections = B(s => s.Reflections, (s, v) => s.Reflections = v, () => defaults.Reflections),
            Trial = B(s => s.Trial, (s, v) => s.Trial = v, () => defaults.Trial),

            CubexConstruct = B(s => s.CubexConstruct, (s, v) => s.CubexConstruct = v, () => defaults.CubexConstruct),
            Fogmog = B(s => s.Fogmog, (s, v) => s.Fogmog = v, () => defaults.Fogmog),
            Flyconid = B(s => s.Flyconid, (s, v) => s.Flyconid = v, () => defaults.Flyconid),
            FuzzyWurmCrawler = B(s => s.FuzzyWurmCrawler, (s, v) => s.FuzzyWurmCrawler = v, () => defaults.FuzzyWurmCrawler),
            Inklet = B(s => s.Inklet, (s, v) => s.Inklet = v, () => defaults.Inklet),
            LeafSlimeS = B(s => s.LeafSlimeS, (s, v) => s.LeafSlimeS = v, () => defaults.LeafSlimeS),
            Nibbit = B(s => s.Nibbit, (s, v) => s.Nibbit = v, () => defaults.Nibbit),
            SlitheringStrangler = B(s => s.SlitheringStrangler, (s, v) => s.SlitheringStrangler = v, () => defaults.SlitheringStrangler),
            SnappingJaxfruit = B(s => s.SnappingJaxfruit, (s, v) => s.SnappingJaxfruit = v, () => defaults.SnappingJaxfruit),
            TwigSlimeM = B(s => s.TwigSlimeM, (s, v) => s.TwigSlimeM = v, () => defaults.TwigSlimeM),
            VineShambler = B(s => s.VineShambler, (s, v) => s.VineShambler = v, () => defaults.VineShambler),
            BygoneEffigy = B(s => s.BygoneEffigy, (s, v) => s.BygoneEffigy = v, () => defaults.BygoneEffigy),
            Byrdonis = B(s => s.Byrdonis, (s, v) => s.Byrdonis = v, () => defaults.Byrdonis),
            PhrogParasite = B(s => s.PhrogParasite, (s, v) => s.PhrogParasite = v, () => defaults.PhrogParasite),
            Vantom = B(s => s.Vantom, (s, v) => s.Vantom = v, () => defaults.Vantom),
            TheKin = B(s => s.TheKin, (s, v) => s.TheKin = v, () => defaults.TheKin),
            CeremonialBeast = B(s => s.CeremonialBeast, (s, v) => s.CeremonialBeast = v, () => defaults.CeremonialBeast),
            CorpseSlug = B(s => s.CorpseSlug, (s, v) => s.CorpseSlug = v, () => defaults.CorpseSlug),
            CalcifiedCultist = B(s => s.CalcifiedCultist, (s, v) => s.CalcifiedCultist = v, () => defaults.CalcifiedCultist),
            DampCultist = B(s => s.DampCultist, (s, v) => s.DampCultist = v, () => defaults.DampCultist),
            FossilStalker = B(s => s.FossilStalker, (s, v) => s.FossilStalker = v, () => defaults.FossilStalker),
            LivingFog = B(s => s.LivingFog, (s, v) => s.LivingFog = v, () => defaults.LivingFog),
            GremlinMerc = B(s => s.GremlinMerc, (s, v) => s.GremlinMerc = v, () => defaults.GremlinMerc),
            HauntedShip = B(s => s.HauntedShip, (s, v) => s.HauntedShip = v, () => defaults.HauntedShip),
            Seapunk = B(s => s.Seapunk, (s, v) => s.Seapunk = v, () => defaults.Seapunk),
            SewerClam = B(s => s.SewerClam, (s, v) => s.SewerClam = v, () => defaults.SewerClam),
            SludgeSpinner = B(s => s.SludgeSpinner, (s, v) => s.SludgeSpinner = v, () => defaults.SludgeSpinner),
            Toadpole = B(s => s.Toadpole, (s, v) => s.Toadpole = v, () => defaults.Toadpole),
            TwoTailedRat = B(s => s.TwoTailedRat, (s, v) => s.TwoTailedRat = v, () => defaults.TwoTailedRat),
            PhantasmalGardener = B(s => s.PhantasmalGardener, (s, v) => s.PhantasmalGardener = v, () => defaults.PhantasmalGardener),
            SkulkingColony = B(s => s.SkulkingColony, (s, v) => s.SkulkingColony = v, () => defaults.SkulkingColony),
            TerrorEel = B(s => s.TerrorEel, (s, v) => s.TerrorEel = v, () => defaults.TerrorEel),
            SoulFysh = B(s => s.SoulFysh, (s, v) => s.SoulFysh = v, () => defaults.SoulFysh),
            WaterfallGiant = B(s => s.WaterfallGiant, (s, v) => s.WaterfallGiant = v, () => defaults.WaterfallGiant),
            BowlbugRock = B(s => s.BowlbugRock, (s, v) => s.BowlbugRock = v, () => defaults.BowlbugRock),
            Chomper = B(s => s.Chomper, (s, v) => s.Chomper = v, () => defaults.Chomper),
            Exoskeleton = B(s => s.Exoskeleton, (s, v) => s.Exoskeleton = v, () => defaults.Exoskeleton),
            HunterKiller = B(s => s.HunterKiller, (s, v) => s.HunterKiller = v, () => defaults.HunterKiller),
            Myte = B(s => s.Myte, (s, v) => s.Myte = v, () => defaults.Myte),
            Ovicopter = B(s => s.Ovicopter, (s, v) => s.Ovicopter = v, () => defaults.Ovicopter),
            TheObscura = B(s => s.TheObscura, (s, v) => s.TheObscura = v, () => defaults.TheObscura),
            ThievingHopper = B(s => s.ThievingHopper, (s, v) => s.ThievingHopper = v, () => defaults.ThievingHopper),
            Tunneler = B(s => s.Tunneler, (s, v) => s.Tunneler = v, () => defaults.Tunneler),
            Decimillipede = B(s => s.Decimillipede, (s, v) => s.Decimillipede = v, () => defaults.Decimillipede),
            Entomancer = B(s => s.Entomancer, (s, v) => s.Entomancer = v, () => defaults.Entomancer),
            InfestedPrism = B(s => s.InfestedPrism, (s, v) => s.InfestedPrism = v, () => defaults.InfestedPrism),
            KaiserCrab = B(s => s.KaiserCrab, (s, v) => s.KaiserCrab = v, () => defaults.KaiserCrab),
            KnowledgeDemon = B(s => s.KnowledgeDemon, (s, v) => s.KnowledgeDemon = v, () => defaults.KnowledgeDemon),
            TheInsatiable = B(s => s.TheInsatiable, (s, v) => s.TheInsatiable = v, () => defaults.TheInsatiable),
            Fabricator = B(s => s.Fabricator, (s, v) => s.Fabricator = v, () => defaults.Fabricator),
            FrogKnight = B(s => s.FrogKnight, (s, v) => s.FrogKnight = v, () => defaults.FrogKnight),
            GlobeHead = B(s => s.GlobeHead, (s, v) => s.GlobeHead = v, () => defaults.GlobeHead),
            TurretOperator = B(s => s.TurretOperator, (s, v) => s.TurretOperator = v, () => defaults.TurretOperator),
            OwlMagistrate = B(s => s.OwlMagistrate, (s, v) => s.OwlMagistrate = v, () => defaults.OwlMagistrate),
            ScrollOfBiting = B(s => s.ScrollOfBiting, (s, v) => s.ScrollOfBiting = v, () => defaults.ScrollOfBiting),
            SlimedBerserker = B(s => s.SlimedBerserker, (s, v) => s.SlimedBerserker = v, () => defaults.SlimedBerserker),
            TheLostAndForgotten = B(s => s.TheLostAndForgotten, (s, v) => s.TheLostAndForgotten = v, () => defaults.TheLostAndForgotten),
            Knights = B(s => s.Knights, (s, v) => s.Knights = v, () => defaults.Knights),
            MechaKnight = B(s => s.MechaKnight, (s, v) => s.MechaKnight = v, () => defaults.MechaKnight),
            SoulNexus = B(s => s.SoulNexus, (s, v) => s.SoulNexus = v, () => defaults.SoulNexus),
            TestSubject = B(s => s.TestSubject, (s, v) => s.TestSubject = v, () => defaults.TestSubject),
            Aeonglass = B(s => s.Aeonglass, (s, v) => s.Aeonglass = v, () => defaults.Aeonglass),
            Doormaker = B(s => s.Doormaker, (s, v) => s.Doormaker = v, () => defaults.Doormaker)
        };
    }

    private static DefaultModSettingsValueBinding<bool> B(Func<RebalancedSpireSettings, bool> getter, Action<RebalancedSpireSettings, bool> setter, Func<bool> defaultFactory)
    {
        return ModSettingsBindings.WithDefault(ModSettingsBindings.Global(RebalancedSpireMain.ModId, RebalancedSpireMain.SettingsKey, getter, setter), defaultFactory);
    }
}