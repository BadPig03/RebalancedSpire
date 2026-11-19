namespace RebalancedSpire.Core.Configs;

internal sealed record RebalancedSpireSettingsSnapshot(int Protocol, string ModVersion, string SessionId, long Revision, RebalancedSpireSettings Settings);
