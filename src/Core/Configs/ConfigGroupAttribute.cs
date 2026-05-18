namespace RebalancedSpire.Core.Configs;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigGroupAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}