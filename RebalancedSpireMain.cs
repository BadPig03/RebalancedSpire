using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using RebalancedSpire.scr.Core;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace RebalancedSpire;

using System.Reflection;
using Godot.Bridge;
using scr.Core.Harmony;

[ModInitializer(nameof(Initialize))]
public partial class RebalancedSpireMain : Node
{
    private const string ModId = "RebalancedSpire";
    public const string Version = "v0.0.7";

    // ReSharper disable once UnusedMember.Global
    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    private static Harmony? _mainHarmony;

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new RebalancedSpireConfig());
        _mainHarmony ??= new Harmony(ModId);
        _mainHarmony.PatchAllForRebalancedSpire(Assembly.GetExecutingAssembly());
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(RebalancedSpireMain).Assembly);
    }
}