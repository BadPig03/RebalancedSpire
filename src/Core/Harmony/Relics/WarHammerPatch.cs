namespace RebalancedSpire.Core.Harmony.Relics;

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
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WarHammerPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.WarHammerConfig;

    private static Task UpgradeARandomCard(WarHammer instance)
    {
        instance.Flash();
        foreach (var card in PileType.Deck.GetPile(instance.Owner).Cards.Where(c => c.IsUpgradable).ToList().StableShuffle(instance.Owner.PlayerRng.Rewards).Take(instance.DynamicVars.Cards.IntValue))
        {
            CardCmd.Upgrade(card);
        }
        return Task.CompletedTask;
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

        if (__instance is not WarHammer)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-WAR_HAMMER.description");
        return false;
    }

    [HarmonyPatch(typeof(WarHammer), "CanonicalVars", MethodType.Getter)]
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
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterDeath")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterDeath(AbstractModel __instance, PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not WarHammer warHammer || creature.HasPower<MinionPower>() || creature.IsPet || !creature.IsMonster || creature.CombatState?.Enemies.Any(c => c.IsAlive) == false)
        {
            return true;
        }

        __result = UpgradeARandomCard(warHammer);
        return false;
    }

    [HarmonyPatch(typeof(WarHammer), "AfterCombatVictory")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatVictory(WarHammer __instance, CombatRoom room, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = UpgradeARandomCard(__instance);
        return false;
    }
}