namespace RebalancedSpire.scr.Core.Harmony.Events;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MorphicGrovePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.MorphicGroveConfig;

    private static async Task Group(MorphicGrove instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        await PlayerCmd.LoseGold(instance.DynamicVars["GroupCost"].BaseValue, instance.Owner, GoldLossType.Stolen);
        foreach (var cardModel in (await CardSelectCmd.FromDeckForTransformation(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 2), player: instance.Owner)).ToList())
        {
            await CardCmd.TransformToRandom(cardModel, instance.Owner.RunState.Rng.Niche, CardPreviewStyle.EventLayout);
        }
        instance.SetEventFinished(instance.L10NLookup("MORPHIC_GROVE.pages.GROUP.description"));
    }

    [HarmonyPatch(typeof(MorphicGrove), nameof(MorphicGrove.CanonicalVars), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(MorphicGrove __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("GroupCost", 99),
            new MaxHpVar(5)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(MorphicGrove), nameof(MorphicGrove.GenerateInitialOptions))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(MorphicGrove __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            new(__instance, () => Group(__instance), "REBALANCEDSPIRE-MORPHIC_GROVE.pages.INITIAL.options.GROUP", HoverTipFactory.Static(StaticHoverTip.Transform)),
            new(__instance, __instance.Loner, "MORPHIC_GROVE.pages.INITIAL.options.LONER")
        }.AsReadOnly();
        return false;
    }
}