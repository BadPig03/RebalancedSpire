namespace RebalancedSpire.Core.Harmony.Cards.Event;

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
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class GodByrdPatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.GodByrd;

    private static async Task OnPlay(GodByrd instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (instance.CombatState == null)
        {
            return;
        }

        var byrdine = instance.Owner.PlayerCombatState?.GetPet<ByrdineMonster>();
        if (byrdine != null)
        {
            await CreatureCmd.TriggerAnim(byrdine, "Attack", 0f);
            await Cmd.Wait(0.6f);
            SfxCmd.Play("event:/sfx/byrdpip/byrd_2_attack");
            NByrdineAttackVfx.Create(byrdine);
            await Cmd.Wait(0.15f);
        }
        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance, cardPlay).TargetingAllOpponents(instance.CombatState).WithAttackerAnim(null, 0f).Execute(choiceContext);
    }

    [HarmonyPatch(typeof(GodByrd), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(GodByrd __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(25, ValueProp.Move)
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

        if (__instance is not GodByrd)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_GOD_BYRD.description");
        return false;
    }

    [HarmonyPatch(typeof(GodByrd), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(GodByrd __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext, cardPlay);
        return false;
    }

    [HarmonyPatch(typeof(GodByrd), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(GodByrd __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Damage.UpgradeValueBy(7);
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

        if (__instance is not GodByrd)
        {
            return true;
        }

        __result = TargetType.AllEnemies;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Type(CardModel __instance, ref CardType __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not GodByrd)
        {
            return true;
        }

        __result = CardType.Attack;
        return false;
    }
}
