namespace RebalancedSpire.Core.Potions;

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterPotion(typeof(EventPotionPool))]
public sealed class BoneTeaPotion : ModPotionTemplate
{
    public override PotionRarity Rarity => PotionRarity.Event;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null)
        {
            return Task.CompletedTask;
        }

        NCombatRoom.Instance?.PlaySplashVfx(Owner.Creature, new Color("cd683d"));
        foreach (var card in PileType.Draw.GetPile(Owner).Cards.Where(c => c.IsUpgradable))
        {
            CardCmd.Upgrade(card);
        }
        return Task.CompletedTask;
    }
}