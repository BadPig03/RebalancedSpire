namespace RebalancedSpire.Core.Harmony.Monsters.Underdocks.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class SewerClamPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SewerClamConfig;

    private static int PlatingPowerAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 7, 6);

    private static async Task AfterAddedToRoom(SewerClam instance)
    {
        await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), instance.Creature, PlatingPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(SewerClam), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(SewerClam __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }
}