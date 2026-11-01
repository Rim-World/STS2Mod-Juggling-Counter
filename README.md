# STS2Mod-杂耍计数（Juggling Counter）

为《杀戮尖塔 2》铁甲战士能力牌"杂耍（Juggling）"的 buff 图标右上角添加"本回合已打出攻击牌数"计数。

A Slay the Spire 2 mod that shows the number of Attack cards played this turn at the top-right corner of the Juggling buff icon.

| 项目 | 值 |
|---|---|
| Mod ID | `JugglingCounterMod` |
| 版本 | 1.0.0 |
| 作者 | 再玩小卡组找人弄你 |
| 最低游戏版本 | 0.107.0 |
| 依赖 | 无 |
| 是否影响玩法 | 否（纯显示增强） |
| 开源地址 | https://github.com/Rim-World/STS2Mod-Juggling-Counter |

## 功能

- 在"杂耍"buff 图标右上角显示本回合已打出的攻击牌数，右下角原有的层数（复制次数）不受影响。
- 始终显示数字（含 `0`），不封顶；回合切换后自动归零。
- 先打出攻击牌再打出杂耍时，计数从战斗历史回溯统计，显示值与游戏实际状态一致。
- 纯显示增强：不改变卡牌效果、数值与游戏机制，不写存档，关闭 mod 后一切恢复原状。

## 兼容性

- 支持 v0.107.x（default 分支）与 v0.111.0（public beta），单一 DLL 双版本通用，两版本均已实测。
- 零第三方依赖（纯 Harmony 补丁），与 BaseLib / RitsuLib 同装无冲突。
- 不保证与盗版、修改版、移动版、老旧版本或冷门/次级 mod 的兼容性。

## 安装

1. 将 `JugglingCounterMod` 文件夹（含 `JugglingCounterMod.dll`、`JugglingCounterMod.pck`、`JugglingCounterMod.json`）放入游戏目录下的 `mods/`。
2. 启动游戏，在 mod 列表中启用"杂耍计数 (Juggling Counter)"。
3. 战斗中拥有"杂耍"能力时，图标右上角即显示本回合攻击牌计数。

## 构建

构建需要 .NET 9 SDK、Godot 4.5.1（mono）以及本机已安装的游戏程序集（`sts2.dll`、`0Harmony.dll`）。

```powershell
cd 9.SRC/JugglingCounterMod
powershell -File build.ps1
```

脚本流程：`dotnet build`（`bin`/`obj` 重定向到项目根 `%TEMP%`）→ Godot `--import` 与 `--export-pack` 生成 pck → 把 dll / pdb / json / pck 安装到 `Steam/steamapps/common/Slay the Spire 2/mods/JugglingCounterMod/`。工程路径与游戏路径在 `JugglingCounterMod.csproj`、`build.ps1` 中为本机配置，换机需自行调整。

## 目录结构

```
STS2Mod-杂耍计数/
├── AGENTS.md          仓库与助手工作规范（与上级目录 AGENTS.md 一致）
├── README.md          本文件
├── CHANGELOG.md       Keep a Changelog 单文件更新日志
├── .gitignore
├── LICENSE            MIT
├── 1.ADR/             架构决策记录（ADR.md 为索引）
├── 2.PLANS/           施工计划（PLANS.md 为索引）
├── 3.DOCS/            调研与报告（DOCS.md 为索引）
├── 4.SDD/             软件设计文档（SDD.md 为索引）
├── 5.ChangeLog/       版本日志分片（ChangeLog.md 为索引）
├── 9.SRC/             源码工程 JugglingCounterMod/
├── 10.OUTPUT/         构建产物与发布包（output.md 为索引，内容不入库）
├── PRIVACY/           机密信息（PRIVACY.md 为索引，永不入库）
├── TMP/               临时与过程文件（TMP.md 为索引，内容不入库）
├── TOOLS/             工具与自写脚本（TOOLS.md 为索引）
├── description/       创意工坊发布文案（中英文）
└── pics/              封面与素材图
```

## 文档索引

| 目录 | 索引 | 说明 |
|---|---|---|
| 架构决策 | [1.ADR/ADR.md](1.ADR/ADR.md) | 零依赖自研、构建打包、双版本兼容三篇决策 |
| 施工计划 | [2.PLANS/PLANS.md](2.PLANS/PLANS.md) | 实现方案与施工记录回填 |
| 知识文档 | [3.DOCS/DOCS.md](3.DOCS/DOCS.md) | 游戏侧 / BaseLib / RitsuLib 调研与 v0.107 兼容核对报告 |
| 软件设计 | [4.SDD/SDD.md](4.SDD/SDD.md) | 总体设计：模块划分、数据流、兼容与测试 |
| 版本日志 | [5.ChangeLog/ChangeLog.md](5.ChangeLog/ChangeLog.md)、[CHANGELOG.md](CHANGELOG.md) | 版本记录与单文件更新日志 |
| 源码 | [9.SRC/JugglingCounterMod/README.md](9.SRC/JugglingCounterMod/README.md) | 面向玩家的英文说明（[中文](9.SRC/JugglingCounterMod/README.zh.md)） |
| 产物 | [10.OUTPUT/output.md](10.OUTPUT/output.md) | 构建产物与发布包登记 |
| 工具 | [TOOLS/TOOLS.md](TOOLS/TOOLS.md) | 自写脚本清单 |

## 版本

当前版本 **1.0.0**（2026-08-22）。版本变更见 [CHANGELOG.md](CHANGELOG.md)。

## 致谢

- 感谢《杀戮尖塔 2》制作组为 mod 开发提供的便利。
- 感谢 BaseLib 与 RitsuLib 的作者提供的开源实现与文档（本 mod 的角标设计参考了二者）。
- 本 mod 使用 DeepSeek Harness（WebUI）+ DeepSeek V4 Flash 完成。

## 许可

MIT，见 [LICENSE](LICENSE)。
