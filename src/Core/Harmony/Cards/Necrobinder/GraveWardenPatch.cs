namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class GraveWardenPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.GraveWarden;

    private static async Task OnPlay(GraveWarden instance, CardPlay cardPlay)
    {
        var combatState = instance.CombatState;
        if (combatState == null)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(instance.Owner.Creature, instance.DynamicVars.Block, cardPlay);
        var souls = Soul.Create(instance.Owner, instance.DynamicVars.Cards.IntValue, combatState).ToList();
        if (instance.IsUpgraded)
        {
            foreach (var soul in souls)
            {
                CardCmd.Upgrade(soul);
            }
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(souls, PileType.Draw, instance.Owner, CardPilePosition.Random));
    }

    [HarmonyPatch(typeof(GraveWarden), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(GraveWarden __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new BlockVar(8, ValueProp.Move),
            new CardsVar(1)
        }.AsReadOnly();
        return false;
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

        if (__instance is not GraveWarden)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_GRAVE_WARDEN.description");
        return false;
    }

    [HarmonyPatch(typeof(GraveWarden), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(GraveWarden __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromCard<Soul>(__instance.IsUpgraded)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(GraveWarden), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(GraveWarden __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(GraveWarden), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(GraveWarden __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Block.UpgradeValueBy(2);
        return false;
    }
}