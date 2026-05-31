namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FishingRodPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.FishingRod;

    private static Task AfterCombatVictory(FishingRod instance, CombatRoom room)
    {
        if (room.Encounter.RoomType is not (RoomType.Monster or RoomType.Elite or RoomType.Boss))
        {
            return Task.CompletedTask;
        }

        instance.CombatsSeen++;
        if (instance.CombatsSeen % instance.DynamicVars["Combats"].IntValue != 0)
        {
            return Task.CompletedTask;
        }

        instance.Flash();
        var cardModel = instance.Owner.RunState.Rng.Niche.NextItem(PileType.Deck.GetPile(instance.Owner).Cards.Where(c => c.IsUpgradable));
        if (cardModel != null)
        {
            CardCmd.Upgrade(cardModel);
        }
        return Task.CompletedTask;
    }

    [HarmonyPatch(typeof(FishingRod), "AfterCombatEnd")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatEnd(FishingRod __instance, CombatRoom room, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterCombatVictory")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatVictory(AbstractModel __instance, CombatRoom room, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not FishingRod fishingRod)
        {
            return true;
        }

        __result = AfterCombatVictory(fishingRod, room);
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (__instance is not FishingRod)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_FISHING_ROD.description");
        return false;
    }
}