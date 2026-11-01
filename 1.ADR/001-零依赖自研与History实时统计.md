# ADR-001：零依赖自研 + History 实时统计 + NPower 角标补丁

- 状态：已接受（2026-08-22，用户确认"零依赖自研"及显示行为决策）
- 关联计划：[2.PLANS/001-G-杂耍计数实现方案.md](../2.PLANS/001-G-杂耍计数实现方案.md)

## 背景

为铁甲战士"杂耍"（Juggling）buff 图标右上角添加"本回合已打出攻击牌数"显示。调研发现（见 [3.DOCS](../3.DOCS/DOCS.md) 三份调研文档）：

- 游戏侧：计数是 `JugglingPower` 每实例 private 内部数据（`Data.attacksPlayedThisTurn`），但可用 `CombatManager.History.CardPlaysStarted` + `HappenedThisTurn` 公共 API 实时重算（与 `AfterApplied` 回溯谓词一致）。
- BaseLib `TwoAmountPowers`（`IHasSecondAmount`）与 RitsuLib `ExtraCornerAmountLabels`（`IPowerExtraIconAmountLabelsProvider`）都是"接口绑定"能力，要求 PowerModel 实现接口；`JugglingPower` 为游戏内置 **sealed** 类，两者均不可直接使用（RitsuLib Capabilities 亦无 PowerModel 宿主）。

## 决策

1. **零第三方依赖**：不依赖 BaseLib / RitsuLib，纯 Harmony 自研（4 个补丁 + 注册表 + 统计函数，约 150 行）。借鉴两者的 MIT 开源设计：Duplicate `%AmountLabel` 创建第二标签、右上角布局常量（(-56,0)-(44,23)）、注册表按节点实例 id 跟踪 + `_ExitTree` 清理、样式复制。
2. **数据源用 History 实时统计**（非反射读 internal data、非自维护计数器）：语义与游戏一致、回合边界自动归零、无版本脆弱性。
3. **UI 注入点**：`NPower.RefreshAmount` Postfix（创建/更新 + 注册）、`NPower._ExitTree` Postfix（注销）、`CombatHistory.CardPlayStarted` Postfix（攻击牌写入 History 后全体刷新；修正记录：最初选用 `JugglingPower.BeforeCardPlayed`，因 Before 钩子早于 History 记录导致计数滞后一张，见 plans §11）、`JugglingPower.AfterSideTurnEnd` Postfix（己方回合结束显式置 0，与游戏内部清零同点同步）。
4. **显示行为**（用户确认）：始终显示数字含 0；真实计数不封顶；图标右上角。
5. **manifest**：`has_pck=false`（纯 DLL）、`affects_gameplay=false`（纯显示）。

## 被否方案与理由

| 方案 | 理由 |
|---|---|
| 依赖 BaseLib 复用 TwoAmountPowers | JugglingPower sealed，无法实现 `IHasSecondAmount`；无其他相关收益 |
| 依赖 RitsuLib 复用 ExtraCornerAmountLabels | 同上（接口绑定）；Capabilities/NodeAttachments 不覆盖 PowerModel/NPower |
| 反射读 `JugglingPower` internal data | private 嵌套类字段属实现细节，版本升级脆弱；与 History 重算无显示差异 |
| 自维护攻击牌计数器 | 与游戏逻辑存在分叉风险（回放/联机/回合边界），无必要 |
| mod pck 覆盖 `scenes/combat/power.tscn` | 全局改所有 buff 图标场景，破坏性大、升级即碎 |

## 影响

- 玩家安装只需 mod 本体（dll+json），无需任何库。
- 升级回归面小（4 个 patch 目标 + 1 个场景布局常量）。
- 与 BaseLib/RitsuLib 同装无冲突（各自按类型分支工作）。

## 未来触发条件

- 若后续扩展为新增自定义内容（卡牌/遗物等）→ 重新评估引入 BaseLib。
- 若需要 mod 设置页/持久化/补丁诊断框架 → 重新评估引入 RitsuLib。
