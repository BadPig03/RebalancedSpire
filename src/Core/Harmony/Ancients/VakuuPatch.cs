namespace RebalancedSpire.Core.Harmony.Ancients;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class VakuuPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.VakuuAncientChoices;

    [HarmonyPatch(typeof(EventModel), "BackgroundScenePath", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BackgroundScenePath(EventModel __instance, ref string __result)
    {
        if (__instance is not Vakuu)
        {
            return true;
        }

        if (!RebalancedSpireSettingsStore.Settings.VakuuFixedArt)
        {
            return true;
        }

        __result = SceneHelper.GetScenePath("events/background_scenes/vakuu_fixed_art");
        return false;
    }

    [HarmonyPatch(typeof(Vakuu), "Pool1", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Pool1(Vakuu __instance, ref IEnumerable<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            SrcHelpers.RelicOption<BloodSoakedRose>(__instance),
            SrcHelpers.RelicOption<LordsParasol>(__instance),
            SrcHelpers.RelicOption<WhisperingEarring>(__instance)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Vakuu), "Pool2", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Pool2(Vakuu __instance, ref IEnumerable<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            SrcHelpers.RelicOption<Fiddle>(__instance),
            SrcHelpers.RelicOption<PreservedFog>(__instance),
            SrcHelpers.RelicOption<SereTalon>(__instance),
            SrcHelpers.RelicOption<DistinguishedCape>(__instance).ThatDecreasesMaxHp(9)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Vakuu), "Pool3", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Pool3(Vakuu __instance, ref IEnumerable<EventOption> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<EventOption>
        {
            SrcHelpers.RelicOption<ChoicesParadox>(__instance),
            SrcHelpers.RelicOption<MusicBox>(__instance),
            SrcHelpers.RelicOption<JeweledMask>(__instance)
        }.AsReadOnly();
        return false;
    }
}