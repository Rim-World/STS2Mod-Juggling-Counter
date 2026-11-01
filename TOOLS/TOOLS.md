# TOOLS 目录索引（TOOLS.md）

本目录收纳本项目的自写脚本与工具。一级子项只有：`scripts/`（自写脚本）与本文件。

## 自写脚本

| 时间 | 工具/脚本 | 用途 | 版本 | 大小 | 来源 | 安装方式/依赖 |
|---|---|---|---|---|---|---|
| 2026-08-22 | `scripts/gen_mod_image.py` | 生成 420×420 中灰纯色占位 mod 图标 PNG（现已由正式图标 `pics/Juggling Counter_small.png` 取代，仅留档） | 无版本号 | 1,417 B | 本项目自写（原项目根 `%TEMP%/tools/gen_mod_image.py`） | 免安装；Python 3 标准库（`zlib`/`struct`），无第三方依赖 |
| 2026-08-22 | `scripts/list-pck.py` | 解析 Godot `.pck` 文件并列出内部文件清单（用于验证打包内容不含源码或 `%TEMP%` 目录） | 无版本号 | 874 B | 本项目自写（原项目根 `%TEMP%/tools/list-pck.py`） | 免安装；Python 3 标准库，无第三方依赖 |

## 本机开发环境（不收录在本目录）

| 工具 | 版本 | 路径 | 说明 |
|---|---|---|---|
| .NET SDK | 9.x（`net9.0` 目标） | 系统安装 | 编译 mod 程序集 |
| Godot | 4.5.1-stable mono | `C:\Godot&GDRE\Godot_v4.5.1-stable_mono_win64\` | `--import` 与 `--export-pack` 生成 pck |
| ilspycmd | 未记录 | 系统安装 | 反编译验证产物引用（一次性排查用） |

构建入口是源码工程内的 `9.SRC/JugglingCounterMod/build.ps1`，不属于自写工具范畴，不收录在本目录。

## 维护说明

- 新脚本放入 `scripts/`，并同时更新本表与 `scripts/scripts.md`。
- 若引入需要独立目录的第三方工具（便携版），在 `TOOLS/` 下单独建以工具命名的文件夹安装，并在此登记版本、来源、大小与环境变量。
