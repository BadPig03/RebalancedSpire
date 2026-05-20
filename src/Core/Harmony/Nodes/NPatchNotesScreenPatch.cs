namespace RebalancedSpire.Core.Harmony.Nodes;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

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

        var button = NMainMenuPatch.ButtonNode.Get(mainMenu);
        if (button == null)
        {
            return;
        }

        button.SetVisible(true);
        button.Enable();
    }
}