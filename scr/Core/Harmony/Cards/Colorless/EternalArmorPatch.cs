namespace RebalancedSpire.scr.Core.Harmony.Cards.Colorless;

using Core.Powers;
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
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class EternalArmorPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.EternalArmorConfig;

    private static async Task OnPlay(EternalArmor instance, PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<PlatingPower>(choiceContext, instance.Owner.Creature, instance.DynamicVars["PlatingPower"].IntValue, instance.Owner.Creature, instance);
        await PowerCmd.Apply<EternalArmorPower>(choiceContext, instance.Owner.Creature, 1, instance.Owner.Creature, instance);
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(CardModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not EternalArmor)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-ETERNAL_ARMOR.description");
        return false;
    }

    [HarmonyPatch(typeof(EternalArmor), nameof(EternalArmor.ExtraHoverTips), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(EternalArmor __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromPower<PlatingPower>(),
            HoverTipFactory.FromPower<EternalArmorPower>(),
            HoverTipFactory.Static(StaticHoverTip.Block)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(EternalArmor), nameof(EternalArmor.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(EternalArmor __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<PlatingPower>(11)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(EternalArmor), nameof(EternalArmor.OnPlay))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(EternalArmor __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(EternalArmor), nameof(EternalArmor.OnUpgrade))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(EternalArmor __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.AddKeyword(CardKeyword.Retain);
        return false;
    }
}