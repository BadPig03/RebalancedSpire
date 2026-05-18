namespace RebalancedSpire.Core.Configs;

using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using BaseLib.Config;
using Godot;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
[ConfigHoverTipsByDefault]
internal class RebalancedSpireConfig : SimpleModConfig
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private const string Map = "Map";
    private const string Neow = "Neow";
    private const string Orobas = "Orobas";
    private const string Pael = "Pael";
    private const string Tezcatara = "Tezcatara";
    private const string Darv = "Darv";
    private const string Nonupeipe = "Nonupeipe";
    private const string Tanx = "Tanx";
    private const string Vakuu = "Vakuu";
    private const string Merchant = "Merchant";
    private const string Event = "Event";
    private const string Ironclad = "Ironclad";
    private const string Silent = "Silent";
    private const string Regent = "Regent";
    private const string Necrobinder = "Necrobinder";
    private const string Defect = "Defect";
    private const string Colorless = "Colorless";
    private const string Overgrowth = "Overgrowth";
    private const string Underdocks = "Underdocks";
    private const string Hive = "Hive";
    private const string Glory = "Glory";

    [ConfigSection("Configurations")]
    [ConfigButton("LoadConfigFileButton")]
    [UsedImplicitly]
    public static void LoadConfigFile(ModConfig config)
    {
        if (Engine.GetMainLoop() is not SceneTree tree || config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        var dialog = new FileDialog
        {
            Title = new LocString("settings_ui", "REBALANCEDSPIRE-SAVE_CONFIG_FILE.title").GetFormattedText(),
            FileMode = FileDialog.FileModeEnum.OpenFile,
            Access = FileDialog.AccessEnum.Filesystem
        };
        dialog.AddFilter("*.json", "Json files (*.json)|*.json");
        dialog.FileSelected += path =>
        {
            rebalancedSpireConfig.LoadConfigFromJsonFile(path);
            dialog.QueueFree();
        };
        dialog.Canceled += dialog.QueueFree;
        tree.Root.AddChild(dialog);
        dialog.PopupCenteredRatio(0.55f);
    }

    [ConfigButton("SaveConfigFileButton")]
    [UsedImplicitly]
    public static void SaveConfigFile(ModConfig config)
    {
        if (Engine.GetMainLoop() is not SceneTree tree)
        {
            return;
        }

        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        var dialog = new FileDialog
        {
            Title = new LocString("settings_ui", "REBALANCEDSPIRE-SAVE_CONFIG_FILE.title").GetFormattedText(),
            FileMode = FileDialog.FileModeEnum.SaveFile,
            Access = FileDialog.AccessEnum.Filesystem,
            CurrentFile = "rebalanced_spire_settings.json"
        };
        dialog.AddFilter("*.json", "Json files (*.json)|*.json");
        dialog.FileSelected += path =>
        {
            rebalancedSpireConfig.SaveConfigToJsonFile(path);
            dialog.QueueFree();
        };
        dialog.Canceled += dialog.QueueFree;
        tree.Root.AddChild(dialog);
        dialog.PopupCenteredRatio(0.55f);
    }

    [ConfigSection(Map)]
    [ConfigButton("ToggleMapConfigButton")]
    [UsedImplicitly]
    public static void ToggleMapConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Map);
    }
    [ConfigGroup(Map)]
    public static bool MapGenerationConfig { get; set; } = true;

    [ConfigSection(Neow)]
    [ConfigButton("ToggleNeowConfigButton")]
    [UsedImplicitly]
    public static void ToggleNeowConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Neow);
    }
    [ConfigGroup(Neow)]
    public static bool BoomingConchConfig { get; set; } = true;
    [ConfigGroup(Neow)]
    public static bool LargeCapsuleConfig { get; set; } = true;
    [ConfigGroup(Neow)]
    public static bool LavaRockConfig { get; set; } = true;
    [ConfigGroup(Neow)]
    public static bool NeowsTalismanConfig { get; set; } = true;
    [ConfigGroup(Neow)]
    public static bool NutritiousOysterConfig { get; set; } = true;
    [ConfigGroup(Neow)]
    public static bool PomanderConfig { get; set; } = true;
    [ConfigGroup(Neow)]
    public static bool NeowChoicesConfig { get; set; } = true;

    [ConfigSection(Orobas)]
    [ConfigButton("ToggleOrobasConfigButton")]
    [UsedImplicitly]
    public static void ToggleOrobasConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Orobas);
    }
    [ConfigGroup(Orobas)]
    public static bool AlchemicalCofferConfig { get; set; } = true;
    [ConfigGroup(Orobas)]
    public static bool OrobasChoicesConfig { get; set; } = true;

    [ConfigSection(Pael)]
    [ConfigButton("TogglePaelConfigButton")]
    [UsedImplicitly]
    public static void TogglePaelConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Pael);
    }
    [ConfigGroup(Pael)]
    public static bool PaelsHornConfig { get; set; } = true;

    [ConfigSection(Tezcatara)]
    [ConfigButton("ToggleTezcataraConfigButton")]
    [UsedImplicitly]
    public static void ToggleTezcataraConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Tezcatara);
    }
    [ConfigGroup(Tezcatara)]
    public static bool BiiigHugConfig { get; set; } = true;
    [ConfigGroup(Tezcatara)]
    public static bool SealOfGoldConfig { get; set; } = true;
    [ConfigGroup(Tezcatara)]
    public static bool ToastyMittensConfig { get; set; } = true;

    [ConfigSection(Darv)]
    [ConfigButton("ToggleDarvConfigButton")]
    [UsedImplicitly]
    public static void ToggleDarvConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Darv);
    }
    [ConfigGroup(Darv)]
    public static bool DustyTomeConfig { get; set; } = true;
    [ConfigGroup(Darv)]
    public static bool DarvChoicesConfig { get; set; } = true;

    [ConfigSection(Nonupeipe)]
    [ConfigButton("ToggleNonupeipeConfigButton")]
    [UsedImplicitly]
    public static void ToggleNonupeipeConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Nonupeipe);
    }
    [ConfigGroup(Nonupeipe)]
    public static bool LoomingFruitConfig { get; set; } = true;

    [ConfigSection(Tanx)]
    [ConfigButton("ToggleTanxConfigButton")]
    [UsedImplicitly]
    public static void ToggleTanxConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Tanx);
    }
    [ConfigGroup(Tanx)]
    public static bool CrossbowConfig { get; set; } = true;
    [ConfigGroup(Tanx)]
    public static bool WarHammerConfig { get; set; } = true;

    [ConfigSection(Vakuu)]
    [ConfigButton("ToggleVakuuConfigButton")]
    [UsedImplicitly]
    public static void ToggleVakuuConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Vakuu);
    }
    [ConfigGroup(Vakuu)]
    public static bool BloodSoakedRoseConfig { get; set; } = true;
    [ConfigGroup(Vakuu)]
    public static bool FiddleConfig { get; set; } = true;
    [ConfigGroup(Vakuu)]
    public static bool PreservedFogConfig { get; set; } = true;
    [ConfigGroup(Vakuu)]
    public static bool SereTalonConfig { get; set; } = true;
    [ConfigGroup(Vakuu)]
    public static bool LordsParasolConfig { get; set; } = true;
    [ConfigGroup(Vakuu)]
    public static bool WhisperingEarringConfig { get; set; } = true;
    [ConfigGroup(Vakuu)]
    public static bool VakuuChoicesConfig { get; set; } = true;
    public static bool VakuuFixedArtConfig { get; set; } = true;

    [ConfigSection(Merchant)]
    [ConfigButton("ToggleMerchantConfigButton")]
    [UsedImplicitly]
    public static void ToggleMerchantConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Merchant);
    }
    [ConfigGroup(Merchant)]
    public static bool MerchantConfig { get; set; } = true;

    [ConfigSection(Event)]
    [ConfigButton("ToggleEventConfigButton")]
    [UsedImplicitly]
    public static void ToggleEventConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Event);
    }
    [ConfigGroup(Event)]
    public static bool HungryForMushroomsConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool LostWispConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool MorphicGroveConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool PunchOffConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool ReflectionsConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool SpiralingWhirlpoolConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool SpiritGrafterConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool SunkenStatueConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool TeaMasterConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool TheLanternKeyConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool TrialConfig { get; set; } = true;
    [ConfigGroup(Event)]
    public static bool WelcomeToWongosConfig { get; set; } = true;

    [ConfigSection(Ironclad)]
    [ConfigButton("ToggleIroncladConfigButton")]
    [UsedImplicitly]
    public static void ToggleIroncladConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Ironclad);
    }
    [ConfigGroup(Ironclad)]
    public static bool BloodWallConfig { get; set; } = true;
    [ConfigGroup(Ironclad)]
    public static bool DrumOfBattleConfig { get; set; } = true;
    [ConfigGroup(Ironclad)]
    public static bool ExpectAFightConfig { get; set; } = true;
    [ConfigGroup(Ironclad)]
    public static bool ForgottenRitualConfig { get; set; } = true;

    [ConfigSection(Silent)]
    [ConfigButton("ToggleSilentConfigButton")]
    [UsedImplicitly]
    public static void ToggleSilentConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Silent);
    }
    [ConfigGroup(Silent)]
    public static bool AcrobaticsConfig { get; set; } = true;
    [ConfigGroup(Silent)]
    public static bool FlickFlackConfig { get; set; } = true;
    [ConfigGroup(Silent)]
    public static bool FollowThroughConfig { get; set; } = true;
    [ConfigGroup(Silent)]
    public static bool UntouchableConfig { get; set; } = true;
    [ConfigGroup(Silent)]
    public static bool WellLaidPlansConfig { get; set; } = true;

    [ConfigSection(Regent)]
    [ConfigButton("ToggleRegentConfigButton")]
    [UsedImplicitly]
    public static void ToggleRegentConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Regent);
    }
    [ConfigGroup(Regent)]
    public static bool GlowConfig { get; set; } = true;

    [ConfigSection(Necrobinder)]
    [ConfigButton("ToggleNecrobinderConfigButton")]
    [UsedImplicitly]
    public static void ToggleNecrobinderConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Necrobinder);
    }
    [ConfigGroup(Necrobinder)]
    public static bool AfterlifeConfig { get; set; } = true;
    [ConfigGroup(Necrobinder)]
    public static bool BansheesCryConfig { get; set; } = true;
    [ConfigGroup(Necrobinder)]
    public static bool DefyConfig { get; set; } = true;
    [ConfigGroup(Necrobinder)]
    public static bool PullAggroConfig { get; set; } = true;
    [ConfigGroup(Necrobinder)]
    public static bool RightHandHandConfig { get; set; } = true;
    [ConfigGroup(Necrobinder)]
    public static bool SicEmConfig { get; set; } = true;
    [ConfigGroup(Necrobinder)]
    public static bool SpurConfig { get; set; } = true;

    [ConfigSection(Defect)]
    [ConfigButton("ToggleDefectConfigButton")]
    [UsedImplicitly]
    public static void ToggleDefectConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Defect);
    }
    [ConfigGroup(Defect)]
    public static bool ConsumingShadowConfig { get; set; } = true;
    [ConfigGroup(Defect)]
    public static bool CoolantConfig { get; set; } = true;
    [ConfigGroup(Defect)]
    public static bool DefragmentConfig { get; set; } = true;
    [ConfigGroup(Defect)]
    public static bool GlassworkConfig { get; set; } = true;
    [ConfigGroup(Defect)]
    public static bool LeapConfig { get; set; } = true;
    [ConfigGroup(Defect)]
    public static bool RefractConfig { get; set; } = true;
    [ConfigGroup(Defect)]
    public static bool SpinnerConfig { get; set; } = true;
    [ConfigGroup(Defect)]
    public static bool SynchronizeConfig { get; set; } = true;
    [ConfigGroup(Defect)]
    public static bool VoltaicConfig { get; set; } = true;

    [ConfigSection(Colorless)]
    [ConfigButton("ToggleColorlessConfigButton")]
    [UsedImplicitly]
    public static void ToggleColorlessConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Colorless);
    }
    [ConfigGroup(Colorless)]
    public static bool BolasConfig { get; set; } = true;
    [ConfigGroup(Colorless)]
    public static bool EternalArmorConfig { get; set; } = true;
    [ConfigGroup(Colorless)]
    public static bool RollingBoulderConfig { get; set; } = true;

    [ConfigSection(Overgrowth)]
    [ConfigButton("ToggleOvergrowthConfigButton")]
    [UsedImplicitly]
    public static void ToggleOvergrowthConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Overgrowth);
    }
    [ConfigGroup(Overgrowth)]
    public static bool CubexConstructConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool FogmogConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool FlyconidConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool FuzzyWurmCrawlerConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool InkletConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool LeafSlimeSConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool NibbitConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool ShrinkerBeetleConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool SlitheringStranglerConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool SnappingJaxfruitConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool TwigSlimeMConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool VineShamblerConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool BygoneEffigyConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool CeremonialBeastConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool ByrdonisConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool PhrogParasiteConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool TheKinConfig { get; set; } = true;
    [ConfigGroup(Overgrowth)]
    public static bool VantomConfig { get; set; } = true;

    [ConfigSection(Underdocks)]
    [ConfigButton("ToggleUnderdocksConfigButton")]
    [UsedImplicitly]
    public static void ToggleUnderdocksConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Underdocks);
    }
    [ConfigGroup(Underdocks)]
    public static bool CorpseSlugConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool CalcifiedCultistConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool DampCultistConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool FossilStalkerConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool LivingFogConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool GremlinMercConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool HauntedShipConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool SeapunkConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool SewerClamConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool SludgeSpinnerConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool ToadpoleConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool TwoTailedRatConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool PhantasmalGardenerConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool SkulkingColonyConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool TerrorEelConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool SoulFyshConfig { get; set; } = true;
    [ConfigGroup(Underdocks)]
    public static bool WaterfallGiantConfig { get; set; } = true;

    [ConfigSection(Hive)]
    [ConfigButton("ToggleHiveConfigButton")]
    [UsedImplicitly]
    public static void ToggleHiveConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Hive);
    }
    [ConfigGroup(Hive)]
    public static bool BowlbugsConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool ChomperConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool ExoskeletonConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool HunterKillerConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool MyteConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool OvicopterConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool TheObscuraConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool ThievingHopperConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool TunnelerConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool DecimillipedeConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool EntomancerConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool InfestedPrismConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool KaiserCrabConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool KnowledgeDemonConfig { get; set; } = true;
    [ConfigGroup(Hive)]
    public static bool TheInsatiableConfig { get; set; } = true;

    [ConfigSection(Glory)]
    [ConfigButton("ToggleGloryConfigButton")]
    [UsedImplicitly]
    public static void ToggleGloryConfig(ModConfig config)
    {
        if (config is not RebalancedSpireConfig rebalancedSpireConfig)
        {
            return;
        }

        rebalancedSpireConfig.InvertAll(Glory);
    }
    [ConfigGroup(Glory)]
    public static bool FabricatorConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool FrogKnightConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool GlobeHeadConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool TurretOperatorConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool OwlMagistrateConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool ScrollOfBitingConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool SlimedBerserkerConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool TheLostAndForgottenConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool KnightsConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool MechaKnightConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool SoulNexusConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool TestSubjectConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool QueenConfig { get; set; } = true;
    [ConfigGroup(Glory)]
    public static bool DoormakerConfig { get; set; } = true;

    private void InvertAll(string name)
    {
        var properties = ConfigProperties.Where(p => p.GetCustomAttribute<ConfigGroupAttribute>()?.Name == name).ToList();
        foreach (var propertyInfo in properties)
        {
            propertyInfo.SetValue(null, !(bool?) propertyInfo.GetValue(null));
        }
        SaveDebounced<RebalancedSpireConfig>();
    }

    private void LoadConfigFromJsonFile(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var failed = false;
        try
        {
            using FileStream utf8Json = File.OpenRead(path);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(utf8Json);
            if (dictionary == null)
            {
                failed = true;
            }
            else
            {
                foreach (PropertyInfo configProperty in ConfigProperties)
                {
                    if (!dictionary.TryGetValue(configProperty.Name, out var str))
                    {
                        failed = true;
                        continue;
                    }

                    try
                    {
                        var converter = TypeDescriptor.GetConverter(configProperty.PropertyType).ConvertFromInvariantString(str);
                        if (converter == null)
                        {
                            continue;
                        }

                        var value = configProperty.GetValue(null);
                        if (converter.Equals(value))
                        {
                            continue;
                        }

                        configProperty.SetValue(null, converter);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
        catch (Exception)
        {
            return;
        }
        if (failed)
        {
            return;
        }

        SaveDebounced();
        ConfigReloaded();
    }

    private void SaveConfigToJsonFile(string path)
    {
        var dictionary = new Dictionary<string, string>();
        try
        {
            foreach (PropertyInfo configProperty in ConfigProperties)
            {
                var invariantString = TypeDescriptor.GetConverter(configProperty.PropertyType).ConvertToInvariantString(configProperty.GetValue(null));
                if (invariantString == null)
                {
                    continue;
                }

                dictionary.Add(configProperty.Name, invariantString);
            }
        }
        catch (Exception)
        {
            return;
        }

        try
        {
            new FileInfo(path).Directory?.Create();
            using FileStream utf8Json = File.Create(path);
            JsonSerializer.Serialize(utf8Json, dictionary, JsonOptions);
        }
        catch
        {
            // ignored
        }
    }

    public static string GetConfigJson()
    {
        var config = ModConfigRegistry.Get<RebalancedSpireConfig>();
        if (config == null)
        {
            return "";
        }

        var dictionary = new Dictionary<string, string>();
        try
        {
            foreach (PropertyInfo configProperty in config.ConfigProperties)
            {
                var invariantString = TypeDescriptor.GetConverter(configProperty.PropertyType).ConvertToInvariantString(configProperty.GetValue(null));
                if (invariantString == null)
                {
                    continue;
                }

                dictionary.Add(configProperty.Name, invariantString);
            }
        }
        catch (Exception)
        {
            return "";
        }

        dictionary.Add("Version", RebalancedSpireMain.Version);
        return "\n" + JsonSerializer.Serialize(dictionary, JsonOptions);
    }
}