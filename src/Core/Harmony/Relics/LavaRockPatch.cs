namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Utils;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LavaRockPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.LavaRock;
    private static readonly SavedAttachedState<LavaRock, int> EnemiesDefeated = new("REBALANCED_SPIRE_RELIC_LAVA_ROCK", () => 0);
    private static readonly SavedAttachedState<LavaRock, int> TriggeredAmount = new("REBALANCED_SPIRE_RELIC_LAVA_ROCK_TRIGGERED", () => 0);

    [HarmonyPatch(typeof(AbstractModel), "AfterCombatVictory")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatVictory(AbstractModel __instance, CombatRoom room, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LavaRock lavaRock)
        {
            return true;
        }

        var enemiesDefeated = EnemiesDefeated[lavaRock] + 1;
        EnemiesDefeated.Set(lavaRock, enemiesDefeated);
        lavaRock.Flash();
        lavaRock.InvokeDisplayAmountChanged();
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "AfterObtained")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterObtained(RelicModel __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LavaRock lavaRock)
        {
            return true;
        }

        lavaRock.HasTriggered = true;
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(LavaRock), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(LavaRock __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("Enemies", 4)
        }.AsReadOnly();
        return false;
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

        if (__instance is not LavaRock)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_LAVA_ROCK.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "DisplayAmount", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_DisplayAmount(RelicModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LavaRock lavaRock)
        {
            return true;
        }

        __result = EnemiesDefeated[lavaRock];
        return false;
    }

    [HarmonyPatch(typeof(LavaRock), "ShowCounter", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShowCounter(LavaRock __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(LavaRock), "TryModifyRewards")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TryModifyRewards(LavaRock __instance, Player player, List<Reward> rewards, AbstractRoom? room, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (player != __instance.Owner || room is not CombatRoom)
        {
            return true;
        }

        var amount = TriggeredAmount[__instance];
        if (EnemiesDefeated[__instance] < __instance.DynamicVars["Enemies"].IntValue + amount)
        {
            return true;
        }

        if (player.RunState.CurrentActIndex != 2 || room.RoomType != RoomType.Boss)
        {
            rewards.Add(new CardReward(CardCreationOptions.ForNonCombatWithUniformOdds(new List<CardPoolModel>([__instance.Owner.Character.CardPool]), c => c.Rarity == CardRarity.Uncommon).WithFlags(CardCreationFlags.NoRarityModification), 3, player));
        }
        EnemiesDefeated.Set(__instance, 0);
        TriggeredAmount.Set(__instance, amount + 1);
        __instance.DynamicVars["Enemies"].BaseValue += 1;
        __instance.Flash();
        __instance.InvokeDisplayAmountChanged();
        __result = true;
        return false;
    }
}