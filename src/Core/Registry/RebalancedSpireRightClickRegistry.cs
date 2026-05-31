namespace RebalancedSpire.Core.Registry;

using Configs;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib;

internal sealed class RebalancedSpireRightClickRegistry
{
    private static readonly bool Enabled = RebalancedSpireSettingsStore.Settings.SealOfGold;

    internal static void Initialize()
    {
        RitsuLibFramework.RegisterRightClick<SealOfGold>(RebalancedSpireMain.ModId, "REBALANCED_SPIRE_RELIC_SEAL_OF_GOLD", c => Enabled && CombatManager.Instance.IsInProgress && c.Model is SealOfGold { Status: RelicStatus.Active } sealOfGold && c.Player == sealOfGold.Owner,  async e =>
        {
            if (e.Model is not SealOfGold { Status: RelicStatus.Active } sealOfGold || e.Player != sealOfGold.Owner)
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
        });
    }
}