namespace RebalancedSpire.Core.Harmony.Nodes;

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Nodes.Potions;
using Potions;

[HarmonyPatch]
public static class NPotionHolderPatch
{
    [HarmonyTargetMethod]
    [UsedImplicitly]
    private static MethodInfo TargetMethod() => AccessTools.AsyncMoveNext(AccessTools.Method(typeof(NPotionHolder), "TargetNode"));

    [HarmonyTranspiler]
    [UsedImplicitly]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var merchantCheck = codes.Single(code => code.opcode == OpCodes.Isinst && Equals(code.operand, typeof(FoulPotion)));
        merchantCheck.opcode = OpCodes.Call;
        merchantCheck.operand = AccessTools.Method(typeof(NPotionHolderPatch), nameof(TargetsMerchant));
        return codes;
    }

    private static bool TargetsMerchant(PotionModel potion) => potion is FoulPotion or TeaOfDiscourtesyPotion { TargetType: TargetType.TargetedNoCreature };
}
