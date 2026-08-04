using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace RebalancedSpire;

using System.Reflection;
using Core.Harmony;
using Core.Registry;
using STS2RitsuLib;
using STS2RitsuLib.Interop;

[ModInitializer("Initialize")]
public partial class RebalancedSpireMain : Node
{
    internal const string ModId = "RebalancedSpire";
    internal const string SettingsKey = "settings";
    internal const string SettingsFileName = "settings.json";
    public const string Version = "v0.3.8-beta";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    private static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();
        new Harmony(ModId).PatchAllForRebalancedSpire(assembly);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);

        RebalancedSpireCardTransformationRegistry.Initialize();
        RebalancedSpireNodesRegistry.Initialize();
        RebalancedSpireSettingsRegistry.Initialize();
    }
}