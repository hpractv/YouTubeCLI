# YouTubeCLI

[![Build Status](https://github.com/hpractv/YouTubeCLI/workflows/Build%20and%20Release/badge.svg)](https://github.com/hpractv/YouTubeCLI/actions)
[![Code Coverage](https://codecov.io/gh/hpractv/YouTubeCLI/branch/main/graph/badge.svg)](https://codecov.io/gh/hpractv/YouTubeCLI)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A command line interface for building and managing YouTube broadcasts.

## 📊 Code Coverage

This project maintains comprehensive test coverage with detailed reports available for each build:

- **Line Coverage**: Measures the percentage of code lines executed during tests
- **Branch Coverage**: Measures the percentage of conditional branches tested
- **Coverage Reports**: Available as artifacts in GitHub Actions and uploaded to Codecov

### Coverage Features

- ✅ **134 Unit Tests** covering all commands and edge cases
- ✅ **Automated Coverage Reports** generated on every build
- ✅ **PR Coverage Comments** showing coverage impact
- ✅ **Coverage Artifacts** with detailed HTML reports
- ✅ **Codecov Integration** for historical coverage tracking

## 🏷️ Versioning

This project follows [Semantic Versioning (SemVer)](https://semver.org/) conventions:

- **Current Version**: `1.0.0`
- **Version Display**: Run `./ytc --version` to see the current version
- **Release Files**: All release files include the version number (e.g., `ytc-windows-x64-1.0.0.zip`)
- **Version Management**: Use `./update-version.ps1` script for version updates

See [VERSIONING.md](VERSIONING.md) for detailed versioning information.

## Build

The project can be built as a single, self-contained executable using the provided PowerShell script.

### Using the Build Script

```powershell
# Build for current platform (macOS osx-arm64 by default)
pwsh build.ps1

# Build for macOS on Intel explicitly
pwsh build.ps1 -Runtime osx-x64

# Build for Windows
pwsh build.ps1 -Runtime win-x64

# Build for Linux
pwsh build.ps1 -Runtime linux-x64

# Clean build
pwsh build.ps1 -Clean

# Debug build
pwsh build.ps1 -Configuration Debug
```



### Manual Build

Alternatively, you can build manually using dotnet CLI:

```bash
# Navigate to src directory
cd src

# Build for macOS
dotnet publish -r osx-x64 -p:PublishSingleFile=true --self-contained

# Build for Windows
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained

# Build for Linux
dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained
```

> **Parameter explanations:**
>
> - `-r <RID>` or `--runtime <RID>`: Target runtime identifier for the build. Common RIDs include:
>    - `osx-x64`: macOS (64-bit Intel)
>    - `win-x64`: Windows (64-bit Intel)
>    - `linux-x64`: Linux (64-bit Intel)
> - `-p:PublishSingleFile=true`: Publishes the application as a single file executable.
> - `--self-contained`: Publishes the application with its own .NET runtime, so it doesn't require .NET to be pre-installed on the machine.


The compiled executable will be located in `src/bin/Release/net8.0/{runtime}/publish/`

## Usage

```bash
# Show help
./ytc -h

# Show version
./ytc -v
```

For detailed command usage, run `ytc [command] -h` for specific command help. You can also get more usage instructions [here](USAGE.md).

## 🧪 Running Tests and Coverage

### Local Coverage Analysis

You can run code coverage analysis locally using the provided scripts:

#### PowerShell (Windows/macOS)
```powershell
# Basic coverage analysis
.\run-coverage.ps1

# Custom output directory
.\run-coverage.ps1 -OutputDir "./my-coverage"

# Open report in browser
.\run-coverage.ps1 -OpenReport
```

#### Bash (Linux/macOS)
```bash
# Basic coverage analysis
./run-coverage.sh

# Custom output directory
./run-coverage.sh --output ./my-coverage

# Open report in browser
./run-coverage.sh --open
```

### Manual Coverage Commands

```bash
# Run tests with coverage
dotnet test tests/YouTubeCLI.Tests/YouTubeCLI.Tests.csproj \
  --configuration Release \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage-results

# Generate HTML report (requires ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator \
  -reports:"./coverage-results/**/coverage.cobertura.xml" \
  -targetdir:"./coverage-report" \
  -reporttypes:"Html"
```

### Coverage Reports

- **HTML Report**: Interactive coverage report with line-by-line details
- **Cobertura XML**: Machine-readable coverage data
- **JSON Summary**: Coverage metrics in JSON format
- **GitHub Actions**: Automated coverage reports on every build
- **Codecov**: Historical coverage tracking and PR comments
