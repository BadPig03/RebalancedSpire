namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Utils;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class DiamondDiademPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.DiamondDiadem;
    private static readonly AttachedState<DiamondDiadem, int> CardsPlayedThisTurn = new(() => 0);

    private static Task AfterCardPlayed(DiamondDiadem instance, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != instance.Owner || !CombatManager.Instance.IsInProgress)
        {
            return Task.CompletedTask;
        }

        var cardsPlayed = CardsPlayedThisTurn.GetValueOrDefault(instance, 0) + 1;
        CardsPlayedThisTurn.Set(instance, cardsPlayed);
        instance.Status = cardsPlayed <= instance.DynamicVars["CardThreshold"].BaseValue ? RelicStatus.Active : RelicStatus.Normal;
        instance.InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private static Task AfterCombatEnd(DiamondDiadem instance)
    {
        CardsPlayedThisTurn.Set(instance, 0);
        instance.Status = RelicStatus.Normal;
        instance.InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private static async Task BeforeSideTurnEnd(DiamondDiadem instance, PlayerChoiceContext choiceContext, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(instance.Owner.Creature))
        {
            return;
        }

        if (CardsPlayedThisTurn.GetValueOrDefault(instance, 0) <= instance.DynamicVars["CardThreshold"].BaseValue)
        {
            instance.Flash();
            await PowerCmd.Apply<DiamondDiademPower>(choiceContext, instance.Owner.Creature, 1, instance.Owner.Creature, null);
        }
        CardsPlayedThisTurn.Set(instance, 0);
        instance.Status = RelicStatus.Active;
        instance.InvokeDisplayAmountChanged();
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

        if (__instance is not DiamondDiadem diamondDiadem)
        {
            return true;
        }

        __result = AfterCardPlayed(diamondDiadem, cardPlay);
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

        if (__instance is not DiamondDiadem diamondDiadem)
        {
            return true;
        }

        __result = AfterCombatEnd(diamondDiadem);
        return false;
    }

    [HarmonyPatch(typeof(DiamondDiadem), "AfterSideTurnStart")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterSideTurnStart(DiamondDiadem __instance, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "BeforeSideTurnEnd")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BeforeSideTurnEnd(AbstractModel __instance, PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not DiamondDiadem diamondDiadem)
        {
            return true;
        }

        __result = BeforeSideTurnEnd(diamondDiadem, choiceContext, participants);
        return false;
    }

    [HarmonyPatch(typeof(DiamondDiadem), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(DiamondDiadem __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("CardThreshold", 2)
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

        if (__instance is not DiamondDiadem)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_DIAMOND_DIADEM.description");
        return false;
    }

    [HarmonyPatch(typeof(DiamondDiadem), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(DiamondDiadem __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>().AsReadOnly();
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

        if (__instance is not DiamondDiadem diamondDiadem)
        {
            return true;
        }

        __result = CardsPlayedThisTurn.GetValueOrDefault(diamondDiadem, 0);
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

        if (__instance is not DiamondDiadem)
        {
            return true;
        }

        __result = CombatManager.Instance.IsInProgress;
        return false;
    }
}