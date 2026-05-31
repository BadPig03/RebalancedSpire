namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Normal;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ScrollOfBitingPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.ScrollOfBiting;

    private const int PaperCutsPowerAmount = 1;

    private static async Task AfterAddedToRoom(ScrollOfBiting instance)
    {
        await PowerCmd.Apply<PaperCutsPower>(new ThrowingPlayerChoiceContext(), instance.Creature, PaperCutsPowerAmount, instance.Creature, null);
    }

    [HarmonyPatch(typeof(ScrollOfBiting), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(ScrollOfBiting __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }
}