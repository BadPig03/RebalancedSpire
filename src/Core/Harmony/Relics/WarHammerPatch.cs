namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
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
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Utils;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WarHammerPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.WarHammer;

    private static readonly SavedAttachedState<WarHammer, int> EnemiesKilled = new("REBALANCED_SPIRE_RELIC_WAR_HAMMER", () => 0);

    private static Task AfterCombatVictory(WarHammer instance)
    {
        var killed = EnemiesKilled[instance];
        for (var i = 0; i < killed; i++)
        {
            var cards = PileType.Deck.GetPile(instance.Owner).Cards.Where(c => c.IsUpgradable).ToList().StableShuffle(instance.Owner.PlayerRng.Rewards).Take(instance.DynamicVars.Cards.IntValue).ToList();
            foreach (var card in cards)
            {
                CardCmd.Upgrade(card);
            }
        }
        EnemiesKilled.Set(instance, 0);
        instance.Flash();
        instance.InvokeDisplayAmountChanged();
        return Task.CompletedTask;
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

        __result = AfterCombatVictory(__instance);
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterDamageGiven")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterDamageGiven(AbstractModel __instance, PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not WarHammer warHammer || !target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()) || !result.WasTargetKilled)
        {
            return true;
        }

        EnemiesKilled.Update(warHammer, i => i + 1);
        warHammer.Flash();
        warHammer.InvokeDisplayAmountChanged();
        __result = Task.CompletedTask;
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

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_WAR_HAMMER.description");
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

        if (__instance is not WarHammer warHammer)
        {
            return true;
        }

        __result = EnemiesKilled[warHammer];
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "ShowCounter", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShowCounter(RelicModel __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not WarHammer)
        {
            return true;
        }

        __result = true;
        return false;
    }

}