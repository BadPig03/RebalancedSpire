namespace RebalancedSpire.scr.Core.Harmony.Ancients;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryVakuu)]
// ReSharper disable InconsistentNaming
public static class VakuuPatch
{
    [HarmonyPatch(typeof(Vakuu), nameof(Vakuu.Pool1), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Pool1(Vakuu __instance, ref IEnumerable<EventOption> __result)
    {
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
        __result = new List<EventOption>
        {
            Helpers.RelicOption<ChoicesParadox>(__instance),
            Helpers.RelicOption<MusicBox>(__instance),
            Helpers.RelicOption<JeweledMask>(__instance)
        };
        return false;
    }
}