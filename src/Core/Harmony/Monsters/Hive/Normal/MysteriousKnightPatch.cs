namespace RebalancedSpire.Core.Harmony.Monsters.Hive.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class MysteriousKnightPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheLanternKeyConfig;

    private const int StrengthPowerAmount = 3;
    private const int PlatingPowerAmount = 3;

    private static async Task AfterAddedToRoom(MysteriousKnight instance)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, StrengthPowerAmount, instance.Creature, null);
        await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), instance.Creature, PlatingPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(MysteriousKnight), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AfterAddedToRoom(MysteriousKnight __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }
}