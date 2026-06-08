namespace RebalancedSpire.Core.Registry;

using Configs;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interactions.RightClick;

internal sealed class RebalancedSpireRightClickRegistry
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.SealOfGold;

    internal static void Initialize()
    {
        ModRightClickRegistry.Register<SealOfGold>(RebalancedSpireMain.ModId, "REBALANCED_SPIRE_RELIC_SEAL_OF_GOLD", async e =>
        {
            if (Disabled || !CombatManager.Instance.IsInProgress || e.Model is not SealOfGold { Status: RelicStatus.Active } sealOfGold)
            {
                return;
            }

            var gold = sealOfGold.DynamicVars.Gold.IntValue;
            if (sealOfGold.Owner.Gold < gold)
            {
                return;
            }

            sealOfGold.Flash();
            sealOfGold.Status = RelicStatus.Normal;
            await PlayerCmd.GainEnergy(sealOfGold.DynamicVars.Energy.BaseValue, sealOfGold.Owner);
            await PlayerCmd.LoseGold(gold, sealOfGold.Owner);
        }, 0, c => c.Model is SealOfGold { Status: RelicStatus.Active } sealOfGold && c.Player == sealOfGold.Owner);
    }
}