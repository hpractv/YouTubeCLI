#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Run code coverage analysis for YouTubeCLI project.

.DESCRIPTION
    This script runs the test suite with code coverage collection and generates
    a comprehensive HTML report showing line and branch coverage.

.PARAMETER OutputDir
    Directory to output coverage reports. Defaults to "./coverage-output".

.PARAMETER OpenReport
    Open the HTML coverage report in the default browser after generation.

.EXAMPLE
    .\run-coverage.ps1

.EXAMPLE
    .\run-coverage.ps1 -OutputDir "./my-coverage" -OpenReport
#>

param(
    [string]$OutputDir = "./coverage-output",
    [switch]$OpenReport
)

Write-Host "🔍 Running Code Coverage Analysis..." -ForegroundColor Green

# Clean previous coverage data
if (Test-Path $OutputDir) {
    Write-Host "🧹 Cleaning previous coverage data..." -ForegroundColor Yellow
    Remove-Item -Path $OutputDir -Recurse -Force
}

# Create output directory
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

try {
    # Run tests with coverage
    Write-Host "🧪 Running tests with coverage collection..." -ForegroundColor Cyan
    dotnet test tests/YouTubeCLI.Tests/YouTubeCLI.Tests.csproj `
        --configuration Release `
        --verbosity normal `
        --collect:"XPlat Code Coverage" `
        --results-directory "$OutputDir/test-results" `
        --logger "trx;LogFileName=test-results.trx" `
        --settings coverlet.runsettings

    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Tests failed!" -ForegroundColor Red
        exit 1
    }

    # Install ReportGenerator if not already installed
    Write-Host "📦 Installing ReportGenerator..." -ForegroundColor Cyan
    dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.2.0 | Out-Null

    # Find coverage files
    $coverageFiles = Get-ChildItem -Path "$OutputDir/test-results" -Recurse -Filter "coverage.cobertura.xml" | ForEach-Object { $_.FullName }

    if ($coverageFiles.Count -eq 0) {
        Write-Host "❌ No coverage files found!" -ForegroundColor Red
        exit 1
    }

    Write-Host "📊 Found $($coverageFiles.Count) coverage file(s)" -ForegroundColor Green

    # Generate HTML report
    Write-Host "📈 Generating coverage report..." -ForegroundColor Cyan
    $coverageFilesString = $coverageFiles -join ";"

    reportgenerator `
        -reports:"$coverageFilesString" `
        -targetdir:"$OutputDir/html-report" `
        -reporttypes:"Html;Cobertura;JsonSummary" `
        -assemblyfilters:"-*.Tests*" `
        -classfilters:"-*.Tests*" `
        -filefilters:"-*.Tests*" `
        -verbosity:Info

    # Display coverage summary
    $summaryFile = "$OutputDir/html-report/Summary.json"
    if (Test-Path $summaryFile) {
        $summary = Get-Content $summaryFile | ConvertFrom-Json

        Write-Host "`n📊 Coverage Summary:" -ForegroundColor Green
        Write-Host "===================" -ForegroundColor Green
        Write-Host "Line Coverage:   $([math]::Round($summary.linecoverage, 2))%" -ForegroundColor $(if ($summary.linecoverage -ge 80) { "Green" } else { "Yellow" })
        Write-Host "Branch Coverage: $([math]::Round($summary.branchcoverage, 2))%" -ForegroundColor $(if ($summary.branchcoverage -ge 70) { "Green" } else { "Yellow" })
        Write-Host "Total Lines:     $($summary.totallines)" -ForegroundColor White
        Write-Host "Covered Lines:   $($summary.coveredlines)" -ForegroundColor White

        # Coverage status
        $lineStatus = if ($summary.linecoverage -ge 80) { "✅" } else { "⚠️" }
        $branchStatus = if ($summary.branchcoverage -ge 70) { "✅" } else { "⚠️" }

        Write-Host "`n🎯 Coverage Status:" -ForegroundColor Green
        Write-Host "Line Coverage:   $lineStatus $(if ($summary.linecoverage -ge 80) { "Target met" } else { "Below target (80%)" })" -ForegroundColor White
        Write-Host "Branch Coverage: $branchStatus $(if ($summary.branchcoverage -ge 70) { "Target met" } else { "Below target (70%)" })" -ForegroundColor White
    }

    Write-Host "`n📁 Coverage Reports Generated:" -ForegroundColor Green
    Write-Host "HTML Report: $OutputDir/html-report/index.html" -ForegroundColor Cyan
    Write-Host "Raw Data:    $OutputDir/test-results/" -ForegroundColor Cyan

    # Open report if requested
    if ($OpenReport) {
        $htmlReport = "$OutputDir/html-report/index.html"
        if (Test-Path $htmlReport) {
            Write-Host "🌐 Opening coverage report in browser..." -ForegroundColor Cyan
            Start-Process $htmlReport
        }
    }

    Write-Host "`n✅ Coverage analysis complete!" -ForegroundColor Green

} catch {
    Write-Host "❌ Error during coverage analysis: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
