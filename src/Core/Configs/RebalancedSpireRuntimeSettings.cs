namespace RebalancedSpire.Core.Configs;

internal static class RebalancedSpireRuntimeSettings
{
    private static RebalancedSpireSettings? _current;

    internal static RebalancedSpireSettings Current => _current ??= RebalancedSpireSettingsSession.Copy(RebalancedSpireSettingsStore.Settings);

    internal static void Install(RebalancedSpireSettings settings)
    {
        var previous = Current;
        if (RebalancedSpireSettingsSession.Equal(previous, settings))
        {
            return;
        }

        _current = RebalancedSpireSettingsSession.Copy(settings);
        RebalancedSpireSettingsModelCache.Refresh(previous, _current);
    }

    internal static void RestoreLocal()
    {
        Install(RebalancedSpireSettingsStore.Settings);
    }
}
