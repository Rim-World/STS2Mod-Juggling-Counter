# 文档索引（DOCS.md）

STS2Mod-杂耍计数 —— 为铁甲战士能力牌"杂耍"（Juggling）的 buff 图标右上角添加"本回合已打出攻击牌数"计数。

分类前缀：`T` = 任务知识，`R` = 用户要求的报告，`E` = 研究与探索的证据陈列。

| 编号 | 文档 | 内容 |
|---|---|---|
| 001-E | [001-E-游戏侧杂耍计数与UI调研.md](001-E-游戏侧杂耍计数与UI调研.md) | 游戏侧调研：Juggling / JugglingPower 反编译源码、计数语义澄清（非全局变量，History 可重算）、NPower 与 `power.tscn` 布局、数据源与 UI 注入方案对比 |
| 002-E | [002-E-BaseLib调研.md](002-E-BaseLib调研.md) | BaseLib-StS2 调研：定位 / 版本 / 安装 / 入门手册、`TwoAmountPowers`（`IHasSecondAmount`）先例、对本 mod 的适用性结论 |
| 003-E | [003-E-RitsuLib调研.md](003-E-RitsuLib调研.md) | STS2-RitsuLib 调研：定位 / 版本 / 安装 / 文档站、`ExtraCornerAmountLabels` 角标框架（TopRight 布局常量）、Patching 框架、Capabilities / NodeAttachments 不适用结论 |
| 004-R | [004-R-v0.107兼容性核对报告.md](004-R-v0.107兼容性核对报告.md) | v0.107 兼容性核对报告：逐项依赖点核对（唯一不兼容点 = `CardPlay.Player` 谓词）、修改计划（谓词统一写法 + `min_game_version`）、版本间机制差异记录、107 编译产物在 111 上的可用性结论 |

其他相关文档：

- 构建 / 打包 / 中间文件策略：[1.ADR/002-构建与打包.md](../1.ADR/002-构建与打包.md) 与 `TMP/02-缓存与临时/说明.md`
- 实施结果与 bug 修复记录：[2.PLANS/001-G-杂耍计数实现方案.md](../2.PLANS/001-G-杂耍计数实现方案.md) §10 / §11 / §12
- 软件设计文档：[4.SDD/001-总体设计.md](../4.SDD/001-总体设计.md)

维护说明：本目录存放可复用的知识、调研与报告，不记录工程决策（写 `1.ADR`）与施工计划（写 `2.PLANS`）。
