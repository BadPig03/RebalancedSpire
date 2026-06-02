namespace RebalancedSpire.Core.Harmony.Monsters.Glory.Boss;

using Afflictions;
using Configs;
using Core.Powers;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.ValueProps;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class AeonglassPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.Aeonglass;

    private const int WithersInitUpgradeLevel = 2;
    private const int WithersMinCount = 2;
    private const int WithersMaxCount = 4;
    private const int WitheringPresencePowerAmount = 12;
    private const int ArtifactPowerAmount = 3;

    private static async Task AfterAddedToRoom(Aeonglass instance)
    {
        NRunMusicController.Instance?.UpdateMusicParameter("queen_progress", 1f);
        var players = instance.Creature.CombatState?.PlayerCreatures.ToList();
        if (players == null)
        {
            return;
        }

        foreach (var creature in players)
        {
            WitheringPresencePlusPower power = (WitheringPresencePlusPower) ModelDb.Power<WitheringPresencePlusPower>().ToMutable();
            power.Target = creature;
            await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), power, instance.Creature, WitheringPresencePowerAmount, instance.Creature, null);
        }
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), instance.Creature, ArtifactPowerAmount, instance.Creature, null);
    }

    private static async Task WitheringMove(Aeonglass instance)
    {
        var players = instance.Creature.CombatState?.Players.ToList();
        if (players == null)
        {
            return;
        }

        foreach (var player in players)
        {
            var statusCards = new List<CardPileAddResult>();
            for (var i = 0; i < WithersMaxCount; i++)
            {
                var wither = instance.CombatState.CreateCard<Wither>(player);
                for (var j = 0; j < WithersInitUpgradeLevel; j++)
                {
                    wither.FakeUpgrade();
                }
                await CardCmd.Afflict<Withering>(wither, 1);
                PileType newPileType = i < WithersMinCount ? PileType.Draw : PileType.Discard;
                statusCards.Add(await CardPileCmd.AddGeneratedCardToCombat(wither, newPileType, null, CardPilePosition.Random));
            }
            if (!LocalContext.IsMe(player))
            {
                continue;
            }

            CardCmd.PreviewCardPileAdd(statusCards);
            await Cmd.Wait(1.2f);
        }
    }

    private static async Task EbbMove(Aeonglass instance)
    {
        await DamageCmd.Attack(instance.EbbDamage).FromMonster(instance).WithAttackerAnim("Attack", 0.15f).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private static async Task IncreasingIntensityMove(Aeonglass instance)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), instance.Creature, instance.IncreasingIntensityTotalStrength, instance.Creature, null);
        await CreatureCmd.GainBlock(instance.Creature, instance.IncreasingIntensityBlock, ValueProp.Move, null);
    }

    [HarmonyPatch(typeof(Aeonglass), "AfterAddedToRoom")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix_AfterAddedToRoom(Aeonglass __instance, ref Task __result)
    {
        if (Disabled)
        {
            return true;
        }

        __result = AfterAddedToRoom(__instance);
        return false;
    }

    [HarmonyPatch(typeof(Aeonglass), "GenerateMoveStateMachine")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(Aeonglass __instance, ref MonsterMoveStateMachine __result)
    {
        if (Disabled)
        {
            return true;
        }

        List<MonsterState> list = [];
        MoveState moveState = new MoveState("WITHERING_MOVE", _ => WitheringMove(__instance), new StatusIntent(WithersMaxCount));
        MoveState moveState2 = new MoveState("EBB_MOVE", _ => EbbMove(__instance), new SingleAttackIntent(__instance.EbbDamage));
        MoveState moveState3 = new MoveState("EYE_LASERS_MOVE", __instance.EyeLasersMove, new MultiAttackIntent(__instance.EyeLasersDamage, __instance.EyeLasersRepeat));
        MoveState moveState4 = new MoveState("INCREASING_INTENSITY_MOVE", _ => IncreasingIntensityMove(__instance), new BuffIntent(), new DefendIntent());
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = moveState2;
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState3);
        list.Add(moveState4);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}