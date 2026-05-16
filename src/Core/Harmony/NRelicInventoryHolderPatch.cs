namespace RebalancedSpire.Core.Harmony;

using GameActions;
using Godot;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class NRelicInventoryHolderPatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.SealOfGoldConfig;

    private static void OnPressed(InputEvent inputEvent, NRelicInventoryHolder instance)
    {
        if (inputEvent is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right })
        {
            return;
        }

        var relicModel = instance._relic.Model;
        var owner = relicModel.Owner;
        if (!CombatManager.Instance.IsInProgress || relicModel is not SealOfGold { Status: RelicStatus.Active } || !LocalContext.IsMe(owner))
        {
            return;
        }

        var index = owner.Relics.FirstIndex(r => r == relicModel);
        if (index < 0)
        {
            return;
        }

        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new UseSealOfGoldAction(owner, index));
    }

    [HarmonyPatch(typeof(NRelicInventoryHolder), "_Ready")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PostFix_Ready(NRelicInventoryHolder __instance)
    {
        if (Disabled)
        {
            return;
        }

        __instance._relic.GuiInput += e => OnPressed(e, __instance);
    }
}