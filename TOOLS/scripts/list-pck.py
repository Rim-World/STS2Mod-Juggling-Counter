# -*- coding: utf-8 -*-
"""列出 pck 内文件清单（Godot pck v3 目录表）。

用法: python list-pck.py <pck路径> [匹配子串]
"""
import struct
import sys

PCK = sys.argv[1]
match = sys.argv[2] if len(sys.argv) > 2 else ""

with open(PCK, "rb") as f:
    f.read(4)   # magic GDPC
    f.read(4)   # version
    f.read(12)  # godot version
    f.read(4)   # flags
    fb, do = struct.unpack("<QQ", f.read(16))
    f.seek(do)
    n = struct.unpack("<I", f.read(4))[0]
    total = 0
    for _ in range(n):
        plen = struct.unpack("<I", f.read(4))[0]
        p = f.read(plen).decode("utf-8").rstrip("\x00").rstrip()
        off, size = struct.unpack("<QQ", f.read(16))
        f.read(16)  # md5
        f.read(4)   # flags
        if match in p:
            print(f"{size:>10}  {p}")
            total += 1
    print(f"--- matched {total} of {n} entries ---")
