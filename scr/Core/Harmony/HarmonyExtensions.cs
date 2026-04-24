namespace RebalancedSpire.scr.Core.Harmony;

using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;

public static class HarmonyExtensions
{
    public static void PatchAllForRebalancedSpire(this Harmony harmony, Assembly assembly, string? category = null)
    {
        RebalancedSpireMain.Logger.Info($"Starting patching for assembly {assembly}");
        try
        {
            var patchProcessors = AccessTools.GetTypesFromAssembly(assembly).Where(type => type.HasHarmonyAttribute()).Select(harmony.CreateClassProcessor);
            var successCount = 0;
            var failCount = 0;
            patchProcessors.DoIf(processor => category?.Equals(processor.Category) ?? string.IsNullOrEmpty(processor.Category),
                delegate(PatchClassProcessor processor)
                {
                    try
                    {
                        processor.Patch();
                        ++successCount;
                    }
                    catch (Exception e)
                    {
                        RebalancedSpireMain.Logger.Error(e.ToString());
                        ++failCount;
                    }
                });

            RebalancedSpireMain.Logger.Info($"Applied {successCount} patches successfully, {failCount} failed");
        }
        catch (Exception e)
        {
            RebalancedSpireMain.Logger.Error($"Error occurred during patching for assembly {assembly}: {e}");
        }
    }
}