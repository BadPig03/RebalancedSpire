namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public class StabbotPatch
{
    [HarmonyPatch(typeof(Stabbot), nameof(Stabbot.MinInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        __result = 18;
    }

    [HarmonyPatch(typeof(Stabbot), nameof(Stabbot.MaxInitialHp), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMaxInitialHp(ref int __result)
    {
        __result = 18;
    }

    [HarmonyPatch(typeof(Stabbot), nameof(Stabbot.StabDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceStabDamage(ref int __result)
    {
        __result -= 2;
    }
}