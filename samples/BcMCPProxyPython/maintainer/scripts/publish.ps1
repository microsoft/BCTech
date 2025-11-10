param(
  [string]$Repository = "pypi"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir
Set-Location ..
Set-Location ..

if (-not (Test-Path dist)) {
  Write-Host "dist/ folder not found. Run maintainer/scripts/build.ps1 first." -ForegroundColor Yellow
  exit 1
}

Write-Host "Installing twine..." -ForegroundColor Cyan
python -m pip install --upgrade twine | Out-Null

Write-Host "Uploading artifacts to '$Repository'..." -ForegroundColor Cyan
python -m twine upload --repository "$Repository" dist/*

