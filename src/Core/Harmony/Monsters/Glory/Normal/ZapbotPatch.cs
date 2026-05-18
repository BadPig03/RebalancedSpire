namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ZapbotPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FabricatorConfig;

    [HarmonyPatch(typeof(Zapbot), "MinInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = 36;
    }

    [HarmonyPatch(typeof(Zapbot), "MaxInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = 36;
    }

    [HarmonyPatch(typeof(Zapbot), "ZapDamage", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceZapDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }
}