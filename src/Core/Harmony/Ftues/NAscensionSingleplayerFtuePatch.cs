namespace RebalancedSpire.Core.Harmony.Ftues;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NAscensionSingleplayerFtuePatch
{
    [HarmonyPatch(typeof(NAscensionSingleplayerFtue), "_Ready")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Ready(NAscensionSingleplayerFtue __instance)
    {
        if (!__instance.HasMeta(StartRunLobbyPatch.RebalancedMetaKey))
        {
            return true;
        }

        __instance.GetNode<MegaLabel>("%Header").SetTextAutoSize(new LocString("ftues", "REBALANCED_SPIRE_FIRST_TIME_FTUE_TITLE").GetFormattedText());
        __instance.GetNode<MegaRichTextLabel>("%Description").SetTextAutoSize(new LocString("ftues", "REBALANCED_SPIRE_FIRST_TIME_FTUE_DESCRIPTION").GetFormattedText());
        __instance.GetNode<MegaRichTextLabel>("%Disclaimer").SetTextAutoSize(new LocString("ftues", "REBALANCED_SPIRE_FIRST_TIME_FTUE_DISCLAIMER").GetFormattedText());
        __instance.GetNode<NButton>("%FtueConfirmButton").Connect(NClickableControl.SignalName.Released, Callable.From((Action<NButton>)(_ =>
        {
            SaveManager.Instance.MarkFtueAsComplete(StartRunLobbyPatch.RebalancedFtueId);
            __instance.CloseFtue();
        })));
        Tween tween = __instance.CreateTween().SetParallel();
        Color modulate = __instance.Modulate;
        modulate.A = 0f;
        __instance.Modulate = modulate;
        tween.TweenProperty(__instance, "position:y", __instance.Position.Y, 0.3).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back).From(__instance.Position.Y + 100f).SetDelay(1.0);
        tween.TweenProperty(__instance, "modulate:a", 1f, 0.3).SetEase(Tween.EaseType.Out).SetDelay(1.0);
        return false;
    }
}