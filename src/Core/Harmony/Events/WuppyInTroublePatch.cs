namespace RebalancedSpire.Core.Harmony.Events;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class WuppyInTroublePatch
{
    private static bool Disabled => !RebalancedSpireRuntimeSettings.Current.WuppyInTrouble;

    [HarmonyPatch(typeof(WuppyInTrouble), "IsAllowed")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_IsAllowed(IRunState runState, ref bool __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = runState.Players.All(p => p.Creature.CurrentHp > WuppyInTrouble.GetHpLoss(p));
        return false;
    }
}
