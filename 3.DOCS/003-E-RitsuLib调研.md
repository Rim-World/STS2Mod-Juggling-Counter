# STS2-RitsuLib 调研报告

- 调研日期：2026-08-22
- 调研对象：https://github.com/BAKAOLC/STS2-RitsuLib
- 本地克隆：`C:\Users\<用户名>\Desktop\Mod\STS2-Reference\STS2-RitsuLib`（tag `v0.5.10`，含中英文双 README 与完整文档站源码）
- 本地另有解包：`STS2-Reference\ritsulib-decompiled-core\`（DLL 反编译，可对照）
- 线上最新：v0.5.14（2026-08-21 发布）
- 授权：MIT

## 1. 定位与安装

- RitsuLib 是 STS2 的通用框架型 mod：内容注册、生命周期钩子、Harmony 补丁框架、持久化、设置 UI、本地化、运行时 UI、诊断与兼容性助手，**与游戏 API 并列**（sits beside the base game API），不替代 BaseLib。
- 安装：`mod_manifest.json` 声明依赖 `{ "id": "STS2-RitsuLib" }`（游戏 API ≥0.105 用对象形式），玩家安装 `mods/STS2-RitsuLib/`（DLL-only，`has_pck: false`）；编译期 NuGet 包 `STS2.RitsuLib`。
- 多版本策略：`STS2.RitsuLib` 主包跟随最新（常为 beta 分支）；稳定分支用 `STS2.RitsuLib.Compat.<api-version>`；玩家侧可用 variant-pack 一个目录支持多 API 版本。
- manifest（本地 v0.5.10）：`id=STS2-RitsuLib`、`version=0.5.10`、`affects_gameplay=false`、`has_dll=true`。
- 入门手册：
  - 文档站（中英双语）：https://sts2-ritsulib.ritsukage.com/ ，从 [getting-started](https://sts2-ritsulib.ritsukage.com/guide/getting-started) 开始，按功能进主题页
  - 本地文档源：`docs/pages/guide/*.md`（getting-started / content-authoring-toolkit / lifecycle-events / patching-guide / persistence-guide / mod-settings / diagnostics-and-compatibility / godot-scene-authoring / framework-design / localization-and-keywords …）
  - 中文 README：`README.zh.md`

## 2. 与本需求直接相关的能力：Combat.Ui.ExtraCornerAmountLabels

`src/Combat/Ui/ExtraCornerAmountLabels/` 是一个**专门为 buff/遗物/意图图标添加"四角角标"的框架**，与本 mod 需求（杂耍 buff 右上角加计数）几乎一一对应：

### 2.1 公共接口（Power 侧）

- `IPowerExtraIconAmountLabelsProvider`：`IReadOnlyList<ExtraIconAmountLabelSlot> GetPowerExtraIconAmountLabelSlots();` —— 给 NPower 提供纯文本角标（独立于游戏原有层数标签）。
- `IPowerExtraIconAmountLabelSpecsProvider`：返回 `ExtraIconAmountLabelSpec[]`（带文本模式/自定义矩形的高级形式；两者都实现时 specs 优先）。
- `IPowerExtraIconAmountLabelsChangeSource`：`event Action? PowerExtraIconAmountLabelsInvalidated;` —— 数据变化时触发，runtime 重同步角标。
- `ExtraIconAmountLabelCorner` 枚举：`TopLeft / TopRight / BottomLeft / BottomRight / Custom`。
- `ExtraIconAmountLabelSlot` / `ExtraIconAmountLabelSpec`：文本 + 角落 + 可选自定义矩形/颜色；`Plain(...)`/`RichText(...)` 工厂。
- 同理还有 Relic（`IRelicExtraIconAmountLabelsProvider`，宿主 `NRelicInventoryHolder`）与 Intent（`IIntentExtraCornerAmountLabelsProvider`，宿主 `NIntent`）。

### 2.2 Power 右上角布局（已内置，正是本 mod 需要的位置）

`ExtraCornerHostLayout.cs` 内置 Power 角标布局常量：

| 角落 | 矩形（宿主局部坐标） | 说明 |
|---|---|---|
| BottomRight（游戏原层数） | left=-56, top=21, right=44, bottom=44 | 与原 `%AmountLabel` 场景坐标一致（右下角） |
| **TopRight** | **left=-56, top=0, right=44, bottom=23** | 顶行、右对齐 → 图标右上角 |
| TopLeft | left=0, top=0, right=44, bottom=23 | |
| BottomLeft | left=0, top=21, right=44, bottom=44 | |

即右上角角标与原层数标签同宽（100px）、同高（23px）、同右对齐，只是 y 移到顶部行。

### 2.3 排版样式复制

`ExtraCornerPowerAmountStyle.Apply`：把 NPower 层数标签的 Min/MaxFontSize、字体、字号、颜色、描边、阴影逐项复制到角标，`AutoSizeEnabled = true`（有 fallback 字体兜底）。

### 2.4 Runtime 机制（供设计借鉴）

`ExtraCornerAmountLabelsRuntime` + `ExtraCornerAmountLabelsPatches`：

- `NPower.RefreshAmount` Postfix → `SyncPower(node)`：读模型（反射 `AccessTools.Field(typeof(NPower), "_amountLabel")` 拿样式参考），检查模型是否实现上述接口，未实现则 `ClearPower`；实现则快照 spec、按角标创建/复用子标签（`RitsuExtraPowerCornerSlot_` 前缀命名）、订阅 `PowerExtraIconAmountLabelsInvalidated`。
- `NPower._ExitTree` Postfix → `ClearPower(node)`：注销订阅、释放角标节点（防止泄漏）。
- 状态管理：`Dictionary<ulong instanceId, PowerHostState>` + `Lock`，按节点实例 id 跟踪。

### 2.5 对本 mod 的适用性结论

| 问题 | 结论 |
|---|---|
| 能否直接给 JugglingPower 用？ | **不能**。`SyncPower` 要求 `model is IPowerExtraIconAmountLabelsProvider`（或 SpecsProvider），`JugglingPower` 是游戏内置 sealed 类，无法实现 RitsuLib 接口；也没有全局注册表可绕开接口。 |
| 能否让 RitsuLib 的 Capabilities 机制扩展 PowerModel？ | 不能。`src/Models/Capabilities/` 只有 `CardModelCapabilityContributors` 与 `OrbModelCapabilityHost`（卡牌/宝珠能力宿主），**没有 PowerModel 能力宿主**。 |
| NodeAttachments（Scaffolding.Godot）呢？ | 不适用。`INodeAttachmentFactory`/`NodeAttachmentDefinition` 是给 **mod 自己创建的场景节点** 附加子节点用的（角色视觉、卡牌拖尾等），不作用于游戏已有的 NPower 场景。 |
| 价值 | ① 布局常量与样式复制实现是本 mod"右上角标签"的直接设计蓝本；② `IPatchMethod`/`ApplyRequiredPatcher` 补丁框架可降低 patch 失败风险；③ 若未来本 mod 增加设置项，`RegisterModSettings` 可直接用。 |

## 3. 其他能力概览（本 mod 用不到，但记录备查）

- **Patching 框架**（`src/Patching/` + docs/patching-guide.md）：`RitsuLibFramework.CreatePatcher(modId, name)` → `RegisterPatches<T>()`（T 实现 `IPatchMethod`，强类型声明目标 `ModPatchTarget`/`PatchTarget.*`）→ `ApplyRequiredPatcher(patcher, DisableMod)`；支持 `ignoreIfMissing` 可选目标、诊断与失败禁用。相比裸 Harmony：目标声明集中、可降级、可诊断。
- **Content authoring**：卡牌/遗物/药水/角色/事件/遭遇/时间线/解锁/关键词/标签注册。
- **Lifecycle**：`SubscribeLifecycle<TEvent>`（框架与游戏时机事件）。
- **持久化**：profile/run 数据 store、JSON 迁移；**设置 UI**：`RegisterModSettings`。
- **本地化**：SmartFormat、关键字、动态变量工具包；**音频**：FMOD 助手。
- **运行时 UI**：TopBar 按钮、卡牌堆、toast、Shell 主题、调试工具 dock、Overlay 宿主（全局覆盖层，非角标用途）。
- **诊断/兼容**：API capability 门控、启动审计、`STS2.RitsuLib.Compat.*` 多版本包。
- **Interop**：mod 间 JSON 通信（KeyedJsonDomTransport 等）。

## 4. 版本与兼容性备注

- 本地 clone v0.5.10；线上最新 v0.5.14（2026-08-21）。RitsuLib 通过 compat 包机制显式管理游戏 API 版本（主包跟随最高 API，通常为 beta），安装 variant-pack 可覆盖多版本。
- 若依赖 RitsuLib，mod 的编译目标（主包 vs Compat 包）需与用户游戏分支（v0.111.0 正式版）匹配——需在构建时按 release 页资产确认；若只是借鉴其设计（MIT 开源），则无此负担。
