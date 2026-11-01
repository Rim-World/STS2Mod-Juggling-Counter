# BaseLib-StS2 调研报告

- 调研日期：2026-08-22
- 调研对象：https://github.com/Alchyr/BaseLib-StS2
- 本地克隆：`C:\Users\<用户名>\Desktop\Mod\STS2-Reference\BaseLib-StS2`（tag `v3.4.0`）
- 线上最新：v3.4.5（2026-08-14 发布，release notes 含 "beta compat"）
- 关联项目：ModTemplate-StS2（https://github.com/Alchyr/ModTemplate-StS2，README 即入门手册）

## 1. 定位与安装

- BaseLib 是 STS2 的"基础库"型 mod，目的是把**自定义内容**（卡牌/遗物/药水/角色/事件/敌人等）的注册、本地化、图集、保存等标准化，供其他 mod 作为依赖使用。
- 安装：`Slay the Spire 2/mods/` 下放置 `BaseLib.dll + BaseLib.pck + BaseLib.json`（release 产物）；引用方式 `<PackageReference Include="Alchyr.Sts2.BaseLib" Version="*" />`（NuGet）。
- manifest（本地 v3.4.0）：`id=BaseLib`、`version=v3.4.0`、`min_game_version=0.107.1`、`affects_gameplay=false`、`has_pck=true, has_dll=true`。
- 入门手册：
  - Wiki：https://alchyr.github.io/BaseLib-Wiki/（官方 wiki，正在建设中）
  - 起步指引：https://github.com/Alchyr/ModTemplate-StS2/blob/master/README.md
  - 仓库内文档：`docs/auto_conversion.md`（资源自动转换）、`docs/FmodAudio.md`
- 授权：LICENSE.txt（见仓库；先例代码 Credits 到 kiooeht 的 Minty-Spire-2）。

## 2. 能力清单（与本 mod 相关的部分）

BaseLib 主要能力面向"新增内容"，与本 mod（给**游戏内置**卡牌的 buff 加 UI 角标）相关的只有一处：

### 2.1 TwoAmountPowers —— "第二数值"显示（直接先例）

`Patches/Utils/TwoAmountPowers.cs`（源自 https://github.com/erasels/Minty-Spire-2 的 kiooeht）：

- `[HarmonyPatch(typeof(NPower))]`，三个补丁：
  1. `RefreshAmount` Postfix：若 `__instance._model is IHasSecondAmount power`，则：
     - 无 `Amount2Label` 子节点时 `Duplicate()` 一份 `%AmountLabel`（MegaLabel），命名 `Amount2Label`、`UniqueNameInOwner=false`、`AddChild` 并 `MoveChild` 到原标签同索引；
     - `SetTextAutoSize(power.GetSecondAmount())`；
     - 位置 = 原标签 Position + `(0, -(fontSize + 2))`（即**原层数标签正上方** 20px）；
     - 空文本时隐藏标签。
  2. `SubscribeToModelEvents` Postfix：把 power 注册进 `SecondAmountRegistry`（`ConditionalWeakTable<IHasSecondAmount, Action>`，value = `__instance.RefreshAmount`）。
  3. `UnsubscribeFromModelEvents` Postfix：注销。
- 配套公开 API：
  - `BaseLib.Abstracts.IHasSecondAmount`：`string GetSecondAmount();`
  - `BaseLib.Extensions.PowerExtensions.InvokeSecondAmountChanged(this IHasSecondAmount)`：值变化时调用，经 registry 触发对应 NPower 的 `RefreshAmount`。
- 接口 XML 注释明确：**在任意 PowerModel 上实现该接口即可显示第二数值**（如计数器/冷却）。

### 2.2 其他与本需求无关但值得知道的能力

- `Abstracts/`：CustomCardModel / CustomPowerModel / CustomRelicModel / CustomCharacterModel 等一套自定义内容基类；`ICustomModel`、`ICustomPower` 等接口。
- `Hooks/`：`IAfterScryed` / `IModifyScryAmount` / `IHealAmountModifier` / `IMaxHandSizeModifier` / `IHealthBarForecastSource` / `IAfterCardDowngraded` / `ICardTypeTextModifier` 等 hook 接口（通过 `BaseLibHooks` 在战斗状态上分发，供自定义模型实现）。
- `Extensions/`：大量 Godot/Harmony/模型扩展（NodeExtensions、ControlExtensions、PowerExtensions、CardExtensions、ModelDbExtensions…）。
- `Patches/UI/`：自定义能量图标、自定义资源 UI、血条预测、图鉴、角色选择等 UI 挂钩。
- `Patches/Localization/`：自定义本地化表、描述覆盖、额外 tooltip。
- `Patches/Saves/`：扩展保存类型/处理器。

## 3. 对本 mod 的适用性结论

| 问题 | 结论 |
|---|---|
| 能否直接复用 TwoAmountPowers 给杂耍 buff 显示第二数值？ | **不能直接复用**。`IHasSecondAmount` 需要 PowerModel 实现该接口，而 `JugglingPower` 是游戏内置 **sealed** 类（`MegaCrit.Sts2.Core.Models.Powers.JugglingPower`），无法被 mod 修改为实现接口。 |
| 能否通过包装/替换 JugglingPower 达成？ | 不可行：sealed 无法继承；替换游戏卡牌行为违背本 mod"只加显示"的目标。 |
| TwoAmountPowers 的价值 | **设计参考**：Duplicate `%AmountLabel` 创建第二个 MegaLabel 的思路、`SecondAmountRegistry`（CWT 弱引用注册表）触发刷新的模式、`SubscribeToModelEvents/UnsubscribeFromModelEvents` 挂钩生命周期的方式，都可直接借鉴（本 mod 可照此思路针对 `JugglingPower` 特化实现）。 |
| 依赖 BaseLib 的收益 | 对本 mod 极小。不依赖它，玩家无需安装 BaseLib；避免 BaseLib 与游戏版本错位风险（BaseLib 版本节奏独立，v3.4.0→v3.4.5 间隔很短）。 |

## 4. 版本与兼容性备注

- 本地 clone 为 v3.4.0；线上最新 v3.4.5（2026-08-14，与 v0.111.0 同期，release notes 提及 "beta compat"，应已适配 v0.111）。
- BaseLib 依赖游戏内部 API 面较宽（自定义内容注册、保存、本地化），每次游戏更新都可能影响；若仅为了两个工具方法而整体依赖，性价比低。
- 若未来本 mod 扩展为"新增自定义内容"，届时再评估引入 BaseLib 不迟（ADR 可记录该触发条件）。
