using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using RebalancedSpire.scr.Core;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace RebalancedSpire;

[ModInitializer(nameof(Initialize))]
public partial class RebalancedSpireMain : Node
{
    private const string ModId = "RebalancedSpire";

    public const string CategoryNeow = "Neow";
    public const string CategoryOrobas = "Orobas";
    public const string CategoryTezcatara = "Tezcatara";
    public const string CategoryPael = "Pael";
    public const string CategoryVakuu = "Vakuu";
    public const string CategorySilent = "Silent";
    public const string CategoryMerchant = "Merchant";
    public const string CategoryOvergrowth = "Overgrowth";
    public const string CategoryUnderdocks = "Underdocks";
    public const string CategoryHive = "Hive";
    public const string CategoryGlory = "Glory";

    // ReSharper disable once UnusedMember.Global
    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAllUncategorized();

        ModConfigRegistry.Register(ModId, new RebalancedSpireConfig());
        if (RebalancedSpireConfig.NeowConfig)
        {
            harmony.PatchCategory(CategoryNeow);
        }
        if (RebalancedSpireConfig.OrobasConfig)
        {
            harmony.PatchCategory(CategoryOrobas);
        }
        if (RebalancedSpireConfig.TezcataraConfig)
        {
            harmony.PatchCategory(CategoryTezcatara);
        }
        if (RebalancedSpireConfig.PaelConfig)
        {
            harmony.PatchCategory(CategoryPael);
        }
        if (RebalancedSpireConfig.VakuuConfig)
        {
            harmony.PatchCategory(CategoryVakuu);
        }

        if (RebalancedSpireConfig.SilentConfig)
        {
            harmony.PatchCategory(CategorySilent);
        }

        if (RebalancedSpireConfig.MerchantConfig)
        {
            harmony.PatchCategory(CategoryMerchant);
        }

        if (RebalancedSpireConfig.OvergrowthConfig)
        {
            harmony.PatchCategory(CategoryOvergrowth);
        }
        if (RebalancedSpireConfig.UnderdocksConfig)
        {
            harmony.PatchCategory(CategoryUnderdocks);
        }
        if (RebalancedSpireConfig.HiveConfig)
        {
            harmony.PatchCategory(CategoryHive);
        }
        if (RebalancedSpireConfig.GloryConfig)
        {
            harmony.PatchCategory(CategoryGlory);
        }
    }
}
