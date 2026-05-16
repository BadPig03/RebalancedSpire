namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FollowThroughPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FollowThroughConfig;

    private static bool WasLastCardPlayedSkill(FollowThrough instance)
    {
        var entry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault(e => e.CardPlay.Card.Owner == instance.Owner && e.HappenedThisTurn(instance.CombatState) && e.CardPlay.Card != instance);
        if (entry == null)
        {
            return false;
        }

        return entry.CardPlay.Card.Type == CardType.Skill;
    }

    private static async Task OnPlay(FollowThrough instance, PlayerChoiceContext choiceContext)
    {
        if (instance.CombatState == null)
        {
            return;
        }

        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance).TargetingAllOpponents(instance.CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        if (!WasLastCardPlayedSkill(instance))
        {
            return;
        }

        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), instance.CombatState.HittableEnemies, instance.DynamicVars.Weak.BaseValue, instance.Owner.Creature, instance);
    }

    [HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Rarity(CardModel __instance, ref CardRarity __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not FollowThrough)
        {
            return true;
        }

        __result = CardRarity.Uncommon;
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

        if (__instance is not FollowThrough)
        {
            return true;
        }

        __result = new LocString("cards", "REBALANCEDSPIRE-FOLLOW_THROUGH.description");
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

        if (__instance is not FollowThrough)
        {
            return true;
        }

        __result = TargetType.AllEnemies;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not FollowThrough)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromPower<WeakPower>()
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(FollowThrough), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(FollowThrough __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(6, ValueProp.Move),
            new PowerVar<WeakPower>(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(FollowThrough), "ShouldGlowGoldInternal", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShouldGlowGoldInternal(FollowThrough __instance, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = WasLastCardPlayedSkill(__instance);
        return false;
    }

    [HarmonyPatch(typeof(FollowThrough), "OnPlay")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnPlay(FollowThrough __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = OnPlay(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(FollowThrough), "OnUpgrade")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_OnUpgrade(FollowThrough __instance)
    {
        if (Disabled)
        {
            return true;
        }

        __instance.DynamicVars.Damage.UpgradeValueBy(2);
        __instance.DynamicVars.Weak.UpgradeValueBy(1);
        return false;
    }
}