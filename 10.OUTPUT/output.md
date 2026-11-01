# 构建产物索引（output.md）

本目录存放构建产物与发布包，除本文件外全部被根 `.gitignore` 忽略（不入库）。

## 目录约定

| 子目录 | 内容 |
|---|---|
| `01-构建产物/` | 构建输出副本（当前为空：`build.ps1` 直接把 dll / pdb / json / pck 安装到游戏 `mods/JugglingCounterMod/`，不在仓库内留存副本） |
| `02-发布包/` | GitHub Release 资产压缩包 |
| `03-备份/` | 历史产物备份（当前为空） |

## 产物清单

| 时间 | 名称 | 版本号 | 大小 | 说明 |
|---|---|---|---|---|
| 2026-11-01 | `02-发布包/JugglingCounterMod.v1.0.0.zip` | 1.0.0 | 179 KB（183,124 B） | 从游戏 `mods/JugglingCounterMod/` 打包的发布包：`JugglingCounterMod.dll` / `.pck` / `.pdb` / `.json`，压缩包内顶级目录为 `JugglingCounterMod/`；已排除以 `.` 开头的上传状态目录（`.sts2-workshop-uploader`） |
| 2026-11-01 | `02-发布包/JugglingCounterModSourceCode.zip` | 1.0.0 | 106 KB（108,268 B） | 从 `9.SRC/JugglingCounterMod/` 打包的源码包，排除 `.godot/`、`bin/`、`obj/` 与 `*.dll` / `*.pck` / `*.pdb` |

## 游戏内已安装构建（不在本目录）

| 时间 | 文件 | 大小 | 说明 |
|---|---|---|---|
| 2026-08-22 | `JugglingCounterMod.dll` | 12,800 B | 以 v0.107 游戏程序集编译；v0.107 / v0.111 双版本实测可用 |
| 2026-08-22 | `JugglingCounterMod.pck` | 168,136 B | 仅含 `JugglingCounterMod/mod_image.png`（正式图标，420×420） |
| 2026-08-22 | `JugglingCounterMod.pdb` | 14,948 B | 调试符号 |
| 2026-08-22 | `JugglingCounterMod.json` | 442 B | manifest（`version=1.0.0`、`min_game_version=0.107.0`、`has_pck=true`、`affects_gameplay=false`） |

安装路径：`C:\Game\Steam\steamapps\common\Slay the Spire 2\mods\JugglingCounterMod\`。

## 登记规则

新增产物后必须在本表登记：时间（YYYY-MM-DD）、名称（相对本目录路径）、版本号、大小、说明。
