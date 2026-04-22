namespace RebalancedSpire.scr.Core.Harmony.Monsters.Hive;

using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;

[HarmonyPatch, HarmonyPatchCategory(RebalancedSpireMain.CategoryHive)]
// ReSharper disable InconsistentNaming
public static class KnowledgeDemonPatch
{
    private static int HealAmount => 20;
    private static readonly int[] _disintegrationDamageValues = [4, 6, 8];

    private static readonly Func<KnowledgeDemon, IReadOnlyList<Creature>, Task>? _curseOfKnowledgeDelegate = Helpers.GetDelegate<KnowledgeDemon>("CurseOfKnowledge");
    private static readonly Func<KnowledgeDemon, IReadOnlyList<Creature>, Task>? _slapMoveDelegate = Helpers.GetDelegate<KnowledgeDemon>("SlapMove");
    private static readonly Func<KnowledgeDemon, IReadOnlyList<Creature>, Task>? _knowledgeOverwhelmingMoveDelegate = Helpers.GetDelegate<KnowledgeDemon>("KnowledgeOverwhelmingMove");

    private static async Task CurseOfKnowledge(KnowledgeDemon instance, IReadOnlyList<Creature> targets)
    {
        if (_curseOfKnowledgeDelegate == null)
        {
            return;
        }

        await _curseOfKnowledgeDelegate(instance, targets);
    }

    private static async Task SlapMove(KnowledgeDemon instance, IReadOnlyList<Creature> targets)
    {
        if (_slapMoveDelegate == null)
        {
            return;
        }

        await _slapMoveDelegate(instance, targets);
    }

    private static async Task KnowledgeOverwhelmingMove(KnowledgeDemon instance, IReadOnlyList<Creature> targets)
    {
        if (_knowledgeOverwhelmingMoveDelegate == null)
        {
            return;
        }

        await _knowledgeOverwhelmingMoveDelegate(instance, targets);
    }

    private static async Task PonderMove(KnowledgeDemon instance)
    {
        await CreatureCmd.TriggerAnim(instance.Creature, "HealTrigger", 1.8f);
        NRunMusicController.Instance?.UpdateMusicParameter("knowledge_demon_progress", 1f);
        instance.IsBurnt = false;
        if (instance.Creature.CombatState == null)
        {
            return;
        }

        await CreatureCmd.Heal(instance.Creature, HealAmount * instance.Creature.CombatState.Players.Count);
        await PowerCmd.Apply<StrengthPower>(instance.Creature, instance.PonderStrength, instance.Creature, null);
    }

    private static async Task ChooseCurse(KnowledgeDemon instance, Creature target)
    {
        if (target.IsDead || target.Player == null)
        {
            return;
        }

        var disintegrationDamage = _disintegrationDamageValues[instance.CurseOfKnowledgeCounter];
        var cards = KnowledgeDemon._curseOfKnowledgeSets[instance.CurseOfKnowledgeCounter].Select(delegate(KnowledgeDemon.IChoosable c)
        {
            CardModel card = instance.CombatState.CreateCard((CardModel)c, target.Player);
            if (card is Disintegration)
            {
                card.DynamicVars["DisintegrationPower"].BaseValue = disintegrationDamage;
            }
            return card;
        }).ToList();
        CardModel? cardModel = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), cards, target.Player);
        if (cardModel == null)
        {
            return;
        }

        await ((KnowledgeDemon.IChoosable) cardModel).OnChosen();
    }

    [HarmonyPatch(typeof(KnowledgeDemon), nameof(KnowledgeDemon.ChooseCurse))]
    [HarmonyPrefix]
    [UsedImplicitly]
    public static bool PreFix_ChooseCurse(KnowledgeDemon __instance, Creature target, ref Task __result)
    {
        __result = ChooseCurse(__instance, target);
        return false;
    }

    [HarmonyPatch(typeof(KnowledgeDemon), nameof(KnowledgeDemon.GenerateMoveStateMachine))]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_GenerateMoveStateMachine(KnowledgeDemon __instance, ref MonsterMoveStateMachine __result)
    {
        List<MonsterState> list = [];
        MoveState moveState = new MoveState("CURSE_OF_KNOWLEDGE_MOVE", t => CurseOfKnowledge(__instance, t), new DebuffIntent());
        MoveState moveState2 = new MoveState("SLAP_MOVE", t => SlapMove(__instance, t), new SingleAttackIntent(__instance.SlapDamage));
        MoveState moveState3 = new MoveState("KNOWLEDGE_OVERWHELMING_MOVE", t => KnowledgeOverwhelmingMove(__instance, t), new MultiAttackIntent(__instance.KnowledgeOverwhelmingDamage, 3));
        MoveState moveState4 = new MoveState("PONDER_MOVE", _ => PonderMove(__instance), new HealIntent(), new BuffIntent());
        ConditionalBranchState branchState = new ConditionalBranchState("CurseOfKnowledgeBranch");
        moveState.FollowUpState = moveState2;
        moveState2.FollowUpState = moveState3;
        moveState3.FollowUpState = moveState4;
        moveState4.FollowUpState = branchState;
        branchState.AddState(moveState, () => __instance._curseOfKnowledgeCounter < 3);
        branchState.AddState(moveState2, () => __instance._curseOfKnowledgeCounter >= 3);
        list.Add(branchState);
        list.Add(moveState);
        list.Add(moveState2);
        list.Add(moveState4);
        list.Add(moveState3);
        __result = new MonsterMoveStateMachine(list, moveState);
        return false;
    }
}