namespace RebalancedSpire.scr.Core.Harmony.Relics;

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
    private static readonly bool Disabled = !RebalancedSpireConfig.LoomingFruitConfig;

    private static readonly List<RelicModel> _fruitRelics =
    [
        ModelDb.Relic<Strawberry>(),
        ModelDb.Relic<Pear>(),
        ModelDb.Relic<Mango>(),
        ModelDb.Relic<DragonFruit>()
    ];

    private static async Task AfterRewardTaken(LoomingFruit instance)
    {
        await RelicCmd.Obtain(_fruitRelics.UnstableShuffle(instance.Owner.PlayerRng.Rewards).Take(1).First().ToMutable(), instance.Owner);
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

        if (__instance is not LoomingFruit)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-LOOMING_FRUIT.description");
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

        if (__instance is not LoomingFruit)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-LOOMING_FRUIT.eventDescription");
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterRewardTaken))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterRewardTaken(AbstractModel __instance, Player player, Reward reward, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LoomingFruit loomingFruit || player != loomingFruit.Owner || reward.RewardType != RewardType.Relic)
        {
            return true;
        }

        __result = AfterRewardTaken(loomingFruit);
        return false;
    }
}