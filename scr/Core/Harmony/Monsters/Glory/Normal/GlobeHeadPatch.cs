namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory.Normal;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class GlobeHeadPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.GlobeHeadConfig;

    private static int GalvanicPowerAmount => 3;

    private static async Task AfterAddedToRoom(GlobeHead instance)
    {
        await PowerCmd.Apply<GalvanicPower>(new ThrowingPlayerChoiceContext(), instance.Creature, GalvanicPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(GlobeHead), nameof(GlobeHead.AfterAddedToRoom))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(GlobeHead __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }
}