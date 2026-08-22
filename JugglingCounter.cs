using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace JugglingCounterMod;

/// <summary>
/// 杂耍（Juggling）buff 的"本回合已打出攻击牌数"统计。
/// 数据源：CombatManager.History.CardPlaysStarted 实时统计，
/// 谓词与游戏内置 JugglingPower.AfterApplied 的回溯初始化一致
/// （攻击牌 + 本玩家 + HappenedThisTurn），因此显示值永远等于游戏语义下的本回合攻击牌数，
/// 回合边界（round/side/玩家 turn 变化）自动归零，无需自维护状态。
/// 谓词统一采用 Card.Owner.Creature 链（v0.107 官方写法）：v0.107 的 CardPlay 无 Player 属性，
/// 而该成员链在 v0.107/v0.111 均存在，单一 DLL 双版本通用；语义与 v0.111 的 CardPlay.Player
/// 写法等价（打出者 == buff 拥有者）。
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
            e.CardPlay.Card.Owner.Creature == power.Owner &&
            e.HappenedThisTurn(power.CombatState));
    }
}
