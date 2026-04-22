namespace RebalancedSpire.scr.Core.Harmony.Relics;

using BaseLib.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryNeow)]
// ReSharper disable InconsistentNaming
public static class BoomingConchPatch
{
    private static int MaxCards => 2;
    private static readonly SpireField<RelicModel, int> CardsPlayed = new(() => 0);

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (__instance is not BoomingConch)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-BOOMING_CONCH.description");
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.ShowCounter), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShowCounter(RelicModel __instance, ref bool __result)
    {
        if (__instance is not BoomingConch boomingConch)
        {
            return true;
        }

        __result = boomingConch.Status == RelicStatus.Active;
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.DisplayAmount), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_DisplayAmount(RelicModel __instance, ref int __result)
    {
        if (__instance is not BoomingConch boomingConch)
        {
            return true;
        }

        __result = CardsPlayed.Get(boomingConch);
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCardPlayed))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCardPlayed(AbstractModel __instance, PlayerChoiceContext context, CardPlay cardPlay, ref Task __result)
    {
        if (__instance is not BoomingConch { Status: RelicStatus.Active } boomingConch || !CombatManager.Instance.IsInProgress || cardPlay.Card.Owner != boomingConch.Owner)
        {
            return true;
        }

        var cardsPlayed = CardsPlayed.Get(boomingConch) + 1;
        CardsPlayed.Set(boomingConch, cardsPlayed);
        boomingConch.Status = cardsPlayed >= MaxCards ? RelicStatus.Disabled : RelicStatus.Active;
        boomingConch.InvokeDisplayAmountChanged();
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyEnergyCostInCombat))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TryModifyEnergyCostInCombat(AbstractModel __instance, CardModel card, decimal originalCost, out decimal modifiedCost, ref bool __result)
    {
        if (__instance is not BoomingConch { Status: RelicStatus.Active } boomingConch || card.Owner != boomingConch.Owner)
        {
            modifiedCost = originalCost;
            return true;
        }

        modifiedCost = 0;
        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyStarCost))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TryModifyStarCost(AbstractModel __instance, CardModel card, decimal originalCost, out decimal modifiedCost, ref bool __result)
    {
        if (__instance is not BoomingConch { Status: RelicStatus.Active } boomingConch || card.Owner != boomingConch.Owner)
        {
            modifiedCost = originalCost;
            return true;
        }

        modifiedCost = 0;
        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.BeforeCombatStart))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BeforeCombatStart(AbstractModel __instance, ref Task __result)
    {
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

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCombatEnd))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCombatEnd(AbstractModel __instance, CombatRoom room, ref Task __result)
    {
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

    [HarmonyPatch(typeof(BoomingConch), nameof(BoomingConch.ModifyHandDraw))]
    [HarmonyPrefix]
    [UsedImplicitly]
    // ReSharper disable BuiltInTypeReferenceStyle
    private static bool PreFix_ModifyHandDraw(BoomingConch __instance, Player player, Decimal count, ref Decimal __result)
    {
        if (__instance.Owner != player)
        {
            return true;
        }

        __result = count;
        return false;
    }
}