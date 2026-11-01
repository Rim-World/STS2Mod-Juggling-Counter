# 架构决策记录索引（ADR.md）

STS2Mod-杂耍计数 —— 为铁甲战士能力牌"杂耍"（Juggling）的 buff 图标右上角添加"本回合已打出攻击牌数"计数。

| 编号 | 日期 | 决策 | 说明 | 链接 |
|---|---|---|---|---|
| 001 | 2026-08-22 | 零依赖自研 + History 实时统计 + NPower 角标补丁 | 纯 Harmony，不依赖 BaseLib / RitsuLib；数据源取 `CombatHistory.CardPlaysStarted` 实时重算；补丁点 `NPower.RefreshAmount` / `NPower._ExitTree` / `CombatHistory.CardPlayStarted` / `JugglingPower.AfterSideTurnEnd`；`affects_gameplay=false` | [001-零依赖自研与History实时统计.md](001-零依赖自研与History实时统计.md) |
| 002 | 2026-08-22 | 构建与打包方案（v1.0.0） | manifest 参数（id / author / version 1.0.0）；`has_pck=true`（模组图标需 `res://{id}/mod_image.png`，修正 001 的 `has_pck=false` 预案）；420×420 正式图标；中间文件与工程目录分离（dotnet 输出重定向 + Godot 缓存归位）；`build.ps1` 直接安装到游戏 `mods/` | [002-构建与打包.md](002-构建与打包.md) |
| 003 | 2026-08-22 | 双版本兼容策略（v0.107 / v0.111 单一 DLL） | `min_game_version=0.107.0`；统计谓词统一为 `Card.Owner.Creature` 链（107/111 通用）；以当前分支 dll 编译，单一 DLL 双版本 | [003-双版本兼容策略.md](003-双版本兼容策略.md) |

## 维护说明

技术栈与架构变化、依赖库引入与删除、大文件增删、技术路线改换、需求重大变化、大版本迭代、功能放弃等重大决策记录在此；小功能增删与调试问题写入 `2.PLANS` / `3.DOCS`，不入 ADR。

- 命名：`001-中文标题.md`，三位递增编号。
- 每条记录在正文头部注明状态、日期、关联计划/文档。
- 新增记录后必须更新上表。
