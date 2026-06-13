namespace RebalancedSpire.Core.Harmony.Cards.Quest;

using Afflictions;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ByrdonisEggPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Byrdonis;

    [HarmonyPatch(typeof(CardModel), "ShouldGlowGoldInternal", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShouldGlowGoldInternal(CardModel __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ByrdonisEgg { Affliction: ToItsOriginOwner })
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TargetType(CardModel __instance, ref TargetType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not ByrdonisEgg { Affliction: ToItsOriginOwner })
        {
            return true;
        }

        __result = TargetType.AnyEnemy;
        return false;
    }
}