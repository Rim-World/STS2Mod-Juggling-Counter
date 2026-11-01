$ErrorActionPreference = 'Stop'

$root = $PSScriptRoot
$modId = 'JugglingCounterMod'
$game = 'C:\Game\Steam\steamapps\common\Slay the Spire 2'
$godot = 'C:\Godot&GDRE\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64_console.exe'
$modOut = Join-Path $game "mods\$modId"

# 中间文件目录：项目根下的 %TEMP%（与工程目录分离，说明见 %TEMP%\说明.md）
$projectRoot = Split-Path (Split-Path $root -Parent) -Parent
$tempRoot = Join-Path $projectRoot '%TEMP%'
$buildRoot = Join-Path $tempRoot 'build'
$godotCache = Join-Path $tempRoot 'godot-cache'
$godotDir = Join-Path $root '.godot'

# MSBuild 命令行属性值中的 % 需转义为 %25（否则会被当作 item metadata 引用解析）
$escapedBuild = $buildRoot.Replace('%', '%25')

# 0) 恢复上次构建移走的 .godot 缓存（若存在），避免全量重新 import
if ((Test-Path $godotCache) -and -not (Test-Path $godotDir)) {
    Move-Item $godotCache $godotDir
}

# 1) .NET 编译：输出/中间目录全部重定向到 %TEMP%\build（Godot.NET.Sdk 默认写到项目 .godot/mono/temp，此处用命令行 global property 覆盖）
Write-Host "==> Build .NET assembly (output -> $buildRoot)"
dotnet build (Join-Path $root "$modId.csproj") -c Debug `
  -p:BaseOutputPath="$escapedBuild\bin\" `
  -p:OutputPath="$escapedBuild\bin\Debug\" `
  -p:BaseIntermediateOutputPath="$escapedBuild\obj\" `
  -p:IntermediateOutputPath="$escapedBuild\obj\Debug\"
if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }

$dll = Join-Path $buildRoot 'bin\Debug\JugglingCounterMod.dll'
$pdb = Join-Path $buildRoot 'bin\Debug\JugglingCounterMod.pdb'
if (-not (Test-Path $dll)) { throw "DLL not found: $dll" }

# 2) Godot import（生成 .import/.ctex 元数据；.godot 缓存目录在工程下临时生成，见步骤 4）
Write-Host "==> Godot import"
Push-Location $root
try {
    & $godot '--headless' '--import' '--path' $root 2>&1 | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "Godot import failed: $LASTEXITCODE" }

    # 3) Godot export pck（export_presets.cfg 的 export_files 只含 mod_image.png，%TEMP% 不会被包入）
    Write-Host "==> Godot export pck"
    New-Item -ItemType Directory -Force -Path $modOut | Out-Null
    $pck = Join-Path $modOut "$modId.pck"
    & $godot '--headless' '--export-pack' 'Windows Desktop' $pck '--path' $root 2>&1 | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "Godot export-pack failed: $LASTEXITCODE" }
}
finally {
    Pop-Location
}

# 4) 把 .godot 缓存移到 %TEMP%\godot-cache，保持工程目录干净
if (Test-Path $godotDir) {
    if (Test-Path $godotCache) { Remove-Item $godotCache -Recurse -Force }
    Move-Item $godotDir $godotCache
}

# 5) 拷贝产物到游戏 mods 目录
Write-Host "==> Copy files to $modOut"
Copy-Item $dll (Join-Path $modOut "$modId.dll") -Force
if (Test-Path $pdb) { Copy-Item $pdb (Join-Path $modOut "$modId.pdb") -Force }
Copy-Item (Join-Path $root "$modId.json") (Join-Path $modOut "$modId.json") -Force

Write-Host '==> Done'
