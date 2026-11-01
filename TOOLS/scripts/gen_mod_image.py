# -*- coding: utf-8 -*-
"""生成 420×420 纯色 PNG（空白占位 mod 图标，创意工坊规格）。

输出路径由脚本位置推算（避免命令行中文参数编码问题）：
  本脚本位于 <项目根>/%TEMP%/tools/  →  输出 <项目根>/src/JugglingCounterMod/JugglingCounterMod/mod_image.png
纯 Python 标准库（struct + zlib）手写 PNG，无第三方依赖。
"""
import os
import struct
import zlib

SIZE = 420
COLOR = (0x80, 0x80, 0x80)  # 中灰，纯色占位

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = os.path.dirname(os.path.dirname(SCRIPT_DIR))  # 项目根 = %TEMP% 的上级
OUT = os.path.join(PROJECT_ROOT, "src", "JugglingCounterMod", "JugglingCounterMod", "mod_image.png")


def chunk(tag: bytes, data: bytes) -> bytes:
    c = tag + data
    return struct.pack(">I", len(data)) + c + struct.pack(">I", zlib.crc32(c) & 0xFFFFFFFF)


def main() -> None:
    row = b"\x00" + bytes(COLOR) * SIZE  # 每行: 滤波器类型0 + RGB
    raw = row * SIZE
    png = b"\x89PNG\r\n\x1a\n"
    png += chunk(b"IHDR", struct.pack(">IIBBBBB", SIZE, SIZE, 8, 2, 0, 0, 0))
    png += chunk(b"IDAT", zlib.compress(raw, 9))
    png += chunk(b"IEND", b"")
    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "wb") as f:
        f.write(png)
    print(f"written {OUT}: {SIZE}x{SIZE} RGB, {len(png)} bytes")


if __name__ == "__main__":
    main()
