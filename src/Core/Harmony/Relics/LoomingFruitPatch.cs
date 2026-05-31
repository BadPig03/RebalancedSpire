namespace RebalancedSpire.Core.Harmony.Relics;

using System.Reflection;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LoomingFruitPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.LoomingFruit;
    private static readonly List<RelicModel> _fruitRelics =
    [
        ModelDb.Relic<Strawberry>(),
        ModelDb.Relic<Pear>(),
        ModelDb.Relic<Mango>()
    ];

    private static async Task AfterRewardTaken(LoomingFruit instance)
    {
        var relic = _fruitRelics.UnstableShuffle(instance.Owner.PlayerRng.Rewards).Take(1).First().ToMutable();
        await RelicCmd.Obtain(relic, instance.Owner);
    }

    private static RewardType? GetRewardType(Reward reward)
    {
        return (RewardType?) typeof(Reward).GetProperty("RewardType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(reward);
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterRewardTaken")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterRewardTaken(AbstractModel __instance, Player player, Reward reward, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LoomingFruit loomingFruit || player != loomingFruit.Owner || GetRewardType(reward) != RewardType.Relic)
        {
            return true;
        }

        __result = AfterRewardTaken(loomingFruit);
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

        if (__instance is not LoomingFruit)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_LOOMING_FRUIT.description");
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

        if (__instance is not LoomingFruit)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_LOOMING_FRUIT.eventDescription");
        return false;
    }
}