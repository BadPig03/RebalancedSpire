namespace RebalancedSpire.scr.Core.Harmony.Monsters.Glory;

using System.Collections.ObjectModel;
using Afflictions;
using Core.Monsters;
using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Encounters;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryGlory)]
// ReSharper disable InconsistentNaming
public static class DoormakerBossPatch
{
    [HarmonyPatch(typeof(EncounterModel), nameof(EncounterModel.HasScene), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_HasScene(EncounterModel __instance, ref bool __result)
    {
        if (__instance is not DoormakerBoss)
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(DoormakerBoss), nameof(DoormakerBoss.ExtraAssetPaths), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_ExtraAssetPaths(DoormakerBoss __instance, ref IEnumerable<string> __result)
    {
        __result = new List<string>
        {
            ModelDb.Affliction<Devoured>().OverlayPath,
            ModelDb.Affliction<Twist>().OverlayPath
        }.AsReadOnly();
        return false;
    }

    [HarmonyPatch(typeof(EncounterModel), nameof(EncounterModel.Slots), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_Slots(EncounterModel __instance, ref IReadOnlyList<string> __result)
    {
        if (__instance is not DoormakerBoss)
        {
            return true;
        }

        __result = new ReadOnlyCollection<string>(
        [
            "doormaker_right", "doormaker_left"
        ]);
        return false;
    }

    [HarmonyPatch(typeof(DoormakerBoss), nameof(DoormakerBoss.GetCameraScaling))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GetCameraScaling(DoormakerBoss __instance, ref float __result)
    {
        __result = 0.75f;
        return false;
    }

    [HarmonyPatch(typeof(DoormakerBoss), nameof(DoormakerBoss.GetCameraOffset))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GetCameraOffset(DoormakerBoss __instance, ref Vector2 __result)
    {
        __result = Vector2.Down * 35f;
        return false;
    }

    [HarmonyPatch(typeof(EncounterModel), nameof(EncounterModel.FullyCenterPlayers), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_FullyCenterPlayers(EncounterModel __instance, ref bool __result)
    {
        if (__instance is not DoormakerBoss)
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(DoormakerBoss), nameof(DoormakerBoss.AllPossibleMonsters), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AllPossibleMonsters(DoormakerBoss __instance, ref IEnumerable<MonsterModel> __result)
    {
        __result = new List<MonsterModel>(
        [
            ModelDb.Monster<DoormakerLeft>().ToMutable(),
            ModelDb.Monster<DoormakerRight>().ToMutable()
        ]);
        return false;
    }

    [HarmonyPatch(typeof(DoormakerBoss), nameof(DoormakerBoss.GenerateMonsters))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMonsters(DoormakerBoss __instance, ref IReadOnlyList<(MonsterModel, string?)> __result)
    {
        __result = new List<(MonsterModel, string?)>(
        [
            (ModelDb.Monster<DoormakerLeft>().ToMutable(), "doormaker_left"),
            (ModelDb.Monster<DoormakerRight>().ToMutable(), "doormaker_right")
        ]).AsReadOnly();
        return false;
    }
}