namespace RebalancedSpire.scr.Core.Harmony.Ancients;

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
    private static readonly bool Disabled = !RebalancedSpireConfig.VakuuChoicesConfig;

    [HarmonyPatch(typeof(EventModel), nameof(EventModel.BackgroundScenePath), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_BackgroundScenePath(EventModel __instance, ref string __result)
    {
        if (__instance is not Vakuu)
        {
            return true;
        }

        if (RebalancedSpireConfig.VakuuFixedArtConfig)
        {
            __result = SceneHelper.GetScenePath("events/background_scenes/vakuu_fixed_art");
            return false;
        }

        if (!RebalancedSpireConfig.VakuuBetaArtConfig)
        {
            return true;
        }

        __result = SceneHelper.GetScenePath("events/background_scenes/vakuu_beta_art");
        return false;
    }

    [HarmonyPatch(typeof(Vakuu), nameof(Vakuu.Pool1), MethodType.Getter)]
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
            RelicHelpers.RelicOption<BloodSoakedRose>(__instance),
            RelicHelpers.RelicOption<LordsParasol>(__instance),
            RelicHelpers.RelicOption<WhisperingEarring>(__instance)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Vakuu), nameof(Vakuu.Pool2), MethodType.Getter)]
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
            RelicHelpers.RelicOption<Fiddle>(__instance),
            RelicHelpers.RelicOption<PreservedFog>(__instance),
            RelicHelpers.RelicOption<SereTalon>(__instance),
            RelicHelpers.RelicOption<DistinguishedCape>(__instance).ThatDecreasesMaxHp(9)
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(Vakuu), nameof(Vakuu.Pool3), MethodType.Getter)]
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
            RelicHelpers.RelicOption<ChoicesParadox>(__instance),
            RelicHelpers.RelicOption<MusicBox>(__instance),
            RelicHelpers.RelicOption<JeweledMask>(__instance)
        }.AsReadOnly();
        return false;
    }
}