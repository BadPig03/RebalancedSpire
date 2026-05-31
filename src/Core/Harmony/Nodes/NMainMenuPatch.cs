namespace RebalancedSpire.Core.Harmony.Nodes;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2RitsuLib.Scaffolding.Godot.NodeAttachments;
using NModChangelogsButton = Core.Nodes.Screens.MainMenu.NModChangelogsButton;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NMainMenuPatch
{
    [HarmonyPatch(typeof(NMainMenu), "_Ready")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Ready(NMainMenu __instance)
    {
        var id = ModNodeAttachmentRegistry.GetQualifiedNodeAttachmentId(RebalancedSpireMain.ModId, "ModChangelogsButton");
        var patchNotesNode = __instance._patchNotesButtonNode;
        patchNotesNode.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => OnPatchNotesChanged()));

        var submenuNode = __instance.SubmenuStack;
        submenuNode.Connect(NSubmenuStack.SignalName.StackModified, Callable.From(() => OnSubmenuStackChanged(submenuNode)));
        OnSubmenuStackChanged(submenuNode);
        return;

        void OnPatchNotesChanged()
        {
            if (!__instance.PatchNotesScreen.IsVisible() || !ModNodeAttachmentRegistry.TryGetAttachedById<NMainMenu, NModChangelogsButton>(__instance, id, out var buttonNode))
            {
                return;
            }

            buttonNode.SetVisible(false);
            buttonNode.Disable();
        }

        void OnSubmenuStackChanged(NMainMenuSubmenuStack stack)
        {
            if (!ModNodeAttachmentRegistry.TryGetAttachedById<NMainMenu, NModChangelogsButton>(__instance, id, out var buttonNode))
            {
                return;
            }

            if (stack.SubmenusOpen)
            {
                buttonNode.SetVisible(false);
                buttonNode.Disable();
            }
            else
            {
                buttonNode.SetVisible(true);
                buttonNode.Enable();
            }
        }
    }
}