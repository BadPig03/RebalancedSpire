namespace RebalancedSpire.Core.Harmony.Relics;

using BaseLib.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WhisperingEarringPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.WhisperingEarringConfig;

    private static int maxCardsToPlay => 13;

    private static readonly SpireField<WhisperingEarring, int> CurrentEnergyUsed = new(() => 0);

    private static async Task VakuuAutoPlay(WhisperingEarring instance, PlayerChoiceContext choiceContext, Player player)
    {
        instance.Flash();
        bool flag;
        using (CardSelectCmd.PushSelector(new VakuuCardSelector()))
        {
            int cardsPlayed;
            for (cardsPlayed = 0; cardsPlayed < maxCardsToPlay; cardsPlayed++)
            {
                var combatState = player.Creature.CombatState;
                if (combatState == null || CombatManager.Instance.IsOverOrEnding || CombatManager.Instance.IsPlayerReadyToEndTurn(player))
                {
                    break;
                }

                var card = PileType.Hand.GetPile(instance.Owner).Cards.FirstOrDefault(c => c.CanPlay());
                if (card == null)
                {
                    break;
                }

                var target = instance.GetTarget(card, combatState);
                await card.SpendResources();
                await CardCmd.AutoPlay(choiceContext, card, target, skipXCapture: true);
            }
            flag = cardsPlayed >= maxCardsToPlay;
            if (cardsPlayed == 0)
            {
                return;
            }
        }
        LocString line = flag ? new LocString("relics", "WHISPERING_EARRING.warning") : new LocString("relics", "WHISPERING_EARRING.approval");
        TalkCmd.Play(line, instance.Owner.Creature, VfxColor.Purple);
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

        if (__instance is not WhisperingEarring)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-WHISPERING_EARRING.description");
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

        if (__instance is not WhisperingEarring whisperingEarring)
        {
            return true;
        }

        __result = CurrentEnergyUsed.Get(whisperingEarring) >= 0;
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

        if (__instance is not WhisperingEarring whisperingEarring)
        {
            return true;
        }

        __result = CurrentEnergyUsed.Get(whisperingEarring);
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterCardPlayedLate")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterCardPlayedLate(AbstractModel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not WhisperingEarring { Status: RelicStatus.Active } whisperingEarring || cardPlay.Card.Owner != whisperingEarring.Owner)
        {
            return true;
        }

        whisperingEarring.Status = RelicStatus.Normal;
        __result = VakuuAutoPlay(whisperingEarring, choiceContext, whisperingEarring.Owner);
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "AfterEnergySpent")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterEnergySpent(AbstractModel __instance, CardModel card, int amount, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not WhisperingEarring whisperingEarring || card.Owner != whisperingEarring.Owner || amount <= 0)
        {
            return true;
        }

        var current = CurrentEnergyUsed.Get(whisperingEarring);
        if (current < 0)
        {
            return true;
        }

        current += amount;
        CurrentEnergyUsed.Set(whisperingEarring, current);
        whisperingEarring.InvokeDisplayAmountChanged();
        if (current < maxCardsToPlay)
        {
            return true;
        }

        whisperingEarring.Status = RelicStatus.Active;
        CurrentEnergyUsed.Set(whisperingEarring, -1);
        whisperingEarring.InvokeDisplayAmountChanged();
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

        if (__instance is not WhisperingEarring whisperingEarring)
        {
            return true;
        }

        CurrentEnergyUsed.Set(whisperingEarring, 0);
        whisperingEarring.InvokeDisplayAmountChanged();
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(WhisperingEarring), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(WhisperingEarring __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new EnergyVar("TotalEnergy", maxCardsToPlay),
            new EnergyVar(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(WhisperingEarring), "AfterAutoPrePlayPhaseEnteredLate")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAutoPrePlayPhaseEnteredLate(WhisperingEarring __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (player != __instance.Owner)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }
}