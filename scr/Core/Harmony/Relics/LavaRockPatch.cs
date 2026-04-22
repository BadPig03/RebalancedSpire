namespace RebalancedSpire.scr.Core.Harmony.Relics;

using System.Collections.ObjectModel;
using BaseLib.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryNeow)]
// ReSharper disable InconsistentNaming
public static class LavaRockPatch
{
    private static int EnemiesDefeatedRequired => 4;
    private static readonly SavedSpireField<LavaRock, int> EnemiesDefeated = new(() => 0, "REBALANCEDSPIRE-LAVA_ROCK");

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (__instance is not LavaRock)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-LAVA_ROCK.description");
        return false;
    }

    [HarmonyPatch(typeof(LavaRock), nameof(LavaRock.ShowCounter), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShowCounter(LavaRock __instance, ref bool __result)
    {
        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.DisplayAmount), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_DisplayAmount(RelicModel __instance, ref int __result)
    {
        if (__instance is not LavaRock lavaRock)
        {
            return true;
        }

        __result = EnemiesDefeated.Get(lavaRock);
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.AfterObtained))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(RelicModel __instance, ref Task __result)
    {
        if (__instance is not LavaRock lavaRock)
        {
            return true;
        }

        lavaRock.HasTriggered = true;
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCombatEnd))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatEnd(AbstractModel __instance, CombatRoom room, ref Task __result)
    {
        if (__instance is not LavaRock lavaRock)
        {
            return true;
        }

        var enemiesDefeated = EnemiesDefeated.Get(lavaRock) + 1;
        EnemiesDefeated.Set(lavaRock, enemiesDefeated);
        lavaRock.InvokeDisplayAmountChanged();
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(LavaRock), nameof(LavaRock.TryModifyRewards))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TryModifyRewards(LavaRock __instance, Player player, List<Reward> rewards, AbstractRoom? room, ref bool __result)
    {
        if (player != __instance.Owner || room is not CombatRoom || EnemiesDefeated.Get(__instance) < EnemiesDefeatedRequired)
        {
            return true;
        }

        __instance.Flash();
        rewards.Add(new CardReward(CardCreationOptions.ForNonCombatWithUniformOdds(new ReadOnlyCollection<CardPoolModel>([__instance.Owner.Character.CardPool]), c => c.Rarity == CardRarity.Uncommon).WithFlags(CardCreationFlags.NoRarityModification), 3, player));
        EnemiesDefeated.Set(__instance, 0);
        __instance.InvokeDisplayAmountChanged();
        __result = true;
        return false;
    }
}