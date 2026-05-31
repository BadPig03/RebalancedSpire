namespace RebalancedSpire.Core.Harmony.Nodes;

using Core.Nodes.Screens.MainMenu;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2RitsuLib.Scaffolding.Godot.NodeAttachments;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NPatchNotesScreenPatch
{
    [HarmonyPatch(typeof(NPatchNotesScreen), "Close")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Close(NPatchNotesScreen __instance)
    {
        var mainMenu = __instance.GetParent<NMainMenu>();
        if (mainMenu == null)
        {
            return;
        }

        var id = ModNodeAttachmentRegistry.GetQualifiedNodeAttachmentId(RebalancedSpireMain.ModId, "ModChangelogsButton");
        if (!ModNodeAttachmentRegistry.TryGetAttachedById<NMainMenu, NModChangelogsButton>(mainMenu, id, out var buttonNode))
        {
            return;
        }

        buttonNode.SetVisible(true);
        buttonNode.Enable();
    }
}