namespace RebalancedSpire.Core.Harmony.RitsuLib;

using Core.Nodes.Screens.MainMenu;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2RitsuLib.Scaffolding.Godot.NodeAttachments;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class MainMenuModSettingsButtonPatchPatch
{
    [HarmonyPatch("STS2RitsuLib.Settings.Patches.MainMenuModSettingsButtonPatch", "IsMainMenuShortcutSurfaceVisible")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_IsMainMenuShortcutSurfaceVisible(NMainMenu mainMenu, ref bool __result)
    {
        var id = ModNodeAttachmentRegistry.GetQualifiedNodeAttachmentId(RebalancedSpireMain.ModId, "ModChangelogsScreen");
        if (!ModNodeAttachmentRegistry.TryGetAttachedById<NMainMenu, NModChangelogsScreen>(mainMenu, id, out var screenNode) || !screenNode.IsVisible())
        {
            return;
        }

        __result = false;
    }
}