namespace RebalancedSpire.scr.Core.Harmony.Ancients;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryOrobas)]
// ReSharper disable InconsistentNaming
public static class OrobasPatch
{
    private static List<EventOption> OptionPool1(Orobas instance) =>
    [
        Helpers.RelicOption<Driftwood>(instance),
        Helpers.RelicOption<GlassEye>(instance),
        Helpers.RelicOption<SandCastle>(instance)
    ];

    private static List<EventOption> OptionPool2(Orobas instance) =>
    [
        Helpers.RelicOption<AlchemicalCoffer>(instance),
        Helpers.RelicOption<ElectricShrymp>(instance),
        Helpers.RelicOption<RadiantPearl>(instance)
    ];

    private static List<EventOption> OptionPool3(Orobas instance)
    {
        List<EventOption> list = [];
        TouchOfOrobas touchOfOrobas = (TouchOfOrobas) ModelDb.Relic<TouchOfOrobas>().ToMutable();
        if (instance.Owner == null || touchOfOrobas.SetupForPlayer(instance.Owner))
        {
            list.Add(Helpers.RelicOption(instance, touchOfOrobas));
        }

        ArchaicTooth archaicTooth = (ArchaicTooth)ModelDb.Relic<ArchaicTooth>().ToMutable();
        if (instance.Owner == null || archaicTooth.SetupForPlayer(instance.Owner))
        {
            list.Add(Helpers.RelicOption(instance, archaicTooth));
        }
        if (list.Count == 0)
        {
            list.Add(new EventOption(instance, null, "OROBAS.pages.INITIAL.options.OPTION_POOL_3_LOCKED"));
        }
        return list;
    }

    private static List<EventOption> SeaGlasses(Orobas instance)
    {
        List<EventOption> list = [];
        foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
        {
            SeaGlass seaGlass = (SeaGlass) ModelDb.Relic<SeaGlass>().ToMutable();
            seaGlass.CharacterId = allCharacter.Id;
            list.Add(Helpers.RelicOption(instance, seaGlass));
        }
        return list;
    }

    private static EventOption PrismaticGem(Orobas instance) => Helpers.RelicOption<PrismaticGem>(instance);

    [HarmonyPatch(typeof(Orobas), nameof(Orobas.AllPossibleOptions), MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_AllPossibleOptions(Orobas __instance, ref IEnumerable<EventOption> __result)
    {
        List<EventOption> options = [];
        options.AddRange(OptionPool1(__instance));
        options.AddRange(OptionPool2(__instance));
        options.AddRange(OptionPool3(__instance));
        options.AddRange(SeaGlasses(__instance));
        options.Add(PrismaticGem(__instance));
        __result = options;
        return false;
    }

    [HarmonyPatch(typeof(Orobas), nameof(Orobas.GenerateInitialOptions))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateInitialOptions(Orobas __instance, ref IReadOnlyList<EventOption> __result)
    {
        Player? player = __instance.Owner;
        if (player == null)
        {
            return true;
        }

        CharacterModel character = player.Character;
        CharacterModel characterModel = __instance.Rng.NextItem(player.UnlockState.Characters.Where(c => c.Id != character.Id)) ?? character;
        var list = OptionPool1(__instance);
        if (__instance.Rng.NextBool())
        {
            list.Add(PrismaticGem(__instance));
        }
        else
        {
            SeaGlass seaGlass = (SeaGlass) ModelDb.Relic<SeaGlass>().ToMutable();
            seaGlass.CharacterId = characterModel.Id;
            list.Add(Helpers.RelicOption(__instance, seaGlass));
        }

        var results = new List<EventOption>();
        results.AddRange(list.UnstableShuffle(__instance.Rng).Take(1));
        results.AddRange(OptionPool2(__instance).UnstableShuffle(__instance.Rng).Take(1));
        results.AddRange(OptionPool3(__instance).UnstableShuffle(__instance.Rng).Take(1));
        __result = results.AsReadOnly();
        return false;
    }
}