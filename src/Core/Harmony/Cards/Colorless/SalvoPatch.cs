namespace RebalancedSpire.Core.Harmony.Cards.Colorless;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SalvoPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Salvo;

    private static async Task OnPlay(Salvo instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (instance.CombatState == null)
        {
            return;
        }

        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance, cardPlay).TargetingAllOpponents(instance.CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await PowerCmd.Apply<RetainHandPower>(choiceContext, instance.Owner.Creature, 1, instance.Owner.Creature, instance);
    }

    [HarmonyPatch(typeof(Salvo), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Salvo __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(8, ValueProp.Move)
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

        if (__instance is not Salvo)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_SALVO.description");
        return false;
    }

    [HarmonyPatch(typeof(Salvo), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Salvo __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(Salvo), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Salvo __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Damage.UpgradeValueBy(3);
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TargetType(CardModel __instance, ref TargetType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Salvo)
        {
            return true;
        }

        __result = TargetType.AllEnemies;
        return false;
    }
}