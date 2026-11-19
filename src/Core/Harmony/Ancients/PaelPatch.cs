namespace RebalancedSpire.Core.Harmony.Ancients;

using System.Reflection.Emit;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Events;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class PaelPatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.PaelsLegion;

    [HarmonyPatch(typeof(Pael), "GenerateInitialOptions")]
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var petCheck = codes.Single(code => code.Calls(AccessTools.Method(typeof(Player), nameof(Player.HasEventPet))));
        petCheck.opcode = OpCodes.Call;
        petCheck.operand = AccessTools.Method(typeof(PaelPatch), nameof(HasBlockingEventPet));
        return codes;
    }

    private static bool HasBlockingEventPet(Player player) => Disabled && player.HasEventPet();
}
