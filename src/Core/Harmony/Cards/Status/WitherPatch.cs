namespace RebalancedSpire.Core.Harmony.Cards.Status;

using Afflictions;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WitherPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Aeonglass;

    private static async Task OnTurnEndInHand(Wither instance, PlayerChoiceContext choiceContext)
    {
        if (instance.Affliction is not Withering)
        {
            await CreatureCmd.Damage(choiceContext, instance.Owner.Creature, new DamageVar(instance.DynamicVars["Fixed"].BaseValue, ValueProp.Unpowered | ValueProp.Move), instance);
            return;
        }

        if (instance.FakeUpgradeLevel == 0)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, instance.Owner.Creature, instance.DynamicVars.Damage, instance);
    }

    [HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalEnergyCost(CardModel __instance, ref int __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Wither)
        {
            return true;
        }

        __result = 1;
        return false;
    }

    [HarmonyPatch(typeof(Wither), "CanonicalKeywords", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalKeywords(Wither __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<CardKeyword>().AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Wither), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Wither __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(0, ValueProp.Unpowered | ValueProp.Move),
            new ("Fixed", 6),
            new ("PerLevel", 3)
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

        if (__instance is not Wither wither)
        {
            return true;
        }

        LocString result;
        if (wither.Affliction is Withering)
        {
            result = wither.FakeUpgradeLevel == 0 && CombatManager.Instance.IsInProgress ? new LocString("cards", "REBALANCED_SPIRE_CARD_WITHER.extraDescription") : new LocString("cards", "REBALANCED_SPIRE_CARD_WITHER.description");
        }
        else {
            result = new LocString("cards", "REBALANCED_SPIRE_CARD_WITHER.extraDescription2");
        }
        __result = result;
        return false;
    }

    [HarmonyPatch(typeof(Wither), "FakeUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_FakeUpgrade(Wither __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.FakeUpgradeLevel++;
        __instance.DynamicVars.Damage.UpgradeValueBy(__instance.DynamicVars["PerLevel"].BaseValue);
        return false;
    }

    [HarmonyPatch(typeof(Wither), "HasTurnEndInHandEffect", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_HasTurnEndInHandEffect(Wither __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        bool result;
        if (__instance.Affliction is Withering)
        {
            result = __instance.FakeUpgradeLevel != 0;
        }
        else
        {
            result = true;
        }
        __result = result;
        return false;
    }

    [HarmonyPatch(typeof(Wither), "OnTurnEndInHand")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnTurnEndInHand(Wither __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnTurnEndInHand(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "ShouldGlowGoldInternal", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShouldGlowGoldInternal(CardModel __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Wither wither)
        {
            return true;
        }

        __result = wither is { Affliction: Withering, FakeUpgradeLevel: 0 };
        return false;
    }

    [HarmonyPatch(typeof(Wither), "Title", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Title(Wither __instance, ref string __result)
    {
        if (Disabled)
        {
            return true;
        }

        string result;
        var title = __instance.TitleLocString.GetFormattedText();
        if (__instance.Affliction is not Withering || __instance.FakeUpgradeLevel == 0)
        {
            result = title;
        }
        else
        {
            result = $"{title}+{__instance.FakeUpgradeLevel}";
        }
        __result = result;
        return false;
    }
}