namespace RebalancedSpire.Core.Harmony.Cards.Silent;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BladeOfInkPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.BladeOfInk;

    [HarmonyPatch(typeof(BladeOfInk), "ExtraHoverTips", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraHoverTips(BladeOfInk __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (Disabled)
        {
            return true;
        }

        var shiv = (Shiv) ModelDb.Card<Shiv>().MutableClone();
        CardCmd.Enchant<Inky>(shiv, 1);
        var list = new List<IHoverTip>
        {
            HoverTipFactory.FromCard(shiv)
        };
        list.AddRange(HoverTipFactory.FromEnchantment<Inky>());
        __result = list.AsReadOnly();
        return false;
    }
}