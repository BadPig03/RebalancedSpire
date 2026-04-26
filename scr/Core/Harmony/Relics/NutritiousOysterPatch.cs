namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NutritiousOysterPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.NutritiousOysterConfig;

    private static async Task AfterCombatVictory(NutritiousOyster nutritiousOyster)
    {
        await CreatureCmd.Heal(nutritiousOyster.Owner.Creature, nutritiousOyster.DynamicVars.Heal.BaseValue);
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not NutritiousOyster)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-NUTRITIOUS_OYSTER.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.EventDescription), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not NutritiousOyster)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-NUTRITIOUS_OYSTER.eventDescription");
        return false;
    }

    [HarmonyPatch(typeof(NutritiousOyster), nameof(NutritiousOyster.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(NutritiousOyster __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new MaxHpVar(11),
            new HealVar(1)
        };
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCombatVictory))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatVictory(AbstractModel __instance, CombatRoom room, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not NutritiousOyster nutritiousOyster || nutritiousOyster.Owner.Creature.IsDead)
        {
            return true;
        }

        nutritiousOyster.Flash();
        __result = AfterCombatVictory(nutritiousOyster);
        return false;
    }
}