namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive.Boss;

using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class TheInsatiablePatch
{
    private static readonly bool Disabled = !RebalancedSpireConfig.TheInsatiableConfig;

    private static int SandpitPowerAmount => 4;
    private static int LongDistancePowerAmount => 5;

    private static int FranticEscapeAmount => 6;


    private static async Task LiquifyMove(TheInsatiable instance, IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_insatiable/the_insatiable_liquify_ground");
        await CreatureCmd.TriggerAnim(instance.Creature, "LiquifySand", 0f);
        await Cmd.Wait(0.5f);
        VfxCmd.PlayOnCreatureCenter(instance.Creature, "vfx/vfx_scream");
        await Cmd.Wait(0.75f);
        foreach (Creature target in targets)
        {
            SandpitPower sandpitPower = (SandpitPower) ModelDb.Power<SandpitPower>().ToMutable();
            sandpitPower.Target = target;
            await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), sandpitPower, instance.Creature, SandpitPowerAmount, instance.Creature, null);
            await PowerCmd.Apply<LongDistancePower>(new ThrowingPlayerChoiceContext(), target, LongDistancePowerAmount, instance.Creature, null);
        }

        var statusCards = new List<CardPileAddResult>();
        foreach (Creature target in targets)
        {
            Player? player = target.Player ?? target.PetOwner;
            if (player == null)
            {
                continue;
            }

            for (var i = 0; i < FranticEscapeAmount; i++)
            {
                CardModel card = instance.CombatState.CreateCard<FranticEscape>(player);
                PileType newPileType = i < FranticEscapeAmount / 2 ? PileType.Draw : PileType.Discard;
                statusCards.Add(await CardPileCmd.AddGeneratedCardToCombat(card, newPileType, null, CardPilePosition.Random));
            }

            if (!LocalContext.IsMe(player))
            {
                continue;
            }

            CardCmd.PreviewCardPileAdd(statusCards);
            await Cmd.Wait(1f);
        }
        instance.HasLiquified = true;
    }

    [HarmonyPatch(typeof(TheInsatiable), nameof(TheInsatiable.LiquifyMove))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_LiquifyMove(TheInsatiable __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = LiquifyMove(__instance, targets);
        return false;
    }
}