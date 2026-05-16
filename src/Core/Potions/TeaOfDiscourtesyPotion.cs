namespace RebalancedSpire.Core.Potions;

using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;

[Pool(typeof(EventPotionPool))]
public class TeaOfDiscourtesyPotion : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Event;

    public override PotionUsage Usage => PotionUsage.AnyTime;

    public override TargetType TargetType => !CombatManager.Instance.IsInProgress ? TargetType.TargetedNoCreature : TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>(
    [
        new CardsVar(2),
        new StringVar("Dazed", ModelDb.Card<Dazed>().Title)
    ]).AsReadOnly();

    public override bool PassesCustomUsabilityCheck => CombatManager.Instance.IsInProgress || Owner.RunState.CurrentRoom is MerchantRoom;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (CombatManager.Instance.IsInProgress)
        {
            if (target != null)
            {
                await CardPileCmd.AddToCombatAndPreview<Dazed>(target, PileType.Discard, DynamicVars.Cards.IntValue, Owner, CardPilePosition.Random);
            }
            return;
        }

        if (Owner.RunState.CurrentRoom is not MerchantRoom)
        {
            return;
        }

        var nMerchantRoom = NRun.Instance?.MerchantRoom;
        if (nMerchantRoom != null)
        {
            SfxCmd.Play("event:/sfx/npcs/merchant/merchant_thank_yous");
            LocString? line = Rng.Chaotic.NextItem(nMerchantRoom._dialogue.FoulPotionLines);
            if (line != null && nMerchantRoom.MerchantButton.PlayDialogue(line) != null)
            {
                NGame.Instance?.ScreenRumble(ShakeStrength.Medium, ShakeDuration.Short, RumbleStyle.Rumble);
            }
        }
        foreach (var cardModel in PileType.Deck.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToList().StableShuffle(Owner.RunState.Rng.Niche).Take(1))
        {
            CardCmd.Upgrade(cardModel);
        }
    }
}