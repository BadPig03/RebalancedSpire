namespace RebalancedSpire.Core.Harmony.Cards.Necrobinder;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WispPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Wisp;

    private static async Task OnPlay(Wisp instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainEnergy(instance.DynamicVars.Energy.BaseValue, instance.Owner);
        var soul = PileType.Draw.GetPile(instance.Owner).Cards.OfType<Soul>().ToList().StableShuffle(instance.Owner.PlayerRng.Transformations).FirstOrDefault();
        if (soul == null)
        {
            return;
        }

        await CardCmd.Transform(soul, instance.CreateClone());
    }

    [HarmonyPatch(typeof(Wisp), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Wisp __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new EnergyVar(1)
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

        if (__instance is not Wisp)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCED_SPIRE_CARD_WISP.description");
        return false;
    }

    [HarmonyPatch(typeof(Wisp), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(Wisp __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance);
        return false;
    }

    [HarmonyPatch(typeof(Wisp), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(Wisp __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Energy.UpgradeValueBy(1);
        return false;
    }
}