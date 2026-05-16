namespace RebalancedSpire.Core.Harmony.Events;

using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TrialPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TrialConfig;

    private static readonly MethodInfo? _setEventStateMethod = typeof(EventModel).GetMethod("SetEventState", BindingFlags.NonPublic | BindingFlags.Instance, null,[typeof(LocString), typeof(IEnumerable<EventOption>)], null);

    private static async Task MerchantInnocent(Trial instance)
    {
	    if (instance.Owner == null)
	    {
		    return;
	    }

	    var list = (await CardSelectCmd.FromDeckForUpgrade(prefs: new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1), player: instance.Owner)).ToList();
	    foreach (var card in list)
	    {
		    CardCmd.Upgrade(card);
	    }
	    instance.SetTrialFinished("TRIAL.pages.MERCHANT_INNOCENT.description");
    }

    private static async Task NondescriptInnocent(Trial instance)
    {
	    if (instance.Owner == null)
	    {
		    return;
	    }

	    var list = (await CardSelectCmd.FromDeckForTransformation(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), player: instance.Owner)).ToList();
	    foreach (var card in list)
	    {
		    await CardCmd.TransformToRandom(card, instance.Owner.RunState.Rng.Niche, CardPreviewStyle.EventLayout);
	    }
	    instance.SetTrialFinished("TRIAL.pages.NONDESCRIPT_INNOCENT.description");
    }

    private static Task Accept(Trial instance)
    {
	    if (instance.Owner == null)
	    {
		    return Task.CompletedTask;
	    }

        if (LocalContext.IsMe(instance.Owner))
		{
			NEventRoom.Instance?.Layout?.RemoveNodesOnPortrait();
		}
		string portraitPath;
		string entryName;
		EventOption[] eventOptions;
		switch (instance.Rng.NextInt(3))
		{
			case 0:
				portraitPath = Trial._trialMerchantVfx;
				entryName = "TRIAL.pages.MERCHANT.description";
				eventOptions =
				[
					new EventOption(instance, instance.MerchantGuilty, "TRIAL.pages.MERCHANT.options.GUILTY", HoverTipFactory.FromCardWithCardHoverTips<Regret>()),
					new EventOption(instance, () => MerchantInnocent(instance), "REBALANCEDSPIRE-TRIAL.pages.MERCHANT.options.INNOCENT")
				];
				break;
			case 1:
				portraitPath = Trial._trialNobleVfx;
				entryName = "TRIAL.pages.NOBLE.description";
				eventOptions =
				[
					new EventOption(instance, instance.NobleGuilty, "TRIAL.pages.NOBLE.options.GUILTY"),
					new EventOption(instance, instance.NobleInnocent, "TRIAL.pages.NOBLE.options.INNOCENT", HoverTipFactory.FromCardWithCardHoverTips<Regret>())
				];
				break;
			case 2:
				portraitPath = Trial._trialNondescriptVfx;
				entryName = "TRIAL.pages.NONDESCRIPT.description";
				eventOptions =
				[
					new EventOption(instance, instance.NondescriptGuilty, "TRIAL.pages.NONDESCRIPT.options.GUILTY", HoverTipFactory.FromCardWithCardHoverTips<Doubt>()),
					new EventOption(instance, () => NondescriptInnocent(instance), "REBALANCEDSPIRE-TRIAL.pages.NONDESCRIPT.options.INNOCENT", HoverTipFactory.Static(StaticHoverTip.Transform))
				];
				break;
			default:
				throw new InvalidOperationException();
		}
		instance.AddVfxAnchoredToPortrait(portraitPath);
		if (LocalContext.IsMe(instance.Owner))
		{
			NEventRoom.Instance?.SetPortrait(PreloadManager.Cache.GetTexture2D(Trial.TrialStartedPath));
		}
		LocString locString = instance.L10NLookup("TRIAL.trialFormat");
		locString.Add(new StringVar("TrialStory", instance.L10NLookup(entryName).GetRawText()));
		_setEventStateMethod?.Invoke(instance, [locString, eventOptions]);
		return Task.CompletedTask;
    }

    [HarmonyPatch(typeof(Trial), "Accept")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Accept(Trial __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Accept(__instance);
        return false;
    }
}