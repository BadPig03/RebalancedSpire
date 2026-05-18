namespace RebalancedSpire.Core.Harmony.Relics;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SwordOfJadePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SunkenStatueConfig;

    private static async Task AfterRoomEntered(SwordOfJade instance)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Owner.Creature, instance.DynamicVars.Strength.BaseValue, null, null);
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), instance.Owner.Creature, instance.DynamicVars.Dexterity.BaseValue, null, null);
    }

    [HarmonyPatch(typeof(RelicModel), "Description", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not SwordOfJade)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-SWORD_OF_JADE.description");
        return false;
    }

    [HarmonyPatch(typeof(SwordOfJade), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(SwordOfJade __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new PowerVar<StrengthPower>(2),
            new PowerVar<DexterityPower>(1)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(SwordOfJade), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(SwordOfJade __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<IHoverTip>
        {
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>()
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(SwordOfJade), "AfterRoomEntered")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterRoomEntered(SwordOfJade __instance, AbstractRoom room, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (room is not CombatRoom)
        {
            return true;
        }

        __result = AfterRoomEntered(__instance);
        return false;
    }
}