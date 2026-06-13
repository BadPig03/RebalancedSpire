namespace RebalancedSpire.Core.Registry;

using Configs;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib;
using STS2RitsuLib.Settings;

internal sealed class RebalancedSpireSettingsRegistry
{
    internal static void Initialize()
    {
        var ui = RebalancedSpireSettingsUiBindings.Create();
        RebalancedSpireSettingsStore.Initialize();

        RitsuLibFramework.RegisterModSettings(RebalancedSpireMain.ModId, p => p
            .WithModDisplayName(L("mod.title", "Rebalanced Spire"))
            .WithTitle(L("others_settings.page.title"))
            .WithDescription(L("others_settings.page.description"))
            .WithSortOrder(10000)
            .WithReadOnlyOnHostSurfaces(ModSettingsHostSurface.RunPause | ModSettingsHostSurface.CombatPause)
            .AddSection("map_generation", s => s
                .WithTitle(L("map_generation.section.label"))
                .AddToggle("uniform_intro_generation", L("map_generation.section.uniform_intro_generation.title"), ui.UniformIntroGeneration, L("map_generation.section.uniform_intro_generation.description"))
                .AddToggle("less_continuous_monsters", L("map_generation.section.less_continuous_monsters.title"), ui.LessContinuousMonsters, L("map_generation.section.less_continuous_monsters.description"))
            ).AddSection("merchant", s => s
                .WithTitle(L("merchant.section.label"))
                .AddToggle("less_price_increase", L("merchant.section.less_price_increase.title"), ui.LessPriceIncrease, L("merchant.section.less_price_increase.description"))
            ), "others"
        );

        RitsuLibFramework.RegisterModSettings(RebalancedSpireMain.ModId, p => p
            .WithModDisplayName(L("mod.title", "Rebalanced Spire"))
            .WithTitle(L("cards_settings.page.title"))
            .WithDescription(L("cards_settings.page.description"))
            .WithSortOrder(0)
            .WithReadOnlyOnHostSurfaces(ModSettingsHostSurface.RunPause | ModSettingsHostSurface.CombatPause)
            .AddSection("the_ironclad", s => s
                .WithTitle(L("the_ironclad.section.label"))
                .AddToggle("blood_wall", L("the_ironclad.section.blood_wall.title"), ui.BloodWall, L("the_ironclad.section.blood_wall.description"))
                .AddToggle("expect_a_fight", L("the_ironclad.section.expect_a_fight.title"), ui.ExpectAFight, L("the_ironclad.section.expect_a_fight.description"))
                .AddToggle("forgotten_ritual", L("the_ironclad.section.forgotten_ritual.title"), ui.ForgottenRitual, L("the_ironclad.section.forgotten_ritual.description"))
            ).AddSection("the_silent", s => s
                .WithTitle(L("the_silent.section.label"))
                .AddToggle("acrobatics", L("the_silent.section.acrobatics.title"), ui.Acrobatics, L("the_silent.section.acrobatics.description"))
                .AddToggle("blade_of_ink", L("the_silent.section.blade_of_ink.title"), ui.BladeOfInk, L("the_silent.section.blade_of_ink.description"))
                .AddToggle("bouncing_flask", L("the_silent.section.bouncing_flask.title"), ui.BouncingFlask, L("the_silent.section.bouncing_flask.description"))
                .AddToggle("flick_flack", L("the_silent.section.flick_flack.title"), ui.FlickFlack, L("the_silent.section.flick_flack.description"))
                .AddToggle("hand_trick", L("the_silent.section.hand_trick.title"), ui.HandTrick, L("the_silent.section.hand_trick.description"))
                .AddToggle("hidden_daggers", L("the_silent.section.hidden_daggers.title"), ui.HiddenDaggers, L("the_silent.section.hidden_daggers.description"))
                .AddToggle("infinite_blades", L("the_silent.section.infinite_blades.title"), ui.InfiniteBlades, L("the_silent.section.infinite_blades.description"))
                .AddToggle("master_planner", L("the_silent.section.master_planner.title"), ui.MasterPlanner, L("the_silent.section.master_planner.description"))
                .AddToggle("poisoned_stab", L("the_silent.section.poisoned_stab.title"), ui.PoisonedStab, L("the_silent.section.poisoned_stab.description"))
                .AddToggle("scare", L("the_silent.section.scare.title"), ui.Scare, L("the_silent.section.scare.description"))
                .AddToggle("shadowmeld", L("the_silent.section.shadowmeld.title"), ui.Shadowmeld, L("the_silent.section.shadowmeld.description"))
                .AddToggle("untouchable", L("the_silent.section.untouchable.title"), ui.Untouchable, L("the_silent.section.untouchable.description"))
                .AddToggle("up_my_sleeve", L("the_silent.section.up_my_sleeve.title"), ui.UpMySleeve, L("the_silent.section.up_my_sleeve.description"))
                .AddToggle("well_laid_plans", L("the_silent.section.well_laid_plans.title"), ui.WellLaidPlans, L("the_silent.section.well_laid_plans.description"))
            ).AddSection("the_regent", s => s
                .WithTitle(L("the_regent.section.label"))
                .AddToggle("foregone_conclusion", L("the_regent.section.foregone_conclusion.title"), ui.ForegoneConclusion, L("the_regent.section.foregone_conclusion.description"))
                .AddToggle("genesis", L("the_regent.section.genesis.title"), ui.Genesis, L("the_regent.section.genesis.description"))
                .AddToggle("glow", L("the_regent.section.glow.title"), ui.Glow, L("the_regent.section.glow.description"))
                .AddToggle("heirloom_hammer", L("the_regent.section.heirloom_hammer.title"), ui.HeirloomHammer, L("the_regent.section.heirloom_hammer.description"))
                .AddToggle("the_sealed_throne", L("the_regent.section.the_sealed_throne.title"), ui.TheSealedThrone, L("the_regent.section.the_sealed_throne.description"))
            ).AddSection("the_necrobinder", s => s
                .WithTitle(L("the_necrobinder.section.label"))
                .AddToggle("afterlife", L("the_necrobinder.section.afterlife.title"), ui.Afterlife, L("the_necrobinder.section.afterlife.description"))
                .AddToggle("banshees_cry", L("the_necrobinder.section.banshees_cry.title"), ui.BansheesCry, L("the_necrobinder.section.banshees_cry.description"))
                .AddToggle("debilitate", L("the_necrobinder.section.debilitate.title"), ui.Debilitate, L("the_necrobinder.section.debilitate.description"))
                .AddToggle("defy", L("the_necrobinder.section.defy.title"), ui.Defy, L("the_necrobinder.section.defy.description"))
                .AddToggle("grave_warden", L("the_necrobinder.section.grave_warden.title"), ui.GraveWarden, L("the_necrobinder.section.grave_warden.description"))
                .AddToggle("pull_aggro", L("the_necrobinder.section.pull_aggro.title"), ui.PullAggro, L("the_necrobinder.section.pull_aggro.description"))
                .AddToggle("right_hand_hand", L("the_necrobinder.section.right_hand_hand.title"), ui.RightHandHand, L("the_necrobinder.section.right_hand_hand.description"))
                .AddToggle("seance", L("the_necrobinder.section.seance.title"), ui.Seance, L("the_necrobinder.section.seance.description"))
                .AddToggle("sic_em", L("the_necrobinder.section.sic_em.title"), ui.SicEm, L("the_necrobinder.section.sic_em.description"))
                .AddToggle("spur", L("the_necrobinder.section.spur.title"), ui.Spur, L("the_necrobinder.section.spur.description"))
                .AddToggle("wisp", L("the_necrobinder.section.wisp.title"), ui.Wisp, L("the_necrobinder.section.wisp.description"))
            ).AddSection("the_defect", s => s
                .WithTitle(L("the_defect.section.label"))
                .AddToggle("consuming_shadow", L("the_defect.section.consuming_shadow.title"), ui.ConsumingShadow, L("the_defect.section.consuming_shadow.description"))
                .AddToggle("coolant", L("the_defect.section.coolant.title"), ui.Coolant, L("the_defect.section.coolant.description"))
                .AddToggle("defragment", L("the_defect.section.defragment.title"), ui.Defragment, L("the_defect.section.defragment.description"))
                .AddToggle("glasswork", L("the_defect.section.glasswork.title"), ui.Glasswork, L("the_defect.section.glasswork.description"))
                .AddToggle("leap", L("the_defect.section.leap.title"), ui.Leap, L("the_defect.section.leap.description"))
                .AddToggle("refract", L("the_defect.section.refract.title"), ui.Refract, L("the_defect.section.refract.description"))
                .AddToggle("spinner", L("the_defect.section.spinner.title"), ui.Spinner, L("the_defect.section.spinner.description"))
                .AddToggle("synchronize", L("the_defect.section.synchronize.title"), ui.Synchronize, L("the_defect.section.synchronize.description"))
                .AddToggle("voltaic", L("the_defect.section.voltaic.title"), ui.Voltaic, L("the_defect.section.voltaic.description"))
            ).AddSection("the_colorless", s => s
                .WithTitle(L("the_colorless.section.label"))
                .AddToggle("bolas", L("the_colorless.section.bolas.title"), ui.Bolas, L("the_colorless.section.bolas.description"))
                .AddToggle("eternal_armor", L("the_colorless.section.eternal_armor.title"), ui.EternalArmor, L("the_colorless.section.eternal_armor.description"))
                .AddToggle("rolling_boulder", L("the_colorless.section.rolling_boulder.title"), ui.RollingBoulder, L("the_colorless.section.rolling_boulder.description"))
            ), "cards"
        );

        RitsuLibFramework.RegisterModSettings(RebalancedSpireMain.ModId, p => p
            .WithModDisplayName(L("mod.title", "Rebalanced Spire"))
            .WithTitle(L("relics_settings.page.title"))
            .WithDescription(L("relics_settings.page.description"))
            .WithSortOrder(1)
            .WithReadOnlyOnHostSurfaces(ModSettingsHostSurface.RunPause | ModSettingsHostSurface.CombatPause)
            .AddSection("neow", s => s
                .WithTitle(L("neow.section.label"))
                .AddToggle("booming_conch", L("neow.section.booming_conch.title"), ui.BoomingConch, L("neow.section.booming_conch.description"))
                .AddToggle("fishing_rod", L("neow.section.fishing_rod.title"), ui.FishingRod, L("neow.section.fishing_rod.description"))
                .AddToggle("large_capsule", L("neow.section.large_capsule.title"), ui.LargeCapsule, L("neow.section.large_capsule.description"))
                .AddToggle("lava_rock", L("neow.section.lava_rock.title"), ui.LavaRock, L("neow.section.lava_rock.description"))
                .AddToggle("neows_lament", L("neow.section.neows_lament.title"), ui.NeowsLament, L("neow.section.neows_lament.description"))
                .AddToggle("neows_talisman", L("neow.section.neows_talisman.title"), ui.NeowsTalisman, L("neow.section.neows_talisman.description"))
                .AddToggle("nutritious_oyster", L("neow.section.nutritious_oyster.title"), ui.NutritiousOyster, L("neow.section.nutritious_oyster.description"))
                .AddToggle("pomander", L("neow.section.pomander.title"), ui.Pomander, L("neow.section.pomander.description"))
                .AddToggle("neow_ancient_choices", L("neow.section.neow_ancient_choices.title"), ui.NeowAncientChoices, L("neow.section.neow_ancient_choices.description"))
            ).AddSection("orobas", s => s
                .WithTitle(L("orobas.section.label"))
                .AddToggle("alchemical_coffer", L("orobas.section.alchemical_coffer.title"), ui.AlchemicalCoffer, L("orobas.section.alchemical_coffer.description"))
                .AddToggle("orobas_ancient_choices", L("orobas.section.orobas_ancient_choices.title"), ui.OrobasAncientChoices, L("orobas.section.orobas_ancient_choices.description"))
            ).AddSection("pael", s => s
                .WithTitle(L("pael.section.label"))
                .AddToggle("paels_horn", L("pael.section.paels_horn.title"), ui.PaelsHorn, L("pael.section.paels_horn.description"))
            ).AddSection("tezcatara", s => s
                .WithTitle(L("tezcatara.section.label"))
                .AddToggle("biiig_hug", L("tezcatara.section.biiig_hug.title"), ui.BiiigHug, L("tezcatara.section.biiig_hug.description"))
                .AddToggle("seal_of_gold", L("tezcatara.section.seal_of_gold.title"), ui.SealOfGold, L("tezcatara.section.seal_of_gold.description"))
                .AddToggle("toasty_mittens", L("tezcatara.section.toasty_mittens.title"), ui.ToastyMittens, L("tezcatara.section.toasty_mittens.description"))
            ).AddSection("darv", s => s
                .WithTitle(L("darv.section.label"))
                .AddToggle("dusty_tome", L("darv.section.dusty_tome.title"), ui.DustyTome, L("darv.section.dusty_tome.description"))
                .AddToggle("darv_ancient_choices", L("darv.section.darv_ancient_choices.title"), ui.DarvAncientChoices, L("darv.section.darv_ancient_choices.description"))
            ).AddSection("nonupeipe", s => s
                .WithTitle(L("nonupeipe.section.label"))
                .AddToggle("looming_fruit", L("nonupeipe.section.looming_fruit.title"), ui.LoomingFruit, L("nonupeipe.section.looming_fruit.description"))
            ).AddSection("tanx", s => s
                .WithTitle(L("tanx.section.label"))
                .AddToggle("crossbow", L("tanx.section.crossbow.title"), ui.Crossbow, L("tanx.section.crossbow.description"))
                .AddToggle("war_hammer", L("tanx.section.war_hammer.title"), ui.WarHammer, L("tanx.section.war_hammer.description"))
            ).AddSection("vakuu", s => s
                .WithTitle(L("vakuu.section.label"))
                .AddToggle("blood_soaked_rose", L("vakuu.section.blood_soaked_rose.title"), ui.BloodSoakedRose, L("vakuu.section.blood_soaked_rose.description"))
                .AddToggle("choices_paradox", L("vakuu.section.choices_paradox.title"), ui.ChoicesParadox, L("vakuu.section.choices_paradox.description"))
                .AddToggle("fiddle", L("vakuu.section.fiddle.title"), ui.Fiddle, L("vakuu.section.fiddle.description"))
                .AddToggle("preserved_fog", L("vakuu.section.preserved_fog.title"), ui.PreservedFog, L("vakuu.section.preserved_fog.description"))
                .AddToggle("sere_talon", L("vakuu.section.sere_talon.title"), ui.SereTalon, L("vakuu.section.sere_talon.description"))
                .AddToggle("lords_parasol", L("vakuu.section.lords_parasol.title"), ui.LordsParasol, L("vakuu.section.lords_parasol.description"))
                .AddToggle("whispering_earring", L("vakuu.section.whispering_earring.title"), ui.WhisperingEarring, L("vakuu.section.whispering_earring.description"))
                .AddToggle("vakuu_ancient_choices", L("vakuu.section.vakuu_ancient_choices.title"), ui.VakuuAncientChoices, L("vakuu.section.vakuu_ancient_choices.description"))
                .AddToggle("vakuu_fixed_art", L("vakuu.section.vakuu_fixed_art.title"), ui.VakuuFixedArt, L("vakuu.section.vakuu_fixed_art.description"))
            ), "relics"
        );

        RitsuLibFramework.RegisterModSettings(RebalancedSpireMain.ModId, p => p
            .WithModDisplayName(L("mod.title", "Rebalanced Spire"))
            .WithTitle(L("events_settings.page.title"))
            .WithDescription(L("events_settings.page.description"))
            .WithSortOrder(2)
            .WithReadOnlyOnHostSurfaces(ModSettingsHostSurface.RunPause | ModSettingsHostSurface.CombatPause)
            .AddSection("overgrowth", s => s
                .WithTitle(L("overgrowth.section.label"))
                .AddToggle("morphic_grove", L("overgrowth.section.morphic_grove.title"), ui.MorphicGrove, L("overgrowth.section.morphic_grove.description"))
            ).AddSection("underdocks", s => s
                .WithTitle(L("underdocks.section.label"))
                .AddToggle("punch_off", L("underdocks.section.punch_off.title"), ui.PunchOff, L("underdocks.section.punch_off.description"))
                .AddToggle("spiraling_whirlpool", L("underdocks.section.spiraling_whirlpool.title"), ui.SpiralingWhirlpool, L("underdocks.section.spiraling_whirlpool.description"))
            ).AddSection("act1", s => s
                .WithTitle(L("act1.section.label"))
                .AddToggle("sunken_statue", L("act1.section.sunken_statue.title"), ui.SunkenStatue, L("act1.section.sunken_statue.description"))
            ).AddSection("hive", s => s
                .WithTitle(L("hive.section.label"))
                .AddToggle("lost_wisp", L("hive.section.lost_wisp.title"), ui.LostWisp, L("hive.section.lost_wisp.description"))
                .AddToggle("spirit_grafter", L("hive.section.spirit_grafter.title"), ui.SpiritGrafter, L("hive.section.spirit_grafter.description"))
                .AddToggle("the_lantern_key", L("hive.section.the_lantern_key.title"), ui.TheLanternKey, L("hive.section.the_lantern_key.description"))
                .AddToggle("welcome_to_wongos", L("hive.section.welcome_to_wongos.title"), ui.WelcomeToWongos, L("hive.section.welcome_to_wongos.description"))
            ).AddSection("act1and2", s => s
                .WithTitle(L("act1and2.section.label"))
                .AddToggle("tea_master", L("act1and2.section.tea_master.title"), ui.TeaMaster, L("act1and2.section.tea_master.description"))
            ).AddSection("glory", s => s
                .WithTitle(L("glory.section.label"))
                .AddToggle("hungry_for_mushrooms", L("glory.section.hungry_for_mushrooms.title"), ui.HungryForMushrooms, L("glory.section.hungry_for_mushrooms.description"))
                .AddToggle("reflections", L("glory.section.reflections.title"), ui.Reflections, L("glory.section.reflections.description"))
                .AddToggle("trial", L("glory.section.trial.title"), ui.Trial, L("glory.section.trial.description"))
            ), "events"
        );

        RitsuLibFramework.RegisterModSettings(RebalancedSpireMain.ModId, p => p
            .WithModDisplayName(L("mod.title", "Rebalanced Spire"))
            .WithTitle(L("enemies_settings.page.title"))
            .WithDescription(L("enemies_settings.page.description"))
            .WithSortOrder(3)
            .WithReadOnlyOnHostSurfaces(ModSettingsHostSurface.RunPause | ModSettingsHostSurface.CombatPause)
            .AddSection("overgrowth", s => s
                .WithTitle(L("overgrowth.section.label"))
                .AddToggle("cubex_construct", L("overgrowth.section.cubex_construct.title"), ui.CubexConstruct, L("overgrowth.section.cubex_construct.description"))
                .AddToggle("fogmog", L("overgrowth.section.fogmog.title"), ui.Fogmog, L("overgrowth.section.fogmog.description"))
                .AddToggle("flyconid", L("overgrowth.section.flyconid.title"), ui.Flyconid, L("overgrowth.section.flyconid.description"))
                .AddToggle("fuzzy_wurm_crawler", L("overgrowth.section.fuzzy_wurm_crawler.title"), ui.FuzzyWurmCrawler, L("overgrowth.section.fuzzy_wurm_crawler.description"))
                .AddToggle("inklet", L("overgrowth.section.inklet.title"), ui.Inklet, L("overgrowth.section.inklet.description"))
                .AddToggle("leaf_slime_s", L("overgrowth.section.leaf_slime_s.title"), ui.LeafSlimeS, L("overgrowth.section.leaf_slime_s.description"))
                .AddToggle("nibbit", L("overgrowth.section.nibbit.title"), ui.Nibbit, L("overgrowth.section.nibbit.description"))
                .AddToggle("ruby_raiders", L("overgrowth.section.ruby_raiders.title"), ui.RubyRaiders, L("overgrowth.section.ruby_raiders.description"))
                .AddToggle("slithering_strangler", L("overgrowth.section.slithering_strangler.title"), ui.SlitheringStrangler, L("overgrowth.section.slithering_strangler.description"))
                .AddToggle("snapping_jaxfruit", L("overgrowth.section.snapping_jaxfruit.title"), ui.SnappingJaxfruit, L("overgrowth.section.snapping_jaxfruit.description"))
                .AddToggle("twig_slime_m", L("overgrowth.section.twig_slime_m.title"), ui.TwigSlimeM, L("overgrowth.section.twig_slime_m.description"))
                .AddToggle("vine_shambler", L("overgrowth.section.vine_shambler.title"), ui.VineShambler, L("overgrowth.section.vine_shambler.description"))
                .AddToggle("bygone_effigy", L("overgrowth.section.bygone_effigy.title"), ui.BygoneEffigy, L("overgrowth.section.bygone_effigy.description"))
                .AddToggle("byrdonis", L("overgrowth.section.byrdonis.title"), ui.Byrdonis, L("overgrowth.section.byrdonis.description"))
                .AddToggle("phrog_parasite", L("overgrowth.section.phrog_parasite.title"), ui.PhrogParasite, L("overgrowth.section.phrog_parasite.description"))
                .AddToggle("vantom", L("overgrowth.section.vantom.title"), ui.Vantom, L("overgrowth.section.vantom.description"))
                .AddToggle("the_kin", L("overgrowth.section.the_kin.title"), ui.TheKin, L("overgrowth.section.the_kin.description"))
                .AddToggle("ceremonial_beast", L("overgrowth.section.ceremonial_beast.title"), ui.CeremonialBeast, L("overgrowth.section.ceremonial_beast.description"))
            ).AddSection("underdocks", s => s
                .WithTitle(L("underdocks.section.label"))
                .AddToggle("corpse_slug", L("underdocks.section.corpse_slug.title"), ui.CorpseSlug, L("underdocks.section.corpse_slug.description"))
                .AddToggle("calcified_cultist", L("underdocks.section.calcified_cultist.title"), ui.CalcifiedCultist, L("underdocks.section.calcified_cultist.description"))
                .AddToggle("damp_cultist", L("underdocks.section.damp_cultist.title"), ui.DampCultist, L("underdocks.section.damp_cultist.description"))
                .AddToggle("fossil_stalker", L("underdocks.section.fossil_stalker.title"), ui.FossilStalker, L("underdocks.section.fossil_stalker.description"))
                .AddToggle("living_fog", L("underdocks.section.living_fog.title"), ui.LivingFog, L("underdocks.section.living_fog.description"))
                .AddToggle("gremlin_merc", L("underdocks.section.gremlin_merc.title"), ui.GremlinMerc, L("underdocks.section.gremlin_merc.description"))
                .AddToggle("haunted_ship", L("underdocks.section.haunted_ship.title"), ui.HauntedShip, L("underdocks.section.haunted_ship.description"))
                .AddToggle("seapunk", L("underdocks.section.seapunk.title"), ui.Seapunk, L("underdocks.section.seapunk.description"))
                .AddToggle("sewer_clam", L("underdocks.section.sewer_clam.title"), ui.SewerClam, L("underdocks.section.sewer_clam.description"))
                .AddToggle("sludge_spinner", L("underdocks.section.sludge_spinner.title"), ui.SludgeSpinner, L("underdocks.section.sludge_spinner.description"))
                .AddToggle("toadpole", L("underdocks.section.toadpole.title"), ui.Toadpole, L("underdocks.section.toadpole.description"))
                .AddToggle("two_tailed_rat", L("underdocks.section.two_tailed_rat.title"), ui.TwoTailedRat, L("underdocks.section.two_tailed_rat.description"))
                .AddToggle("phantasmal_gardener", L("underdocks.section.phantasmal_gardener.title"), ui.PhantasmalGardener, L("underdocks.section.phantasmal_gardener.description"))
                .AddToggle("skulking_colony", L("underdocks.section.skulking_colony.title"), ui.SkulkingColony, L("underdocks.section.skulking_colony.description"))
                .AddToggle("terror_eel", L("underdocks.section.terror_eel.title"), ui.TerrorEel, L("underdocks.section.terror_eel.description"))
                .AddToggle("soul_fysh", L("underdocks.section.soul_fysh.title"), ui.SoulFysh, L("underdocks.section.soul_fysh.description"))
                .AddToggle("waterfall_giant", L("underdocks.section.waterfall_giant.title"), ui.WaterfallGiant, L("underdocks.section.waterfall_giant.description"))
            ).AddSection("hive", s => s
                .WithTitle(L("hive.section.label"))
                .AddToggle("bowlbug_rock", L("hive.section.bowlbug_rock.title"), ui.BowlbugRock, L("hive.section.bowlbug_rock.description"))
                .AddToggle("chomper", L("hive.section.chomper.title"), ui.Chomper, L("hive.section.chomper.description"))
                .AddToggle("exoskeleton", L("hive.section.exoskeleton.title"), ui.Exoskeleton, L("hive.section.exoskeleton.description"))
                .AddToggle("hunter_killer", L("hive.section.hunter_killer.title"), ui.HunterKiller, L("hive.section.hunter_killer.description"))
                .AddToggle("myte", L("hive.section.myte.title"), ui.Myte, L("hive.section.myte.description"))
                .AddToggle("ovicopter", L("hive.section.ovicopter.title"), ui.Ovicopter, L("hive.section.ovicopter.description"))
                .AddToggle("the_obscura", L("hive.section.the_obscura.title"), ui.TheObscura, L("hive.section.the_obscura.description"))
                .AddToggle("thieving_hopper", L("hive.section.thieving_hopper.title"), ui.ThievingHopper, L("hive.section.thieving_hopper.description"))
                .AddToggle("tunneler", L("hive.section.tunneler.title"), ui.Tunneler, L("hive.section.tunneler.description"))
                .AddToggle("decimillipede", L("hive.section.decimillipede.title"), ui.Decimillipede, L("hive.section.decimillipede.description"))
                .AddToggle("entomancer", L("hive.section.entomancer.title"), ui.Entomancer, L("hive.section.entomancer.description"))
                .AddToggle("infested_prism", L("hive.section.infested_prism.title"), ui.InfestedPrism, L("hive.section.infested_prism.description"))
                .AddToggle("kaiser_crab", L("hive.section.kaiser_crab.title"), ui.KaiserCrab, L("hive.section.kaiser_crab.description"))
                .AddToggle("knowledge_demon", L("hive.section.knowledge_demon.title"), ui.KnowledgeDemon, L("hive.section.knowledge_demon.description"))
                .AddToggle("the_insatiable", L("hive.section.the_insatiable.title"), ui.TheInsatiable, L("hive.section.the_insatiable.description"))
            ).AddSection("glory", s => s
                .WithTitle(L("glory.section.label"))
                .AddToggle("fabricator", L("glory.section.fabricator.title"), ui.Fabricator, L("glory.section.fabricator.description"))
                .AddToggle("frog_knight", L("glory.section.frog_knight.title"), ui.FrogKnight, L("glory.section.frog_knight.description"))
                .AddToggle("globe_head", L("glory.section.globe_head.title"), ui.GlobeHead, L("glory.section.globe_head.description"))
                .AddToggle("turret_operator", L("glory.section.turret_operator.title"), ui.TurretOperator, L("glory.section.turret_operator.description"))
                .AddToggle("owl_magistrate", L("glory.section.owl_magistrate.title"), ui.OwlMagistrate, L("glory.section.owl_magistrate.description"))
                .AddToggle("scroll_of_biting", L("glory.section.scroll_of_biting.title"), ui.ScrollOfBiting, L("glory.section.scroll_of_biting.description"))
                .AddToggle("slimed_berserker", L("glory.section.slimed_berserker.title"), ui.SlimedBerserker, L("glory.section.slimed_berserker.description"))
                .AddToggle("the_lost_and_forgotten", L("glory.section.the_lost_and_forgotten.title"), ui.TheLostAndForgotten, L("glory.section.the_lost_and_forgotten.description"))
                .AddToggle("knights", L("glory.section.knights.title"), ui.Knights, L("glory.section.knights.description"))
                .AddToggle("mecha_knight", L("glory.section.mecha_knight.title"), ui.MechaKnight, L("glory.section.mecha_knight.description"))
                .AddToggle("soul_nexus", L("glory.section.soul_nexus.title"), ui.SoulNexus, L("glory.section.soul_nexus.description"))
                .AddToggle("test_subject", L("glory.section.test_subject.title"), ui.TestSubject, L("glory.section.test_subject.description"))
                .AddToggle("aeonglass", L("glory.section.aeonglass.title"), ui.Aeonglass, L("glory.section.aeonglass.description"))
                .AddToggle("doormaker", L("glory.section.doormaker.title"), ui.Doormaker, L("glory.section.doormaker.description"))
            ), "enemies"
        );
    }

    private static ModSettingsText L(string key, string fallback = "")
    {
        return ModSettingsText.LocString(new LocString("settings_ui", key), fallback);
    }
}