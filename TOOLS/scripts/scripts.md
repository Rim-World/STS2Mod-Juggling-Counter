# scripts 目录说明（scripts.md）

| 时间 | 脚本 | 用途 | 语言 | 依赖 |
|---|---|---|---|---|
| 2026-08-22 | `gen_mod_image.py` | 生成 420×420 中灰纯色占位 mod 图标 PNG（已被正式图标取代，仅留档） | Python 3 | 标准库（`zlib`、`struct`） |
| 2026-08-22 | `list-pck.py` | 解析 Godot `.pck`，列出包内文件清单 | Python 3 | 标准库（`struct`、`sys`） |

两个脚本均为一次性工具，由原项目根 `%TEMP%/tools/` 迁入；核心逻辑已固化在 `9.SRC/JugglingCounterMod/build.ps1` 与 `export_presets.cfg` 中，删除脚本不影响构建。

## 用法

- `python gen_mod_image.py`：无命令行参数。脚本按自身位置推算输出路径（原为 `<项目根>/src/JugglingCounterMod/JugglingCounterMod/mod_image.png`）；迁入 `TOOLS/scripts/` 后推算路径已失效，如需复用请先改 `OUT` 常量。
- `python list-pck.py <pck路径> [匹配子串]`：打印 pck 内文件清单，可选按子串过滤。
