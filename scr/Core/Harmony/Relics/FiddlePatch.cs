namespace RebalancedSpire.scr.Core.Harmony.Relics;

using BaseLib.Patches.Hooks;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FiddlePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.FiddleConfig;

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Description), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Description(RelicModel __instance, ref LocString __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (__instance is not Fiddle)
        {
            return true;
        }

        __result = new LocString("relics", "REBALANCEDSPIRE-FIDDLE.description");
        return false;
    }

    [HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ShouldDraw))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ShouldDraw(Fiddle __instance, Player player, bool fromHandDraw, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        if (player != __instance.Owner)
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(Fiddle), nameof(Fiddle.AfterPreventingDraw))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterPreventingDraw(ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(MaxHandSizePatch), nameof(MaxHandSizePatch.GetMaxHandSize))]
    [HarmonyPostfix]
    [UsedImplicitly]
    public static void PostFix_ModifyMaxHandSize(Player player, ref int __result)
    {
        if (Disabled)
        {
            return;
        }

        var value = 0;
        foreach (var relic in player.Relics)
        {
            if (relic is not Fiddle fiddle)
            {
                continue;
            }

            value -= fiddle.DynamicVars.Cards.IntValue;
        }
        __result = Math.Max(0, __result + value);
    }
}