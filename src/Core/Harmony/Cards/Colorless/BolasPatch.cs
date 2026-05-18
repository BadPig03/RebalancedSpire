namespace RebalancedSpire.Core.Harmony.Cards.Colorless;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BolasPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BolasConfig;

    private static async Task BeforeHandDraw(Bolas instance, Player player)
    {
        if (player != instance.Owner || !CombatManager.Instance.History.CardPlaysFinished.Any(e => e.RoundNumber == instance.CombatState?.RoundNumber - 1 || e.CardPlay.Card != instance))
        {
            return;
        }

        var pile = instance.Pile;
        if (pile is { Type: PileType.Hand })
        {
            return;
        }

        await CardPileCmd.Add(instance, PileType.Hand);
        instance.DynamicVars.Damage.BaseValue += instance.DynamicVars["IncrementAmount"].BaseValue;
    }

    [HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(CardModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Bolas)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-BOLAS.description");
        return false;
    }

    [HarmonyPatch(typeof(Bolas), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Bolas __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(3, ValueProp.Move),
            new ("IncrementAmount", 3)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Bolas), "BeforeHandDraw")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BeforeHandDraw(Bolas __instance, Player player, PlayerChoiceContext choiceContext, ICombatState combatState, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = BeforeHandDraw(__instance, player);
        return false;
    }

    [HarmonyPatch(typeof(Bolas), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Bolas __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Damage.UpgradeValueBy(1);
        __instance.DynamicVars["IncrementAmount"].UpgradeValueBy(1);
        return false;
    }
}