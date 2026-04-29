namespace RebalancedSpire.scr.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WarHammerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.WarHammerConfig;

    private static Task AfterDeath(WarHammer instance)
    {
        instance.Flash();
        var enumerable = PileType.Deck.GetPile(instance.Owner).Cards.Where(c => c.IsUpgradable).ToList().StableShuffle(instance.Owner.RunState.Rng.Niche).Take(instance.DynamicVars.Cards.IntValue);
        foreach (CardModel item in enumerable)
        {
            CardCmd.Upgrade(item);
        }

        return Task.CompletedTask;
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

        if (__instance is not WarHammer)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-WAR_HAMMER.description");
        return false;
    }

    [HarmonyPatch(typeof(WarHammer), nameof(WarHammer.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(WarHammer __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(1)
        };
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterDeath))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterDeath(AbstractModel __instance, PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not WarHammer warHammer || wasRemovalPrevented || creature.HasPower<MinionPower>())
        {
            return true;
        }

        __result = AfterDeath(warHammer);
        return false;
    }

    [HarmonyPatch(typeof(WarHammer), nameof(WarHammer.AfterCombatVictory))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatVictory(WarHammer __instance, CombatRoom room, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }
}