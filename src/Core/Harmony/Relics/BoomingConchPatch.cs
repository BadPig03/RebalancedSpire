namespace RebalancedSpire.Core.Harmony.Relics;

using BaseLib.Utils;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BoomingConchPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BoomingConchConfig;
    private static readonly SpireField<RelicModel, int> CardsPlayed = new(() => 0);

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not BoomingConch)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-BOOMING_CONCH.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(RelicModel __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not BoomingConch)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new CardsVar(2)
        }.AsReadOnly();
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

        if (__instance is not BoomingConch boomingConch)
        {
            return true;
        }

        __result = boomingConch.Status == RelicStatus.Active;
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

        if (__instance is not BoomingConch boomingConch)
        {
            return true;
        }

        __result = CardsPlayed.Get(boomingConch);
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterCardPlayed")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCardPlayed(AbstractModel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not BoomingConch { Status: RelicStatus.Active } boomingConch || !CombatManager.Instance.IsInProgress || cardPlay.Card.Owner != boomingConch.Owner)
        {
            return true;
        }

        var cardsPlayed = CardsPlayed.Get(boomingConch) + 1;
        CardsPlayed.Set(boomingConch, cardsPlayed);
        boomingConch.Status = cardsPlayed >= boomingConch.DynamicVars.Cards.IntValue ? RelicStatus.Disabled : RelicStatus.Active;
        boomingConch.InvokeDisplayAmountChanged();
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "TryModifyEnergyCostInCombat")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TryModifyEnergyCostInCombat(AbstractModel __instance, CardModel card, decimal originalCost, out decimal modifiedCost, ref bool __result)
    {
        if (Disabled)
        {
            modifiedCost = originalCost;
            return true;
        }

        if (__instance is not BoomingConch { Status: RelicStatus.Active } boomingConch || card.Owner != boomingConch.Owner)
        {
            modifiedCost = originalCost;
            return true;
        }

        modifiedCost = 0;
        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "TryModifyStarCost")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TryModifyStarCost(AbstractModel __instance, CardModel card, decimal originalCost, out decimal modifiedCost, ref bool __result)
    {
        if (Disabled)
        {
            modifiedCost = originalCost;
            return true;
        }

        if (__instance is not BoomingConch { Status: RelicStatus.Active } boomingConch || card.Owner != boomingConch.Owner)
        {
            modifiedCost = originalCost;
            return true;
        }

        modifiedCost = 0;
        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "BeforeCombatStart")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BeforeCombatStart(AbstractModel __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not BoomingConch boomingConch)
        {
            return true;
        }

        var currentRoom = boomingConch.Owner.Creature.CombatState?.RunState.CurrentRoom;
        if (currentRoom is not CombatRoom || currentRoom.RoomType != RoomType.Elite)
        {
            return true;
        }

        boomingConch.Status = RelicStatus.Active;
        CardsPlayed.Set(boomingConch, 0);
        boomingConch.InvokeDisplayAmountChanged();
        __result = Task.CompletedTask;
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

        if (__instance is not BoomingConch boomingConch)
        {
            return true;
        }

        boomingConch.Status = RelicStatus.Normal;
        CardsPlayed.Set(boomingConch, 0);
        boomingConch.InvokeDisplayAmountChanged();
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(BoomingConch), "AfterSideTurnStart")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterSideTurnStart(BoomingConch __instance, CombatSide side, ICombatState combatState, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(BoomingConch), "ModifyHandDraw")]
    [HarmonyPrefix]
    [UsedImplicitly]
    // ReSharper disable BuiltInTypeReferenceStyle
    private static bool PreFix_ModifyHandDraw(BoomingConch __instance, Player player, Decimal count, ref Decimal __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance.Owner != player)
        {
            return true;
        }

        __result = count;
        return false;
    }
}