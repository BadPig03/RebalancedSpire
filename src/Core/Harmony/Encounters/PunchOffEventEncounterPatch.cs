namespace RebalancedSpire.Core.Harmony.Encounters;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PunchOffEventEncounterPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.PunchOff;

    [HarmonyPatch(typeof(PunchOffEventEncounter), "GenerateMonsters")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMonsters(PunchOffEventEncounter __instance, ref IReadOnlyList<(MonsterModel, string?)> __result)
    {
        if (Disabled)
        {
            return true;
        }

        PunchConstruct punchConstruct = (PunchConstruct) ModelDb.Monster<PunchConstruct>().ToMutable();
        punchConstruct.StartsWithFastPunch = true;
        punchConstruct.StartingHpReduction = 1;
        PunchConstruct punchConstruct2 = (PunchConstruct) ModelDb.Monster<PunchConstruct>().ToMutable();
        punchConstruct2.StartingHpReduction = 2;
        __result = new List<(MonsterModel, string?)>(
        [
            (punchConstruct, null),
            (punchConstruct2, null)
        ]).AsReadOnly();
        return false;
    }
}