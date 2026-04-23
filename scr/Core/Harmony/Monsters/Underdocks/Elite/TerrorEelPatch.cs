namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Elite;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TerrorEelPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TerrorEelConfig;

    [HarmonyPatch(typeof(TerrorEel), nameof(TerrorEel.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = 140;
    }
}