#!/usr/bin/env pwsh
# Build script for YouTubeCLI - Creates single file executables

param(
    [string]$Runtime = "osx-arm64",
    [string]$Configuration = "Release",
    [switch]$Clean
)

# Change to src directory
Set-Location -Path "src"

Write-Host "Building YouTubeCLI as single file executable..." -ForegroundColor Green
Write-Host "Runtime: $Runtime" -ForegroundColor Yellow
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow

# Clean if requested
if ($Clean) {
    Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
    dotnet clean
    Remove-Item -Path "bin", "obj" -Recurse -Force -ErrorAction SilentlyContinue
}

# Build the project as a single file
try {
    dotnet publish -r $Runtime -c $Configuration -p:PublishSingleFile=true --self-contained

    if ($LASTEXITCODE -eq 0) {
        $outputPath = "bin/$Configuration/net8.0/$Runtime/publish/"
        Write-Host "Build successful!" -ForegroundColor Green
        Write-Host "Executable location: $outputPath" -ForegroundColor Cyan

        # List the output files
        Get-ChildItem -Path $outputPath | Format-Table Name, Length, LastWriteTime

        # Show executable info
        $exeName = if ($Runtime -like "win-*") { "ytc.exe" } else { "ytc" }
        $exePath = Join-Path $outputPath $exeName

        if (Test-Path $exePath) {
            $fileSize = (Get-Item $exePath).Length
            $fileSizeMB = [math]::Round($fileSize / 1MB, 2)
            Write-Host "Executable: $exeName ($fileSizeMB MB)" -ForegroundColor Green
        }
    }
    else {
        Write-Host "Build failed!" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "Error during build: $_" -ForegroundColor Red
    exit 1
}
finally {
    # Return to original directory
    Set-Location -Path ".."
}

Write-Host "Build complete!" -ForegroundColor Green