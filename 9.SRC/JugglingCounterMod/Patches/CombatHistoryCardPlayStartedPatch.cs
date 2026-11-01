using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;

namespace JugglingCounterMod.Patches;

/// <summary>
/// CombatHistory.CardPlayStarted Postfix：每当一张卡牌开始打出并写入战斗历史后，刷新所有杂耍角标。
/// 选择此点的原因（v0.111.0 CardModel 打出流程 1926-1929 行实测）：
///   await Hook.BeforeCardPlayed(...)            ← Before 钩子：此时当前牌尚未写入 History
///   History.CardPlayStarted(combatState, cardPlay)  ← 记录点：Postfix 时 History 已含当前牌
/// 原先挂在 JugglingPower.BeforeCardPlayed 上会导致计数恒滞后一张（当前攻击牌未被计入）。
/// 本方法是卡牌打出记录的唯一入口（CardModel.cs 唯一调用点），自动打出/联机各端同样经过。
/// </summary>
[HarmonyPatch(typeof(CombatHistory), nameof(CombatHistory.CardPlayStarted))]
public static class CombatHistoryCardPlayStartedPatch
{
    public static void Postfix(CardPlay cardPlay)
    {
        try
        {
            if (cardPlay?.Card?.Type != CardType.Attack)
            {
                return;
            }
            JugglingCounterRegistry.RefreshAll();
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: CombatHistory.CardPlayStarted postfix failed: {e}");
        }
    }
}
