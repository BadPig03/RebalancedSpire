namespace RebalancedSpire.Core.Harmony.Blitz;

using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Blitz;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class BlitzCombatRewardsPatch
{
    [HarmonyPatch(typeof(BlitzRunModel), "TryReplaceDefaultRoomRewards")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_TryReplaceDefaultRoomRewards(Player player, List<Reward> rewards, AbstractRoom room, ref bool __result)
    {
        if (player.RunState.GameMode != GameMode.Blitz || room is not CombatRoom combat)
        {
            return true;
        }

        var settings = RebalancedSpireRuntimeSettings.Current;
        var rarity = combat.RoomType switch
        {
            RoomType.Monster when combat.Encounter.IsWeak && settings.BlitzWeakRewards => CardRarity.Common,
            RoomType.Monster when !combat.Encounter.IsWeak && settings.BlitzNormalRewards => CardRarity.Uncommon,
            RoomType.Elite when settings.BlitzEliteRewards => CardRarity.Rare,
            _ => (CardRarity?)null
        };
        if (rarity is not { } cardRarity)
        {
            return true;
        }

        rewards.Add(new CardReward(BlitzRunModel.GetCardRewardOptions(player, cardRarity), 3, player));
        switch (combat.RoomType)
        {
            case RoomType.Monster when combat.Encounter.IsWeak:
                rewards.Add(new PotionReward(player));
                break;
            case RoomType.Elite:
                rewards.Add(new RelicReward(player));
                rewards.Add(new RelicReward(player));
                break;
            case RoomType.Unassigned:
            case RoomType.Boss:
            case RoomType.Treasure:
            case RoomType.Shop:
            case RoomType.Event:
            case RoomType.RestSite:
            case RoomType.Map:
            default:
                break;
        }

        __result = true;
        return false;
    }
}
