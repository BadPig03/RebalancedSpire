namespace RebalancedSpire.scr.Core.Harmony;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NCompendiumSubmenuPatch
{
    [HarmonyPatch(typeof(NCompendiumSubmenu), nameof(NCompendiumSubmenu._Ready))]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Ready(NCompendiumSubmenu __instance)
    {
        __instance._bestiaryButton.Enable();
    }
}