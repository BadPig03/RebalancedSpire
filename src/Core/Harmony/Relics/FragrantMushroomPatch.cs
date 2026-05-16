namespace RebalancedSpire.Core.Harmony.Relics;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class FragrantMushroomPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.HungryForMushroomsConfig;

    [HarmonyPatch(typeof(FragrantMushroom), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(FragrantMushroom __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new HpLossVar(15),
            new CardsVar(3)
        }.AsReadOnly();
        return false;
    }
}