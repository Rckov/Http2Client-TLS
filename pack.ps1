# Simple script to build NuGet package locally
param(
    [string]$Version = "1.0.0"
)

Write-Host "Building Http2Client NuGet package v$Version" -ForegroundColor Green

try {
    # Update version
    $projectFile = "src/Http2Client.csproj"
    Write-Host "Updating version to $Version..." -ForegroundColor Yellow
    $content = Get-Content $projectFile
    $content = $content -replace '<Version>.*</Version>', "<Version>$Version</Version>"
    Set-Content $projectFile $content

    # Clean and create output directory
    Write-Host "Preparing output directory..." -ForegroundColor Yellow
    if (Test-Path "output") { Remove-Item -Recurse -Force "output" }
    New-Item -ItemType Directory -Path "output" | Out-Null

    # Build and pack
    Write-Host "Restoring dependencies..." -ForegroundColor Yellow
    dotnet restore $projectFile
    if ($LASTEXITCODE -ne 0) { throw "Restore failed" }

    Write-Host "Building project..." -ForegroundColor Yellow
    dotnet build $projectFile --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }

    Write-Host "Creating NuGet package..." -ForegroundColor Yellow
    dotnet pack $projectFile --configuration Release --no-build --output "output"
    if ($LASTEXITCODE -ne 0) { throw "Pack failed" }

    Write-Host "`nPackage created successfully!" -ForegroundColor Green
    Write-Host "Files in output directory:" -ForegroundColor Green
    Get-ChildItem "output" | ForEach-Object { 
        $sizeKB = [math]::Round($_.Length / 1024, 1)
        Write-Host "  $($_.Name) ($sizeKB KB)" -ForegroundColor Cyan 
    }
    
    Write-Host "`nTo publish to NuGet.org, run:" -ForegroundColor Yellow
    Write-Host "  dotnet nuget push output/*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor White

} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}