# 更新日志（CHANGELOG）

本文件遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 规范，版本号遵循 [Semantic Versioning](https://semver.org/lang/zh-CN/)。

## [未发布]

### 变更

- 仓库按 AGENTS 规范整理为十目录结构：`adr/` → `1.ADR/`、`plans/` → `2.PLANS/`、`docs/` → `3.DOCS/`、`src/` → `9.SRC/`；文档按 `001-` 三位编号与 `T/E/R`、`G/M/S` 分类重命名，索引重建，正文引用同步更新。
- 构建中间产物与 Godot 缓存归档到 `TMP/02-缓存与临时/`，一次性脚本迁入 `TOOLS/scripts/`。
- 新增 `4.SDD/001-总体设计.md`、`5.ChangeLog/`、`README.md`、`CHANGELOG.md`、`TMP/TMP.md`、`TOOLS/TOOLS.md`、`10.OUTPUT/output.md`、`PRIVACY/PRIVACY.md`。
- 代码（`.cs` / `.csproj` / `project.godot` / `export_presets.cfg` / `build.ps1` / mod 清单）零改动。
- 本次仓库整理提交：`7d119c3`（版本号不变，仍为 1.0.0）。

## [1.0.1] - 2026-11-13

### 修复

- 打出者判定改为历史条目快照的 `e.Actor`：卡牌打出后易主（如联机中的"球"，The Ball）时，旧实现按 `Card.Owner` 重算会把这张攻击牌改记到另一名玩家头上，导致计数与官方内部计数不一致；新实现与 v0.111/0.113 官方的 `CardPlay.Player` 语义一致。
- 单一 DLL 兼容性不变：`Actor` 在 v0.107 / v0.111 / v0.113 均存在。

### 兼容性

- 针对 v0.113.0 完成接口核对与编译（`NPower` / `MegaLabel` / `power.tscn` / `ModManifest` 均兼容）；运行时待用户实测。
- 角标 1px 观感已核对：不是布局偏移，来源是数字轮廓与 UI 缩放栅格化；布局常量不变。

## [1.0.0] - 2026-08-22

### 新增

- 首个正式版本：在铁甲战士能力牌"杂耍"（Juggling）的 buff 图标右上角显示"本回合已打出攻击牌数"。
- 计数始终显示（含 `0`）、不封顶，回合切换自动归零；先出攻击牌再出杂耍时从战斗历史回溯，显示值正确。
- 4 个 Harmony 补丁（`NPower.RefreshAmount` / `NPower._ExitTree` / `CombatHistory.CardPlayStarted` / `JugglingPower.AfterSideTurnEnd`）+ 注册表管理角标生命周期。
- 模组图标（420×420）与 pck 打包。
- 中英文发布文案、中英文 README、MIT LICENSE。

### 修复

- 角标计数恒滞后一张：刷新点由 `JugglingPower.BeforeCardPlayed`（Before 语义，早于 History 写入）改为 `CombatHistory.CardPlayStarted` Postfix。

### 兼容性

- 支持 v0.107.x（default）与 v0.111.0（public beta），单一 DLL 双版本实测通过；`min_game_version = 0.107.0`。
- 零第三方依赖，与 BaseLib / RitsuLib 同装无冲突。

### 已知问题

- 联机环境下的 UI 同步行为未实测（`affects_gameplay=false`，按本地显示处理）。
- 无自动化测试；回归依赖游戏内清单实测。
- 与盗版、修改版、移动版、老旧版本或冷门/次级 mod 的兼容性不作保证。
