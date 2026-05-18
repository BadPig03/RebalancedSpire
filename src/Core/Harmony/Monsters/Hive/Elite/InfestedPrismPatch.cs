namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Elite;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class InfestedPrismPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.InfestedPrismConfig;

    [HarmonyPatch(typeof(InfestedPrism), "MinInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IncreaseMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 10;
    }
}