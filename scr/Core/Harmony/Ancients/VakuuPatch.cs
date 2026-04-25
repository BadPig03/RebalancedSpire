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
        if (!RebalancedSpireConfig.VakuuArtConfig)
        {
            return true;
        }

        if (__instance is not Vakuu)
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
            Helpers.RelicOption<BloodSoakedRose>(__instance),
            Helpers.RelicOption<LordsParasol>(__instance),
            Helpers.RelicOption<WhisperingEarring>(__instance)
        };
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
            Helpers.RelicOption<Fiddle>(__instance),
            Helpers.RelicOption<PreservedFog>(__instance),
            Helpers.RelicOption<SereTalon>(__instance),
            Helpers.RelicOption<DistinguishedCape>(__instance).ThatDecreasesMaxHp(9)
        };
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
            Helpers.RelicOption<ChoicesParadox>(__instance),
            Helpers.RelicOption<MusicBox>(__instance),
            Helpers.RelicOption<JeweledMask>(__instance)
        };
        return false;
    }
}