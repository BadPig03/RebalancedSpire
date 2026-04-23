namespace RebalancedSpire.scr.Core.Harmony.Monsters.Underdocks.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TwoTailedRatPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TwoTailedRatConfig;

    [HarmonyPatch(typeof(TwoTailedRat), nameof(TwoTailedRat.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(TwoTailedRat __instance, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = __instance.MinInitialHp;
    }
}