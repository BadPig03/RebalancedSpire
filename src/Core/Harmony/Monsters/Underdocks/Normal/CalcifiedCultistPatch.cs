namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CalcifiedCultistPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.CalcifiedCultistConfig;

    [HarmonyPatch(typeof(CalcifiedCultist), "MaxInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(CalcifiedCultist __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = __instance.MinInitialHp;
    }
}