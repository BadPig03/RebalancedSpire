namespace RebalancedSpire.Core.Powers;

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPower]
public sealed class GodByrdAwakenedPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: ModelDb.Power<GodByrdPower>().IconPath,
        BigIconPath: ModelDb.Power<GodByrdPower>().ResolvedBigIconPath
    );

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(Owner) || Owner.CombatState == null)
        {
            return;
        }

        Flash();
        var byrdine = Owner.Player?.PlayerCombatState?.GetPet<ByrdineMonster>();
        if (byrdine != null)
        {
            await CreatureCmd.TriggerAnim(byrdine, "Attack", 0f);
            await Cmd.Wait(0.6f);
            SfxCmd.Play("event:/sfx/byrdpip/byrd_2_attack");
            NByrdineAttackVfx.Create(byrdine);
            await Cmd.Wait(0.15f);
        }
        await CreatureCmd.Damage(choiceContext, Owner.CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner);
    }
}
