using BaseLib.Config;

namespace RebalancedSpire.scr.Core;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
[ConfigHoverTipsByDefault]
internal class RebalancedSpireConfig : SimpleModConfig
{
    [ConfigSection("Ancients")]
    public static bool NeowConfig { get; set; } = true;
    public static bool OrobasConfig { get; set; } = true;
    public static bool TezcataraConfig { get; set; } = true;
    public static bool PaelConfig { get; set; } = true;
    public static bool VakuuConfig { get; set; } = true;

    [ConfigSection("Merchant")]
    public static bool MerchantConfig { get; set; } = true;

    [ConfigSection("Silent")]
    public static bool AcrobaticsConfig { get; set; } = true;

    [ConfigSection("Necrobinder")]
    public static bool DefyConfig { get; set; } = true;

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
    public static bool LouseProgenitorConfig { get; set; } = true;
    public static bool MyteConfig { get; set; } = true;
    public static bool OvicopterConfig { get; set; } = true;
    public static bool TheObscuraConfig { get; set; } = true;
    public static bool SlumberingBeetleConfig { get; set; } = true;
    public static bool SpinyToadConfig { get; set; } = true;
    public static bool ThievingHopperConfig { get; set; } = true;
    public static bool TunnelerConfig { get; set; } = true;
    public static bool DecimillipedeConfig { get; set; } = true;
    public static bool EntomancerConfig { get; set; } = true;
    public static bool InfestedPrismConfig { get; set; } = true;
    public static bool KaiserCrabConfig { get; set; } = true;
    public static bool KnowledgeDemonConfig { get; set; } = true;
    public static bool TheInsatiableConfig { get; set; } = true;

    [ConfigSection("Glory")]
    public static bool AxebotConfig { get; set; } = true;
    public static bool DevotedSculptorConfig { get; set; } = true;
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
}