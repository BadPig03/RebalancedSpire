namespace RebalancedSpire.scr.Core.Harmony;

using Core.Monsters;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class AttackCommandPatch
{
    [HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.TargetingRandomOpponents))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TargetingRandomOpponents(AttackCommand __instance, CombatState combatState, bool allowDuplicates, ref AttackCommand __result)
    {
        if (!combatState.ContainsMonster<DoormakerBase>())
        {
            return true;
        }

        if (__instance._singleTarget != null) {
            throw new InvalidOperationException("Targets already set.");
        }
        if (__instance._combatState != null) {
            throw new InvalidOperationException("Already set to target opponents of attacker");
        }
        if (__instance.Attacker == null) {
            throw new InvalidOperationException("We require an attacker to be able to grab its opponents");
        }

        if (__instance.Attacker.GetPower<OmnidynamicsPower>() == null && __instance.Attacker.PetOwner?.Creature.GetPower<OmnidynamicsPower>() == null)
        {
            return true;
        }

        __instance._combatState = combatState;
        __instance.TargetSide = __instance.Attacker?.Side == CombatSide.Enemy ? CombatSide.Player : CombatSide.Enemy;
        __result = __instance;
        return false;
    }
}