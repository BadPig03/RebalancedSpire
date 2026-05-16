namespace RebalancedSpire.Core.Harmony.Relics;

using BaseLib.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class AlchemicalCofferPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.AlchemicalCofferConfig;

    private static readonly SavedSpireField<AlchemicalCoffer, ModelId?> LastPotionUsed = new(() => null, "REBALANCEDSPIRE-ALCHEMICAL_COFFER");

    private static async Task AfterObtained(AlchemicalCoffer instance)
    {
        await PlayerCmd.GainMaxPotionCount(instance.DynamicVars["PotionSlots"].IntValue, instance.Owner);
    }

    private static Task AfterPotionUsed(AlchemicalCoffer instance, PotionModel potion)
    {
        if (potion.Owner != instance.Owner)
        {
            return Task.CompletedTask;
        }

        LastPotionUsed.Set(instance, potion.Id);
        return Task.CompletedTask;
    }

    private static async Task AfterCombatEnd(AlchemicalCoffer instance)
    {
        var potionId = LastPotionUsed.Get(instance);
        if (potionId == null)
        {
            return;
        }

        await PotionCmd.TryToProcure(SaveUtil.PotionOrDeprecated(potionId).ToMutable(), instance.Owner);
    }

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not AlchemicalCoffer)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-ALCHEMICAL_COFFER.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "EventDescription", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_EventDescription(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not AlchemicalCoffer)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-ALCHEMICAL_COFFER.eventDescription");
        return false;
    }

    [HarmonyPatch(typeof(AlchemicalCoffer), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(AlchemicalCoffer __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("PotionSlots", 1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(AlchemicalCoffer), "AfterObtained")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(AlchemicalCoffer __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterObtained(__instance);
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterPotionUsed")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterPotionUsed(AbstractModel __instance, PotionModel potion, Creature? target, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not AlchemicalCoffer alchemicalCoffer)
        {
            return true;
        }

        __result = AfterPotionUsed(alchemicalCoffer, potion);
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

        if (__instance is not AlchemicalCoffer alchemicalCoffer)
        {
            return true;
        }

        __result = AfterCombatEnd(alchemicalCoffer);
        return false;
    }
}