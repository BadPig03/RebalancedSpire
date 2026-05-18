namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class CorpseSlugPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.CorpseSlugConfig;

    [HarmonyPatch(typeof(CorpseSlug), "MinInitialHp", MethodType.Getter)]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReduceMinInitialHp(ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        __result -= 6;
    }
}