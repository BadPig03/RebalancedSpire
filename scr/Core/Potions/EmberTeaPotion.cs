namespace RebalancedSpire.scr.Core.Potions;

using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

[Pool(typeof(SharedPotionPool))]
public sealed class EmberTeaPotion : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Event;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    public override IEnumerable<DynamicVar> CanonicalVars => new ReadOnlyCollection<DynamicVar>(
    [
        new PowerVar<StrengthPower>(5)
    ]);

    public override IEnumerable<IHoverTip> ExtraHoverTips => new ReadOnlyCollection<IHoverTip>(
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ]);

    public override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null)
        {
            return;
        }

        NCombatRoom.Instance?.PlaySplashVfx(target, new Color("fd2155"));
        await PowerCmd.Apply<StrengthPower>(choiceContext, target, DynamicVars.Strength.BaseValue, Owner.Creature, null);
    }
}