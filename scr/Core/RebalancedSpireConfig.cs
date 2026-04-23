using BaseLib.Config;

namespace RebalancedSpire.scr.Core;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
internal class RebalancedSpireConfig : SimpleModConfig
{
    [ConfigSection("Ancients")]
    [ConfigHoverTip]
    public static bool NeowConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool OrobasConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TezcataraConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool PaelConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool VakuuConfig { get; set; } = true;

    [ConfigSection("Merchant")]
    [ConfigHoverTip]
    public static bool MerchantConfig { get; set; } = true;

    [ConfigSection("Silent")]
    [ConfigHoverTip]
    public static bool AcrobaticsConfig { get; set; } = true;

    [ConfigSection("Necrobinder")]
    [ConfigHoverTip]
    public static bool DefyConfig { get; set; } = true;

    [ConfigSection("Overgrowth")]
    [ConfigHoverTip]
    public static bool CubexConstructConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool FogmogConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool FlyconidConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool FuzzyWurmCrawlerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool InkletConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool LeafSlimeSConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool NibbitConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool ShrinkerBeetleConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SlitheringStranglerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SnappingJaxfruitConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TwigSlimeMConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool VineShamblerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool BygoneEffigyConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool CeremonialBeastConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool ByrdonisConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool PhrogParasiteConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TheKinConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool VantomConfig { get; set; } = true;

    [ConfigSection("Underdocks")]
    [ConfigHoverTip]
    public static bool CalcifiedCultistConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool DampCultistConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool FossilStalkerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool LivingFogConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool GremlinMercConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool HauntedShipConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SeapunkConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SewerClamConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SludgeSpinnerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool ToadpoleConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TwoTailedRatConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool PhantasmalGardenerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SkulkingColonyConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TerrorEelConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SoulFyshConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool WaterfallGiantConfig { get; set; } = true;

    [ConfigSection("Hive")]
    [ConfigHoverTip]
    public static bool BowlbugsConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool ChomperConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool ExoskeletonConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool HunterKillerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool LouseProgenitorConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool MyteConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool OvicopterConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TheObscuraConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SlumberingBeetleConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SpinyToadConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool ThievingHopperConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TunnelerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool DecimillipedeConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool EntomancerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool InfestedPrismConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool KaiserCrabConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool KnowledgeDemonConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TheInsatiableConfig { get; set; } = true;

    [ConfigSection("Glory")]
    [ConfigHoverTip]
    public static bool AxebotConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool DevotedSculptorConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool FabricatorConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool FrogKnightConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool GlobeHeadConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TurretOperatorConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool OwlMagistrateConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool ScrollOfBitingConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SlimedBerserkerConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TheLostAndForgottenConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool KnightsConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool MechaKnightConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool SoulNexusConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool TestSubjectConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool QueenConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool DoormakerConfig { get; set; } = true;
}