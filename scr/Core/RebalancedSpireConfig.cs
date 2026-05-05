using BaseLib.Config;

namespace RebalancedSpire.scr.Core;

using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
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

    [ConfigSection("Neow")]
    public static bool BoomingConchConfig { get; set; } = true;
    public static bool LargeCapsuleConfig { get; set; } = true;
    public static bool LavaRockConfig { get; set; } = true;
    public static bool NeowsTalismanConfig { get; set; } = true;
    public static bool NutritiousOysterConfig { get; set; } = true;
    public static bool PomanderConfig { get; set; } = true;
    public static bool NeowChoicesConfig { get; set; } = true;

    [ConfigSection("Orobas")]
    public static bool AlchemicalCofferConfig { get; set; } = true;
    public static bool OrobasChoicesConfig { get; set; } = true;

    [ConfigSection("Pael")]
    public static bool PaelsHornConfig { get; set; } = true;

    [ConfigSection("Tezcatara")]
    public static bool BiiigHugConfig { get; set; } = true;
    public static bool ToastyMittensConfig { get; set; } = true;

    [ConfigSection("Darv")]
    public static bool DustyTomeConfig { get; set; } = true;
    public static bool DarvChoicesConfig { get; set; } = true;

    [ConfigSection("Nonupeipe")]
    public static bool LoomingFruitConfig { get; set; } = true;

    [ConfigSection("Tanx")]
    public static bool CrossbowConfig { get; set; } = true;
    public static bool WarHammerConfig { get; set; } = true;

    [ConfigSection("Vakuu")]
    public static bool BloodSoakedRoseConfig { get; set; } = true;
    public static bool FiddleConfig { get; set; } = true;
    public static bool PreservedFogConfig { get; set; } = true;
    public static bool SereTalonConfig { get; set; } = true;
    public static bool LordsParasolConfig { get; set; } = true;
    public static bool WhisperingEarringConfig { get; set; } = true;
    public static bool VakuuChoicesConfig { get; set; } = true;
    public static bool ApparitionArtConfig { get; set; } = false;
    public static bool VakuuFixedArtConfig { get; set; } = true;

    [ConfigVisibleIf(nameof(VakuuFixedArtConfig), false)]
    public static bool VakuuBetaArtConfig { get; set; } = false;

    [ConfigSection("Merchant")]
    public static bool MerchantConfig { get; set; } = true;

    [ConfigSection("Event")]
    public static bool HungryForMushroomsConfig { get; set; } = true;
    public static bool LostWispConfig { get; set; } = true;
    public static bool MorphicGroveConfig { get; set; } = true;
    public static bool PunchOffConfig { get; set; } = true;
    public static bool ReflectionsConfig { get; set; } = true;
    public static bool SpiralingWhirlpoolConfig { get; set; } = true;
    public static bool SpiritGrafterConfig { get; set; } = true;
    public static bool SunkenStatueConfig { get; set; } = true;
    public static bool TeaMasterConfig { get; set; } = true;
    public static bool TheLanternKeyConfig { get; set; } = true;
    public static bool TrialConfig { get; set; } = true;
    public static bool WelcomeToWongosConfig { get; set; } = true;

    [ConfigSection("Silent")]
    public static bool AcrobaticsConfig { get; set; } = true;
    public static bool FollowThroughConfig { get; set; } = true;
    public static bool UntouchableConfig { get; set; } = true;
    public static bool WellLaidPlansConfig { get; set; } = true;

    [ConfigSection("Necrobinder")]
    public static bool BansheesCryConfig { get; set; } = true;
    public static bool DefyConfig { get; set; } = true;

    [ConfigSection("Defect")]
    public static bool CoolantConfig { get; set; } = true;
    public static bool DefragmentConfig { get; set; } = true;
    public static bool SynchronizeConfig { get; set; } = true;

    [ConfigSection("Overgrowth")]
    public static bool CubexConstructConfig { get; set; } = true;
    public static bool FogmogConfig { get; set; } = true;
    public static bool FlyconidConfig { get; set; } = true;
    public static bool FuzzyWurmCrawlerConfig { get; set; } = true;
    public static bool InkletConfig { get; set; } = true;
    public static bool LeafSlimeSConfig { get; set; } = true;
    public static bool NibbitConfig { get; set; } = true;
    public static bool ShrinkerBeetleConfig { get; set; } = true;
    public static bool SlitheringStranglerConfig { get; set; } = true;
    public static bool SnappingJaxfruitConfig { get; set; } = true;
    public static bool TwigSlimeMConfig { get; set; } = true;
    public static bool VineShamblerConfig { get; set; } = true;
    public static bool BygoneEffigyConfig { get; set; } = true;
    public static bool CeremonialBeastConfig { get; set; } = true;
    public static bool ByrdonisConfig { get; set; } = true;
    public static bool PhrogParasiteConfig { get; set; } = true;
    public static bool TheKinConfig { get; set; } = true;
    public static bool VantomConfig { get; set; } = true;

    [ConfigSection("Underdocks")]
    public static bool CalcifiedCultistConfig { get; set; } = true;
    public static bool DampCultistConfig { get; set; } = true;
    public static bool FossilStalkerConfig { get; set; } = true;
    public static bool LivingFogConfig { get; set; } = true;
    public static bool GremlinMercConfig { get; set; } = true;
    public static bool HauntedShipConfig { get; set; } = true;
    public static bool SeapunkConfig { get; set; } = true;
    public static bool SewerClamConfig { get; set; } = true;
    public static bool SludgeSpinnerConfig { get; set; } = true;
    public static bool ToadpoleConfig { get; set; } = true;
    public static bool TwoTailedRatConfig { get; set; } = true;
    public static bool PhantasmalGardenerConfig { get; set; } = true;
    public static bool SkulkingColonyConfig { get; set; } = true;
    public static bool TerrorEelConfig { get; set; } = true;
    public static bool SoulFyshConfig { get; set; } = true;
    public static bool WaterfallGiantConfig { get; set; } = true;

    [ConfigSection("Hive")]
    public static bool BowlbugsConfig { get; set; } = true;
    public static bool ChomperConfig { get; set; } = true;
    public static bool ExoskeletonConfig { get; set; } = true;
    public static bool HunterKillerConfig { get; set; } = true;
    public static bool MyteConfig { get; set; } = true;
    public static bool OvicopterConfig { get; set; } = true;
    public static bool TheObscuraConfig { get; set; } = true;
    public static bool ThievingHopperConfig { get; set; } = true;
    public static bool TunnelerConfig { get; set; } = true;
    public static bool DecimillipedeConfig { get; set; } = true;
    public static bool EntomancerConfig { get; set; } = true;
    public static bool KaiserCrabConfig { get; set; } = true;
    public static bool KnowledgeDemonConfig { get; set; } = true;
    public static bool TheInsatiableConfig { get; set; } = true;

    [ConfigSection("Glory")]
    public static bool FabricatorConfig { get; set; } = true;
    public static bool FrogKnightConfig { get; set; } = true;
    public static bool GlobeHeadConfig { get; set; } = true;
    public static bool TurretOperatorConfig { get; set; } = true;
    public static bool OwlMagistrateConfig { get; set; } = true;
    public static bool ScrollOfBitingConfig { get; set; } = true;
    public static bool SlimedBerserkerConfig { get; set; } = true;
    public static bool TheLostAndForgottenConfig { get; set; } = true;
    public static bool KnightsConfig { get; set; } = true;
    public static bool MechaKnightConfig { get; set; } = true;
    public static bool SoulNexusConfig { get; set; } = true;
    public static bool TestSubjectConfig { get; set; } = true;
    public static bool QueenConfig { get; set; } = true;
    public static bool DoormakerConfig { get; set; } = true;

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

        Save();
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