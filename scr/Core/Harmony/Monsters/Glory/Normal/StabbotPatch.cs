namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class StabbotPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FabricatorConfig;

    [HarmonyPatch(typeof(Stabbot), nameof(Stabbot.MinInitialHp), MethodType.Getter)]
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

    [HarmonyPatch(typeof(Stabbot), nameof(Stabbot.MaxInitialHp), MethodType.Getter)]
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

    [HarmonyPatch(typeof(Stabbot), nameof(Stabbot.StabDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceStabDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 2;
    }
}