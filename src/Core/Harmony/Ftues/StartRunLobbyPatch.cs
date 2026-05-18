namespace RebalancedSpire.Core.Harmony.Ftues;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Saves;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class StartRunLobbyPatch
{
    internal const string RebalancedFtueId = "rebalanced_spire_first_time_ftue";
    internal const string RebalancedMetaKey = "rebalanced_spire_ftue";

    [HarmonyPatch(typeof(StartRunLobby), "SetSingleplayerAscensionAfterCharacterChanged")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_SetSingleplayerAscensionAfterCharacterChanged(StartRunLobby __instance, ModelId characterId)
    {
        if (SaveManager.Instance.SeenPopup(RebalancedFtueId))
        {
            return;
        }

        var ftue = NAscensionSingleplayerFtue.Create();
        if (ftue == null)
        {
            return;
        }

        ftue.SetMeta(RebalancedMetaKey, true);
        NModalContainer.Instance?.Add(ftue);
    }
}