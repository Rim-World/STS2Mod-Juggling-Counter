using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace JugglingCounterMod;

/// <summary>
/// 杂耍（Juggling）buff 的"本回合已打出攻击牌数"统计。
/// 数据源：CombatManager.History.CardPlaysStarted 实时统计，
/// 谓词与游戏内置 JugglingPower.AfterApplied 的回溯初始化一致
/// （攻击牌 + 打出者 == buff 拥有者 + HappenedThisTurn），因此显示值等于游戏语义下的本回合攻击牌数，
/// 回合边界（round/side/玩家 turn 变化）自动归零，无需自维护状态。
///
/// 打出者判定用历史条目自带的 e.Actor，而不是 e.CardPlay.Card.Owner：
/// CardPlayStartedEntry 在写入历史时把 cardPlay.Card.Owner.Creature 快照进 Actor，
/// 之后卡牌易主不会改变它。这一点在 v0.111+ 很重要——"球"（The Ball）等牌打出后
/// 会被移交给另一名玩家（Card.Owner 改变），若每次刷新都用 Card.Owner 重算，
/// 这张攻击牌会被改记到另一名玩家头上，与官方计数（v0.111/113 的 CardPlay.Player）不一致。
/// v0.107 的 CardPlay 没有 Player 属性，而 Actor 在 v0.107/v0.111/v0.113 均存在，
/// 因此单一 DLL 可跨版本通用。
/// </summary>
public static class JugglingCounter
{
    public static int CountAttacksPlayedThisTurn(JugglingPower power)
    {
        if (power == null || power.Owner?.Player == null)
        {
            return 0;
        }
        CombatManager? combat = CombatManager.Instance;
        if (combat?.History == null)
        {
            return 0;
        }
        return combat.History.CardPlaysStarted.Count(e =>
            e.CardPlay.Card.Type == CardType.Attack &&
            e.Actor == power.Owner &&
            e.HappenedThisTurn(power.CombatState));
    }
}
