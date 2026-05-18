namespace RebalancedSpire.Core.Harmony.Events;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class LostWispPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.LostWispConfig;

    private static async Task Claim(LostWisp instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), instance.Owner.Creature, instance.DynamicVars.Damage, null, null);
        await RelicCmd.Obtain<MegaCrit.Sts2.Core.Models.Relics.LostWisp>(instance.Owner);
        instance.SetEventFinished(instance.L10NLookup("LOST_WISP.pages.CLAIM.description"));
    }

    [HarmonyPatch(typeof(EventModel), "IsAllowed")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_IsAllowed(EventModel __instance, IRunState runState, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not LostWisp)
        {
            return true;
        }

        __result = runState.Players.All(p => p.Creature.CurrentHp >= 10);
        return false;
    }

    [HarmonyPatch(typeof(LostWisp), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(LostWisp __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new GoldVar(60),
            new DamageVar(8, ValueProp.Unblockable | ValueProp.Unpowered),
            new StringVar("Relic", ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.LostWisp>().Title.GetFormattedText())
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(LostWisp), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(LostWisp __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            new(__instance, () => Claim(__instance), "REBALANCEDSPIRE-LOST_WISP.pages.INITIAL.options.CLAIM", HoverTipFactory.FromRelic<MegaCrit.Sts2.Core.Models.Relics.LostWisp>()),
            new(__instance, __instance.Search, "LOST_WISP.pages.INITIAL.options.SEARCH")
        }.AsReadOnly();
        return false;
    }
}