namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SealOfGoldPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SealOfGoldConfig;

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SealOfGold)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-SEAL_OF_GOLD.description");
        return false;
    }

    [HarmonyPatch(typeof(SealOfGold), "AfterSideTurnStart")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterSideTurnStart(SealOfGold __instance, CombatSide side, ICombatState combatState, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (side != CombatSide.Player)
        {
            return true;
        }

        __instance.Status = RelicStatus.Active;
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterTurnEnd")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterTurnEnd(AbstractModel __instance, PlayerChoiceContext choiceContext, CombatSide side, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SealOfGold sealOfGold || side != CombatSide.Player)
        {
            return true;
        }

        sealOfGold.Status = RelicStatus.Normal;
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterCombatEnd")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatEnd(AbstractModel __instance, CombatRoom room, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SealOfGold sealOfGold)
        {
            return true;
        }

        sealOfGold.Status = RelicStatus.Normal;
        __result = Task.CompletedTask;
        return false;
    }
}