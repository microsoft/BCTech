param(
  [string]$Configuration = "release"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir
Set-Location ..
Set-Location ..

Write-Host "Installing build backend..." -ForegroundColor Cyan
python -m pip install --upgrade build | Out-Null

Write-Host "Building bc-mcp-proxy package ($Configuration)..." -ForegroundColor Cyan
python -m build

Write-Host "Artifacts written to:" -ForegroundColor Green
Get-ChildItem -Path dist

