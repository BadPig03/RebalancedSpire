namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DevotedSculptorPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.DevotedSculptorConfig;

    [HarmonyPatch(typeof(DevotedSculptor), nameof(DevotedSculptor.SavageDamage), MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceSavageDamage(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 4;
    }
}