$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

Write-Host "==> Building frontend..."
Set-Location "$scriptDir\src\frontend"
& npm install
& npm run build

Write-Host "==> Starting backend..."
Set-Location "$scriptDir\src\GitHubExplorer.Api"
& dotnet run @args
