namespace RebalancedSpire.Core.Harmony.Nodes;

using BaseLib.Utils;
using Core.Nodes.Screens.MainMenu;
using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using NModChangelogsButton = Core.Nodes.Screens.MainMenu.NModChangelogsButton;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NMainMenuPatch
{
    public static readonly AddedNode<NMainMenu, NModChangelogsButton> ButtonNode = new(_ => NModChangelogsButton.Create());
    private static readonly AddedNode<NMainMenu, NModChangelogsScreen> ScreenNode = new(_ => NModChangelogsScreen.Create());

    [HarmonyPatch(typeof(NMainMenu), "_Ready")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Ready(NMainMenu __instance)
    {
        var screenNode = ScreenNode.Get(__instance);
        screenNode?.SetVisible(false);
        __instance.AddChild(screenNode);

        var buttonNode = ButtonNode.Get(__instance);
        buttonNode?.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => screenNode?.Open()));
        __instance.AddChild(buttonNode);

        var patchNotesNode = __instance._patchNotesButtonNode;
        patchNotesNode.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => OnPatchNotesChanged()));

        var submenuNode = __instance.SubmenuStack;
        submenuNode.Connect(NSubmenuStack.SignalName.StackModified, Callable.From(() => OnSubmenuStackChanged(submenuNode)));
        OnSubmenuStackChanged(submenuNode);
        return;

        void OnPatchNotesChanged()
        {
            if (!__instance.PatchNotesScreen.IsVisible())
            {
                return;
            }

            buttonNode?.SetVisible(false);
            buttonNode?.Disable();
        }

        void OnSubmenuStackChanged(NMainMenuSubmenuStack stack)
        {
            if (stack.SubmenusOpen)
            {
                buttonNode?.SetVisible(false);
                buttonNode?.Disable();
            }
            else
            {
                buttonNode?.SetVisible(true);
                buttonNode?.Enable();
            }
        }
    }
}