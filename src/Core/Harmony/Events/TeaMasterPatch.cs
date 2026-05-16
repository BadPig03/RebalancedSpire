namespace RebalancedSpire.Core.Harmony.Events;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using Potions;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TeaMasterPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TeaMasterConfig;

    private static async Task BoneTea(TeaMaster instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        var result = await PotionCmd.TryToProcure<BoneTeaPotion>(instance.Owner);
        if (result.success)
        {
            await PlayerCmd.LoseGold(instance.DynamicVars["BoneTeaCost"].BaseValue, instance.Owner, GoldLossType.Spent);
        }
        instance.SetEventFinished(instance.L10NLookup("TEA_MASTER.pages.DONE.description"));
    }

    private static async Task EmberTea(TeaMaster instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        for (var i = 0; i <= 1; i++) {
            var result = await PotionCmd.TryToProcure<EmberTeaPotion>(instance.Owner);
            if (result.success)
            {
                await PlayerCmd.LoseGold(instance.DynamicVars["EmberTeaCost"].BaseValue / 2, instance.Owner, GoldLossType.Spent);
            }
        }
        instance.SetEventFinished(instance.L10NLookup("TEA_MASTER.pages.DONE.description"));
    }

    private static async Task TeaOfDiscourtesy(TeaMaster instance)
    {
        if (instance.Owner == null)
        {
            return;
        }

        await PotionCmd.TryToProcure<TeaOfDiscourtesyPotion>(instance.Owner);
        instance.SetEventFinished(instance.L10NLookup("TEA_MASTER.pages.TEA_OF_DISCOURTESY.description"));
    }

    [HarmonyPatch(typeof(TeaMaster), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(TeaMaster __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new("BoneTeaCost", 50),
            new("EmberTeaCost", 150),
            new StringVar("BoneTeaTitle", ModelDb.Potion<BoneTeaPotion>().Title.GetFormattedText()),
            new StringVar("EmberTeaTitle", ModelDb.Potion<EmberTeaPotion>().Title.GetFormattedText()),
            new StringVar("TeaOfDiscourtesyTitle", ModelDb.Potion<TeaOfDiscourtesyPotion>().Title.GetFormattedText()),
            new StringVar("BoneTeaDescription", ModelDb.Potion<BoneTeaPotion>().DynamicDescription.GetFormattedText()),
            new StringVar("EmberTeaDescription", ModelDb.Potion<EmberTeaPotion>().DynamicDescription.GetFormattedText()),
            new StringVar("TeaOfDiscourtesyDescription", ModelDb.Potion<TeaOfDiscourtesyPotion>().DynamicDescription.GetFormattedText())
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(TeaMaster), "IsAllowed")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_IsAllowed(TeaMaster __instance, IRunState runState, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = runState.CurrentActIndex < 2 && runState.Players.All(p => p is { Gold: >= 150, HasOpenPotionSlots: true });
        return false;
    }

    [HarmonyPatch(typeof(TeaMaster), "GenerateInitialOptions")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(TeaMaster __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            __instance.Owner?.Gold < __instance.DynamicVars["BoneTeaCost"].BaseValue ? new EventOption(__instance, null, "TEA_MASTER.pages.INITIAL.options.BONE_TEA_LOCKED") : new EventOption(__instance, () => BoneTea(__instance), "REBALANCEDSPIRE-TEA_MASTER.pages.INITIAL.options.BONE_TEA", HoverTipFactory.FromPotion<BoneTeaPotion>()),
            __instance.Owner?.Gold < __instance.DynamicVars["EmberTeaCost"].BaseValue ? new EventOption(__instance, null, "TEA_MASTER.pages.INITIAL.options.EMBER_TEA_LOCKED") : new EventOption(__instance, () => EmberTea(__instance), "REBALANCEDSPIRE-TEA_MASTER.pages.INITIAL.options.EMBER_TEA", HoverTipFactory.FromPotion<EmberTeaPotion>()),
            new(__instance, () => TeaOfDiscourtesy(__instance), "REBALANCEDSPIRE-TEA_MASTER.pages.INITIAL.options.TEA_OF_DISCOURTESY", HoverTipFactory.FromPotion<TeaOfDiscourtesyPotion>())
        }.AsReadOnly();
        return false;
    }
}