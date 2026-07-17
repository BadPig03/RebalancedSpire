namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class HistoryCoursePatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.WarHistorianRepy;

    private static async Task AfterAutoPrePlayPhaseEntered(HistoryCourse instance, PlayerChoiceContext choiceContext, Player player)
    {
        if (player != instance.Owner || instance.Owner.PlayerCombatState?.TurnNumber == 1)
        {
            return;
        }

        var cardModel = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(e => e.CardPlay.Player == instance.Owner && e.HappenedLastPlayerTurn(instance.Owner) && e.CardPlay.Card.Type is CardType.Attack or CardType.Skill && !e.CardPlay.Card.IsDupe)?.CardPlay.Card;
        if (cardModel == null)
        {
            return;
        }

        instance.Flash();
        await CardCmd.AutoPlay(choiceContext, cardModel.CreateDupe(player), null);
    }

    [HarmonyPatch(typeof(HistoryCourse), "AfterAutoPrePlayPhaseEntered")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAutoPrePlayPhaseEntered(HistoryCourse __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAutoPrePlayPhaseEntered(__instance, choiceContext, player);
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

        if (__instance is not HistoryCourse)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCED_SPIRE_RELIC_HISTORY_COURSE.description");
        return false;
    }
}