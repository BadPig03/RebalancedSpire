namespace RebalancedSpire.Core.Harmony.Enchantments;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public class InkyPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.BladeOfInk;

    [HarmonyPatch(typeof(Inky), "CanonicalVars", MethodType.Getter)]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CanonicalVars(Inky __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = new List<DynamicVar>
        {
            new DamageVar(2, ValueProp.Move),
            new PowerVar<WeakPower>(1)
        }.AsReadOnly();
        return false;
    }
}