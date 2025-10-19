# PowerShell script to update version following semantic versioning
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("major", "minor", "patch", "prerelease")]
    [string]$Type,

    [string]$PrereleaseLabel = "beta"
)

# Read current version from project file
$projectFile = "src/YouTubeCLI.csproj"
$content = Get-Content $projectFile -Raw

# Extract current version
$versionMatch = [regex]::Match($content, '<Version>([^<]+)</Version>')
if (-not $versionMatch.Success) {
    Write-Error "Could not find version in project file"
    exit 1
}

$currentVersion = $versionMatch.Groups[1].Value
Write-Host "Current version: $currentVersion"

# Parse version components
$versionParts = $currentVersion.Split('.')
$major = [int]$versionParts[0]
$minor = [int]$versionParts[1]
$patch = [int]$versionParts[2]

# Update version based on type
switch ($Type) {
    "major" {
        $major++
        $minor = 0
        $patch = 0
    }
    "minor" {
        $minor++
        $patch = 0
    }
    "patch" {
        $patch++
    }
    "prerelease" {
        # For prerelease, we don't increment the version numbers
        # but we add a prerelease identifier
        $newVersion = "$major.$minor.$patch-$prereleaseLabel.1"
    }
}

if ($Type -ne "prerelease") {
    $newVersion = "$major.$minor.$patch"
}

Write-Host "New version: $newVersion"

# Update the project file
$newContent = $content -replace '<Version>[^<]+</Version>', "<Version>$newVersion</Version>"
$newContent = $newContent -replace '<AssemblyVersion>[^<]+</AssemblyVersion>', "<AssemblyVersion>$newVersion.0</AssemblyVersion>"
$newContent = $newContent -replace '<FileVersion>[^<]+</FileVersion>', "<FileVersion>$newVersion.0</FileVersion>"
$newContent = $newContent -replace '<InformationalVersion>[^<]+</InformationalVersion>', "<InformationalVersion>$newVersion</InformationalVersion>"

Set-Content $projectFile -Value $newContent -NoNewline

Write-Host "Version updated to: $newVersion"
Write-Host "Updated project file: $projectFile"

# Show the changes
Write-Host "`nChanges made:"
Write-Host "- Version: $currentVersion -> $newVersion"
Write-Host "- AssemblyVersion: $newVersion.0"
Write-Host "- FileVersion: $newVersion.0"
Write-Host "- InformationalVersion: $newVersion"
