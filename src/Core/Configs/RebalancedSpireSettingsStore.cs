namespace RebalancedSpire.Core.Configs;

using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Utils.Persistence;
using STS2RitsuLib.Utils.Persistence.Migration;

internal static class RebalancedSpireSettingsStore
{
    private static ModDataStore Store => ModDataStore.For(RebalancedSpireMain.ModId);

    internal static RebalancedSpireSettings Settings => Store.CreateCache<RebalancedSpireSettings>(RebalancedSpireMain.SettingsKey).Value;

    internal static void Initialize()
    {
        using (RitsuLibFramework.BeginModDataRegistration(RebalancedSpireMain.ModId, false))
        {
            Store.Register(RebalancedSpireMain.SettingsKey, RebalancedSpireMain.SettingsFileName, SaveScope.Global, () => new RebalancedSpireSettings(), true, new ModDataMigrationConfig
                {
                    CurrentDataVersion = RebalancedSpireSettings.CurrentSchemaVersion,
                    MinimumSupportedDataVersion = 1
                }, []
            );
        }
    }
}