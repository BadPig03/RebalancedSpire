namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FrogKnightPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FrogKnightConfig;

    [HarmonyPatch(typeof(FrogKnight), nameof(FrogKnight.PlatingAmount), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReducePlatingAmount(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 12, 10);
    }

    [HarmonyPatch(typeof(FrogKnight), nameof(FrogKnight.TongueLashDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceTongueLashDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }

    [HarmonyPatch(typeof(FrogKnight), nameof(FrogKnight.StrikeDownEvilDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceStrikeDownEvilDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }
}