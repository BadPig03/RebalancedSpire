namespace RebalancedSpire.scr.Core.Harmony.Cards.Event;

using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class ApparitionPatch
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.Portrait), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix(CardModel __instance, ref Texture2D __result)
    {
        if (!RebalancedSpireConfig.ApparitionArtConfig)
        {
            return true;
        }

        if (__instance is not Apparition apparition)
        {
            return true;
        }

        __result = ResourceLoader.Load<Texture2D>(apparition.BetaPortraitPath);
        return false;
    }
}