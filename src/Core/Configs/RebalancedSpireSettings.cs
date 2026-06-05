namespace RebalancedSpire.Core.Configs;

using System.Text.Json.Serialization;
using JetBrains.Annotations;

public sealed class RebalancedSpireSettings
{
    public const int CurrentSchemaVersion = 1;

    [JsonPropertyName("schema_version")]
    [UsedImplicitly]
    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    [JsonPropertyName("uniform_intro_generation")]
    public bool UniformIntroGeneration { get; set; } = true;

    [JsonPropertyName("less_continuous_monsters")]
    public bool LessContinuousMonsters { get; set; } = true;

    [JsonPropertyName("less_price_increase")]
    public bool LessPriceIncrease { get; set; } = true;

    [JsonPropertyName("blood_wall")]
    public bool BloodWall { get; set; } = true;

    [JsonPropertyName("drum_of_battle")]
    public bool DrumOfBattle { get; set; } = true;

    [JsonPropertyName("expect_a_fight")]
    public bool ExpectAFight { get; set; } = true;

    [JsonPropertyName("forgotten_ritual")]
    public bool ForgottenRitual { get; set; } = true;

    [JsonPropertyName("acrobatics")]
    public bool Acrobatics { get; set; } = true;

    [JsonPropertyName("blade_of_ink")]
    public bool BladeOfInk { get; set; } = true;

    [JsonPropertyName("bouncing_flask")]
    public bool BouncingFlask { get; set; } = true;

    [JsonPropertyName("flick_flack")]
    public bool FlickFlack { get; set; } = true;

    [JsonPropertyName("hand_trick")]
    public bool HandTrick { get; set; } = true;

    [JsonPropertyName("hidden_daggers")]
    public bool HiddenDaggers { get; set; } = true;

    [JsonPropertyName("infinite_blades")]
    public bool InfiniteBlades { get; set; } = true;

    [JsonPropertyName("master_planner")]
    public bool MasterPlanner { get; set; } = true;

    [JsonPropertyName("poisoned_stab")]
    public bool PoisonedStab { get; set; } = true;

    [JsonPropertyName("scare")]
    public bool Scare { get; set; } = true;

    [JsonPropertyName("shadowmeld")]
    public bool Shadowmeld { get; set; } = true;

    [JsonPropertyName("untouchable")]
    public bool Untouchable { get; set; } = true;

    [JsonPropertyName("up_my_sleeve")]
    public bool UpMySleeve { get; set; } = true;

    [JsonPropertyName("well_laid_plans")]
    public bool WellLaidPlans { get; set; } = true;

    [JsonPropertyName("glow")]
    public bool Glow { get; set; } = true;

    [JsonPropertyName("the_sealed_throne")]
    public bool TheSealedThrone { get; set; } = true;

    [JsonPropertyName("afterlife")]
    public bool Afterlife { get; set; } = true;

    [JsonPropertyName("banshees_cry")]
    public bool BansheesCry { get; set; } = true;

    [JsonPropertyName("debilitate")]
    public bool Debilitate { get; set; } = true;

    [JsonPropertyName("defy")]
    public bool Defy { get; set; } = true;

    [JsonPropertyName("grave_warden")]
    public bool GraveWarden { get; set; } = true;

    [JsonPropertyName("pull_aggro")]
    public bool PullAggro { get; set; } = true;

    [JsonPropertyName("right_hand_hand")]
    public bool RightHandHand { get; set; } = true;

    [JsonPropertyName("seance")]
    public bool Seance { get; set; } = true;

    [JsonPropertyName("sic_em")]
    public bool SicEm { get; set; } = true;

    [JsonPropertyName("spur")]
    public bool Spur { get; set; } = true;

    [JsonPropertyName("wisp")]
    public bool Wisp { get; set; } = true;

    [JsonPropertyName("consuming_shadow")]
    public bool ConsumingShadow { get; set; } = true;

    [JsonPropertyName("coolant")]
    public bool Coolant { get; set; } = true;

    [JsonPropertyName("defragment")]
    public bool Defragment { get; set; } = true;

    [JsonPropertyName("glasswork")]
    public bool Glasswork { get; set; } = true;

    [JsonPropertyName("leap")]
    public bool Leap { get; set; } = true;

    [JsonPropertyName("refract")]
    public bool Refract { get; set; } = true;

    [JsonPropertyName("spinner")]
    public bool Spinner { get; set; } = true;

    [JsonPropertyName("synchronize")]
    public bool Synchronize { get; set; } = true;

    [JsonPropertyName("voltaic")]
    public bool Voltaic { get; set; } = true;

    [JsonPropertyName("bolas")]
    public bool Bolas { get; set; } = true;

    [JsonPropertyName("eternal_armor")]
    public bool EternalArmor { get; set; } = true;

    [JsonPropertyName("rolling_boulder")]
    public bool RollingBoulder { get; set; } = true;

    [JsonPropertyName("booming_conch")]
    public bool BoomingConch { get; set; } = true;

    [JsonPropertyName("fishing_rod")]
    public bool FishingRod { get; set; } = true;

    [JsonPropertyName("large_capsule")]
    public bool LargeCapsule { get; set; } = true;

    [JsonPropertyName("lava_rock")]
    public bool LavaRock { get; set; } = true;

    [JsonPropertyName("neows_lament")]
    public bool NeowsLament { get; set; } = true;

    [JsonPropertyName("neows_talisman")]
    public bool NeowsTalisman { get; set; } = true;

    [JsonPropertyName("nutritious_oyster")]
    public bool NutritiousOyster { get; set; } = true;

    [JsonPropertyName("pomander")]
    public bool Pomander { get; set; } = true;

    [JsonPropertyName("neow_ancient_choices")]
    public bool NeowAncientChoices { get; set; } = true;

    [JsonPropertyName("alchemical_coffer")]
    public bool AlchemicalCoffer { get; set; } = true;

    [JsonPropertyName("orobas_ancient_choices")]
    public bool OrobasAncientChoices { get; set; } = true;

    [JsonPropertyName("paels_horn")]
    public bool PaelsHorn { get; set; } = true;

    [JsonPropertyName("biiig_hug")]
    public bool BiiigHug { get; set; } = true;

    [JsonPropertyName("seal_of_gold")]
    public bool SealOfGold { get; set; } = true;

    [JsonPropertyName("toasty_mittens")]
    public bool ToastyMittens { get; set; } = true;

    [JsonPropertyName("dusty_tome")]
    public bool DustyTome { get; set; } = true;

    [JsonPropertyName("darv_ancient_choices")]
    public bool DarvAncientChoices { get; set; } = true;

    [JsonPropertyName("looming_fruit")]
    public bool LoomingFruit { get; set; } = true;

    [JsonPropertyName("crossbow")]
    public bool Crossbow { get; set; } = true;

    [JsonPropertyName("war_hammer")]
    public bool WarHammer { get; set; } = true;

    [JsonPropertyName("blood_soaked_rose")]
    public bool BloodSoakedRose { get; set; } = true;

    [JsonPropertyName("choices_paradox")]
    public bool ChoicesParadox { get; set; } = true;

    [JsonPropertyName("fiddle")]
    public bool Fiddle { get; set; } = true;

    [JsonPropertyName("preserved_fog")]
    public bool PreservedFog { get; set; } = true;

    [JsonPropertyName("sere_talon")]
    public bool SereTalon { get; set; } = true;

    [JsonPropertyName("lords_parasol")]
    public bool LordsParasol { get; set; } = true;

    [JsonPropertyName("whispering_earring")]
    public bool WhisperingEarring { get; set; } = true;

    [JsonPropertyName("vakuu_ancient_choices")]
    public bool VakuuAncientChoices { get; set; } = true;

    [JsonPropertyName("vakuu_fixed_art")]
    public bool VakuuFixedArt { get; set; } = true;

    [JsonPropertyName("morphic_grove")]
    public bool MorphicGrove { get; set; } = true;

    [JsonPropertyName("punch_off")]
    public bool PunchOff { get; set; } = true;

    [JsonPropertyName("spiraling_whirlpool")]
    public bool SpiralingWhirlpool { get; set; } = true;

    [JsonPropertyName("sunken_statue")]
    public bool SunkenStatue { get; set; } = true;

    [JsonPropertyName("lost_wisp")]
    public bool LostWisp { get; set; } = true;

    [JsonPropertyName("spirit_grafter")]
    public bool SpiritGrafter { get; set; } = true;

    [JsonPropertyName("welcome_to_wongos")]
    public bool WelcomeToWongos { get; set; } = true;

    [JsonPropertyName("the_lantern_key")]
    public bool TheLanternKey { get; set; } = true;

    [JsonPropertyName("tea_master")]
    public bool TeaMaster { get; set; } = true;

    [JsonPropertyName("hungry_for_mushrooms")]
    public bool HungryForMushrooms { get; set; } = true;

    [JsonPropertyName("reflections")]
    public bool Reflections { get; set; } = true;

    [JsonPropertyName("trial")]
    public bool Trial { get; set; } = true;

    [JsonPropertyName("cubex_construct")]
    public bool CubexConstruct { get; set; } = true;

    [JsonPropertyName("fogmog")]
    public bool Fogmog { get; set; } = true;

    [JsonPropertyName("flyconid")]
    public bool Flyconid { get; set; } = true;

    [JsonPropertyName("fuzzy_wurm_crawler")]
    public bool FuzzyWurmCrawler { get; set; } = true;

    [JsonPropertyName("inklet")]
    public bool Inklet { get; set; } = true;

    [JsonPropertyName("leaf_slime_s")]
    public bool LeafSlimeS { get; set; } = true;

    [JsonPropertyName("nibbit")]
    public bool Nibbit { get; set; } = true;

    [JsonPropertyName("slithering_strangler")]
    public bool SlitheringStrangler { get; set; } = true;

    [JsonPropertyName("snapping_jaxfruit")]
    public bool SnappingJaxfruit { get; set; } = true;

    [JsonPropertyName("twig_slime_m")]
    public bool TwigSlimeM { get; set; } = true;

    [JsonPropertyName("vine_shambler")]
    public bool VineShambler { get; set; } = true;

    [JsonPropertyName("bygone_effigy")]
    public bool BygoneEffigy { get; set; } = true;

    [JsonPropertyName("byrdonis")]
    public bool Byrdonis { get; set; } = true;

    [JsonPropertyName("phrog_parasite")]
    public bool PhrogParasite { get; set; } = true;

    [JsonPropertyName("vantom")]
    public bool Vantom { get; set; } = true;

    [JsonPropertyName("the_kin")]
    public bool TheKin { get; set; } = true;

    [JsonPropertyName("ceremonial_beast")]
    public bool CeremonialBeast { get; set; } = true;

    [JsonPropertyName("corpse_slug")]
    public bool CorpseSlug { get; set; } = true;

    [JsonPropertyName("calcified_cultist")]
    public bool CalcifiedCultist { get; set; } = true;

    [JsonPropertyName("damp_cultist")]
    public bool DampCultist { get; set; } = true;

    [JsonPropertyName("fossil_stalker")]
    public bool FossilStalker { get; set; } = true;

    [JsonPropertyName("living_fog")]
    public bool LivingFog { get; set; } = true;

    [JsonPropertyName("gremlin_merc")]
    public bool GremlinMerc { get; set; } = true;

    [JsonPropertyName("haunted_ship")]
    public bool HauntedShip { get; set; } = true;

    [JsonPropertyName("seapunk")]
    public bool Seapunk { get; set; } = true;

    [JsonPropertyName("sewer_clam")]
    public bool SewerClam { get; set; } = true;

    [JsonPropertyName("sludge_spinner")]
    public bool SludgeSpinner { get; set; } = true;

    [JsonPropertyName("toadpole")]
    public bool Toadpole { get; set; } = true;

    [JsonPropertyName("two_tailed_rat")]
    public bool TwoTailedRat { get; set; } = true;

    [JsonPropertyName("phantasmal_gardener")]
    public bool PhantasmalGardener { get; set; } = true;

    [JsonPropertyName("skulking_colony")]
    public bool SkulkingColony { get; set; } = true;

    [JsonPropertyName("terror_eel")]
    public bool TerrorEel { get; set; } = true;

    [JsonPropertyName("soul_fysh")]
    public bool SoulFysh { get; set; } = true;

    [JsonPropertyName("waterfall_giant")]
    public bool WaterfallGiant { get; set; } = true;

    [JsonPropertyName("bowlbug_rock")]
    public bool BowlbugRock { get; set; } = true;

    [JsonPropertyName("chomper")]
    public bool Chomper { get; set; } = true;

    [JsonPropertyName("exoskeleton")]
    public bool Exoskeleton { get; set; } = true;

    [JsonPropertyName("hunter_killer")]
    public bool HunterKiller { get; set; } = true;

    [JsonPropertyName("myte")]
    public bool Myte { get; set; } = true;

    [JsonPropertyName("ovicopter")]
    public bool Ovicopter { get; set; } = true;

    [JsonPropertyName("the_obscura")]
    public bool TheObscura { get; set; } = true;

    [JsonPropertyName("thieving_hopper")]
    public bool ThievingHopper { get; set; } = true;

    [JsonPropertyName("tunneler")]
    public bool Tunneler { get; set; } = true;

    [JsonPropertyName("decimillipede")]
    public bool Decimillipede { get; set; } = true;

    [JsonPropertyName("entomancer")]
    public bool Entomancer { get; set; } = true;

    [JsonPropertyName("infested_prism")]
    public bool InfestedPrism { get; set; } = true;

    [JsonPropertyName("kaiser_crab")]
    public bool KaiserCrab { get; set; } = true;

    [JsonPropertyName("knowledge_demon")]
    public bool KnowledgeDemon { get; set; } = true;

    [JsonPropertyName("the_insatiable")]
    public bool TheInsatiable { get; set; } = true;

    [JsonPropertyName("fabricator")]
    public bool Fabricator { get; set; } = true;

    [JsonPropertyName("frog_knight")]
    public bool FrogKnight { get; set; } = true;

    [JsonPropertyName("globe_head")]
    public bool GlobeHead { get; set; } = true;

    [JsonPropertyName("turret_operator")]
    public bool TurretOperator { get; set; } = true;

    [JsonPropertyName("owl_magistrate")]
    public bool OwlMagistrate { get; set; } = true;

    [JsonPropertyName("scroll_of_biting")]
    public bool ScrollOfBiting { get; set; } = true;

    [JsonPropertyName("slimed_berserker")]
    public bool SlimedBerserker { get; set; } = true;

    [JsonPropertyName("the_lost_and_forgotten")]
    public bool TheLostAndForgotten { get; set; } = true;

    [JsonPropertyName("knights")]
    public bool Knights { get; set; } = true;

    [JsonPropertyName("mecha_knight")]
    public bool MechaKnight { get; set; } = true;

    [JsonPropertyName("soul_nexus")]
    public bool SoulNexus { get; set; } = true;

    [JsonPropertyName("test_subject")]
    public bool TestSubject { get; set; } = true;

    [JsonPropertyName("aeonglass")]
    public bool Aeonglass { get; set; } = true;

    [JsonPropertyName("doormaker")]
    public bool Doormaker { get; set; } = true;
}