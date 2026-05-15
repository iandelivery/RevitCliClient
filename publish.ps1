$ProjectPath = Join-Path $PSScriptRoot "RevitCliClient.csproj"
$OutputPath = Join-Path $PSScriptRoot "publish"

Write-Host "Publishing Revit CLI Client..." -ForegroundColor Cyan

dotnet publish $ProjectPath `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -o $OutputPath

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nPublished successfully!" -ForegroundColor Green
    Write-Host "Output: $OutputPath" -ForegroundColor Green
    Write-Host "Executable: $OutputPath\RevitCliClient.exe" -ForegroundColor Green

    $ExePath = Join-Path $OutputPath "RevitCliClient.exe"
    if (Test-Path $ExePath) {
        $Size = (Get-Item $ExePath).Length / 1MB
        Write-Host "Size: $([math]::Round($Size, 2)) MB" -ForegroundColor Green
    }
} else {
    Write-Host "`nPublish failed!" -ForegroundColor Red
    exit 1
}
