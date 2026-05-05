namespace RebalancedSpire.scr.Core.Potions;

using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;

[Pool(typeof(SharedPotionPool))]
public sealed class BoneTeaPotion : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Event;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target != null)
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