namespace RebalancedSpire.Core.Harmony.Cards.Ironclad;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BloodWallPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.BloodWallConfig;

    [HarmonyPatch(typeof(BloodWall), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(BloodWall __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new HpLossVar(2),
            new BlockVar(18, ValueProp.Move)
        }.AsReadOnly();
        return false;
    }
}