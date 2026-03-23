<#
.SYNOPSIS
    Builds and installs the BC Admin Center MCP Proxy to the user's local AppData folder.

.DESCRIPTION
    - Publishes a self-contained .NET 8 build
    - Copies output to %LOCALAPPDATA%\BcMcpProxy
    - Prints the VS Code MCP configuration snippet

.PARAMETER Configuration
    Build configuration. Default: Release

.EXAMPLE
    .\scripts\install.ps1
#>
[CmdletBinding()]
param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$projectDir = Split-Path -Parent $PSScriptRoot
$projectFile = Join-Path $projectDir "BcAdminMcpProxy.csproj"
$installDir = Join-Path $env:LOCALAPPDATA "BcMcpProxy"
$exeName = "BcAdminMcpProxy.exe"

Write-Host "=== BC Admin Center MCP Proxy Installer ===" -ForegroundColor Cyan
Write-Host ""

# 1. Build
Write-Host "[1/3] Publishing ($Configuration)..." -ForegroundColor Yellow
$publishDir = Join-Path $projectDir "out" "publish"
dotnet publish $projectFile `
    --configuration $Configuration `
    --output $publishDir `
    --self-contained false `
    --nologo `
    -v quiet

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed."
    exit 1
}
Write-Host "      Published to: $publishDir" -ForegroundColor Gray

# 2. Copy to AppData
Write-Host "[2/3] Installing to $installDir ..." -ForegroundColor Yellow
if (Test-Path $installDir) {
    $response = Read-Host "BcAdminMcpProxy is already installed at $installDir. Overwrite? (y/N)"
    if ($response -ne 'y' -and $response -ne 'Y') {
        Write-Host "      Installation cancelled." -ForegroundColor Red
        exit 0
    }

    Remove-Item "$installDir\*" -Recurse -Force
    Copy-Item "$publishDir\*" $installDir -Recurse -Force
} else {
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null
    Copy-Item "$publishDir\*" $installDir -Recurse -Force
}
Write-Host "      Installed." -ForegroundColor Gray

# 3. Print VS Code config
$exePath = Join-Path $installDir $exeName
$escapedPath = $exePath.Replace('\', '\\')

Write-Host "[3/3] Done!" -ForegroundColor Green
Write-Host ""
Write-Host "Add this to your VS Code settings.json or .vscode/mcp.json:" -ForegroundColor Cyan
Write-Host ""
Write-Host @"
{
  "mcp": {
    "servers": {
      "bc-admin-center": {
        "type": "stdio",
        "command": "$escapedPath"
      }
    }
  }
}
"@ -ForegroundColor White

Write-Host ""
Write-Host "Executable: $exePath" -ForegroundColor Gray
Write-Host "Config:     $(Join-Path $installDir 'appsettings.json')" -ForegroundColor Gray
Write-Host "Token cache: $(Join-Path $env:LOCALAPPDATA 'BcMcpProxy' 'bc_mcp_proxy_token_cache.bin')" -ForegroundColor Gray
