using BaseLib.Config;

namespace RebalancedSpire.scr.Core;

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

    [ConfigSection("Characters")]
    [ConfigHoverTip]
    public static bool SilentConfig { get; set; } = true;

    [ConfigSection("Chapters")]
    [ConfigHoverTip]
    public static bool OvergrowthConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool UnderdocksConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool HiveConfig { get; set; } = true;

    [ConfigHoverTip]
    public static bool GloryConfig { get; set; } = true;
}