# 施工计划索引（PLANS.md）

| 编号 | 计划 | 层级 | 状态 | 时间 | 说明 |
|---|---|---|---|---|---|
| 001 | [001-G-杂耍计数实现方案.md](001-G-杂耍计数实现方案.md) | G（整体） | ✅ 已完成 | 2026-08-22 | 杂耍 buff 右上角"本回合攻击牌计数"完整实现方案：需求、架构（4 补丁 + 注册表 + History 统计）、核心设计、工程结构、实施步骤、验证清单、风险与回归；含 §11 v1.0.1 bug 修复记录与 §12 v0.107 兼容计划 |
| 002 | [002-S-0.113兼容修正.md](002-S-0.113兼容修正.md) | S（小修） | ✅ 已完成 | 2026-11-13 | v0.113.0 兼容核对：补丁点/场景/manifest 全通过；统计谓词改为 `CardPlayStartedEntry.Actor`（修复卡牌易主串账）；角标 1px 确认为字体轮廓与缩放栅格化，不改布局；三路只读对抗审查 0 必须追加改动（`3.DOCS/006-R`） |

## 施工记录回填

| 时间 | 计划 | 实际施工内容 | 验收状态 |
|---|---|---|---|
| 2026-08-22 | 001-G | 建立 `JugglingCounterMod` 工程（csproj / manifest / project.godot / export_presets.cfg / build.ps1 / ModEntry / JugglingCounter / JugglingCounterRegistry / Patches×4 / mod_image.png），`build.ps1` 一次构建通过并安装到游戏 `mods/JugglingCounterMod/`；v1.0.0 提交入仓 | ✅ 通过 |
| 2026-08-22 | 001-G §11 | 修复"计数恒滞后一张"：刷新点由 `JugglingPower.BeforeCardPlayed` Postfix 改为 `CombatHistory.CardPlayStarted` Postfix（删除 `Patches/JugglingBeforeCardPlayedPatch.cs`，新增 `Patches/CombatHistoryCardPlayStartedPatch.cs`） | ✅ 通过（场景 A/B 实测） |
| 2026-08-22 | 001-G §12 | v0.107 兼容：统计谓词改为 `Card.Owner.Creature` 链、`min_game_version` 改为 `0.107.0`、以 107 dll 重建并覆盖安装 | ✅ 通过（107 与 111 双版本实测） |
| 2026-11-01 | —（仓库整理） | 按 AGENTS 规范整理为十目录结构：`adr/`→`1.ADR/`、`plans/`→`2.PLANS/`、`docs/`→`3.DOCS/`、`src/`→`9.SRC/`，中间文件与工具脚本归档，补全 SDD / ChangeLog / README / CHANGELOG / TMP / TOOLS 等文档；代码零改动 | ✅ 通过（blob 保全校验 + 推送成功） |
| 2026-11-13 | 002-S | v0.113.0 兼容修正：`JugglingCounter` 谓词改为 `e.Actor == power.Owner`（卡牌易主不串账），manifest 1.0.1，以 113 程序集编译通过；角标 1px 核对为字体轮廓现象，未改布局 | ✅ 编译与静态核对通过，运行时待实测 |
| 2026-11-13 | 002-S | 三路只读对抗性审查（谓词 / 刷新生命周期 / 113 假设），复核 107/111/113 源码与安装产物 IL；0 必须追加的代码/补丁/UI 改动；P3 边界见 `3.DOCS/006-R` | ✅ 审查通过 |

开工前检查本索引；方案变更先改计划文档再施工；施工完成后回填上表。
