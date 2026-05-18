namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class GuardbotPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FabricatorConfig;

    [HarmonyPatch(typeof(Guardbot), "MinInitialHp", MethodType.Getter)]
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

    [HarmonyPatch(typeof(Guardbot), "MaxInitialHp", MethodType.Getter)]
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
}