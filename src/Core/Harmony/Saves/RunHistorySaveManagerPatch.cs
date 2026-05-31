namespace RebalancedSpire.Core.Harmony.Saves;

using System.Text.Json;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RunHistorySaveManagerPatch
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    [HarmonyPatch(typeof(RunHistorySaveManager), "SaveHistory")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_SaveHistory(RunHistorySaveManager __instance, RunHistory history)
    {
        history.SchemaVersion = __instance._migrationManager.GetLatestVersion<RunHistory>();
        var content = JsonSerializationUtility.ToJson(history) + JsonSerializer.Serialize(new RebalancedSpireSettings(), Options);
        var path = Path.Combine(Path.GetDirectoryName(__instance.HistoryPath)!, "rebalanced_spire_history", $"{history.StartTime}_rebalancedspire.run");
        __instance._saveStore.WriteFile(path, content);
        RebalancedSpireMain.Logger.Info($"Saved modded run history: {history.StartTime}_rebalancedspire.run");
    }
}