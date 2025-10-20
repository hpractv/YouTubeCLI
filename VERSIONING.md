# Versioning System

This project follows [Semantic Versioning (SemVer)](https://semver.org/) conventions for version management.

## Version Format

Versions follow the format: `MAJOR.MINOR.PATCH[-PRERELEASE]`

- **MAJOR**: Breaking changes that are not backward compatible
- **MINOR**: New features that are backward compatible
- **PATCH**: Bug fixes that are backward compatible
- **PRERELEASE**: Optional prerelease identifier (e.g., `1.0.0-beta.1`)

## Examples

- `1.0.0` - Initial release
- `1.0.1` - Bug fix
- `1.1.0` - New feature
- `2.0.0` - Breaking change
- `1.0.0-beta.1` - Prerelease version

## Updating Versions

### 🚀 Automated Version Management (Recommended)

Use the GitHub Actions workflow for automated version management:

1. **Go to Actions** → **Version Management** → **Run workflow**
2. **Select version type**:
   - `patch` - Bug fixes (1.0.0 → 1.0.1)
   - `minor` - New features (1.0.0 → 1.1.0)
   - `major` - Breaking changes (1.0.0 → 2.0.0)
   - `prerelease` - Pre-release versions (1.0.0 → 1.0.0-beta.1)
3. **Choose action**:
   - Create Pull Request (recommended)
   - Commit directly to current branch

### Using the PowerShell Script (Local)

Use the provided `update-version.ps1` script for local development:

```powershell
# Patch version (bug fixes)
./update-version.ps1 -Type patch

# Minor version (new features)
./update-version.ps1 -Type minor

# Major version (breaking changes)
./update-version.ps1 -Type major

# Prerelease version
./update-version.ps1 -Type prerelease -PrereleaseLabel "beta"
```

### Manual Updates

Edit the version in `src/YouTubeCLI.csproj`:

```xml
<PropertyGroup>
  <Version>1.0.0</Version>
  <AssemblyVersion>1.0.0.0</AssemblyVersion>
  <FileVersion>1.0.0.0</FileVersion>
  <InformationalVersion>1.0.0</InformationalVersion>
</PropertyGroup>
```

## 🔄 CI/CD Version Handling

The CI workflow automatically handles versioning:

### ✅ Version Validation
- **Semantic Versioning Format**: Validates `MAJOR.MINOR.PATCH[-PRERELEASE]` format
- **Pre-release Detection**: Identifies prerelease versions automatically
- **Tag Matching**: Ensures release tags match project version

### 🏷️ Release Naming
- **Pre-releases**: `v1.0.0-beta.1 Pre-release (PR #123)`
- **Releases**: `v1.0.0 Release`
- **Git Tags**: `v1.0.0-pr123` (pre-release) or `v1.0.0` (release)

### 📦 File Naming
All release files include version numbers:
- `ytc-windows-x64-1.0.0.zip`
- `ytc-macos-arm64-1.0.0.zip`
- `ytc-linux-x64-1.0.0.tar.gz`

## Release Files

When a release is created, the version is automatically appended to all release files:

- `ytc-windows-x64-1.0.0.zip`
- `ytc-windows-x86-1.0.0.zip`
- `ytc-macos-arm64-1.0.0.zip`
- `ytc-macos-x64-1.0.0.zip`
- `ytc-linux-x64-1.0.0.tar.gz`
- `ytc-linux-arm64-1.0.0.tar.gz`

## Release Names

- **Pre-releases**: `v1.0.0 Pre-release (PR #123)`
- **Releases**: `v1.0.0 Release`

## Git Tags

- **Pre-releases**: `v1.0.0-pr123`
- **Releases**: `v1.0.0`

## Version Display

The version is displayed when running the CLI with the `-v` or `--version` flag:

```bash
./ytc --version
# Output: 1.0.0
```
