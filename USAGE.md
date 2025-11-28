# YouTube CLI Usage Guide

A comprehensive guide for using the YouTube CLI tool to manage YouTube broadcasts.

## Table of Contents

- [Installation and Prerequisites](#installation-and-prerequisites)
- [Getting Started](#getting-started)
- [Commands Overview](#commands-overview)
- [Command Details](#command-details)
- [Platform-Specific Usage](#platform-specific-usage)
- [Configuration Files](#configuration-files)
- [Troubleshooting](#troubleshooting)

## Installation and Prerequisites

### Prerequisites

Before using the YouTube CLI tool, ensure you have the following:

1. **YouTube Channel**: A YouTube channel with live streaming capabilities
2. **Google Cloud Project**: A Google Cloud project with YouTube Data API v3 enabled
3. **OAuth 2.0 Credentials**: Client secrets file from Google Cloud Console
4. **Broadcast Configuration**: JSON file containing your broadcast settings

### Installation

#### Option 1: Download Pre-built Executable

Download the latest release from the [GitHub Releases](https://github.com/hpractv/YouTubeCLI/releases) page:

- **Windows**: Download `ytc.exe` for Windows x64
- **macOS**: Download `ytc` for macOS (Intel or Apple Silicon)
- **Linux**: Download `ytc` for Linux x64

#### Option 2: Build from Source

1. **Prerequisites for Building**:
   - .NET 8.0 SDK
   - PowerShell (for Windows/macOS) or Bash (for Linux)

2. **Build Commands**:

   **Using PowerShell (Windows/macOS)**:
   ```powershell
   # Build for current platform
   pwsh build.ps1

   # Build for specific platform
   pwsh build.ps1 -Runtime win-x64    # Windows
   pwsh build.ps1 -Runtime osx-x64    # macOS Intel
   pwsh build.ps1 -Runtime osx-arm64   # macOS Apple Silicon
   pwsh build.ps1 -Runtime linux-x64  # Linux
   ```

   **Using dotnet CLI**:
   ```bash
   cd src
   dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained
   ```

3. **Executable Location**: The compiled executable will be in `src/bin/Release/net8.0/{runtime}/publish/`

### Google Cloud Setup

1. **Create a Google Cloud Project**:
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select an existing one

2. **Enable YouTube Data API v3**:
   - Navigate to "APIs & Services" > "Library"
   - Search for "YouTube Data API v3"
   - Click "Enable"

3. **Create OAuth 2.0 Credentials**:
   - Go to "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "OAuth 2.0 Client IDs"
   - Choose "Desktop application"
   - Download the JSON file (this is your client secrets file)

## Getting Started

### Basic Usage

```bash
# Show help
./ytc -h

# Show version
./ytc -v

# Get help for specific command
./ytc create -h
./ytc list -h
./ytc update -h
./ytc end -h
```

### First Time Setup

1. **Prepare your broadcast configuration file** (see [Configuration Files](#configuration-files) section)
2. **Run your first command** with the `--test-mode` flag to create a private broadcast for testing
3. **Authenticate** when prompted (browser will open for OAuth)

## Commands Overview

| Command  | Description               | Key Options                  |
| -------- | ------------------------- | ---------------------------- |
| `create` | Create YouTube broadcasts | `-f`, `-c`, `-u`, `-t`       |
| `list`   | List existing broadcasts  | `-f`, `-c`, `-u`, `-p`       |
| `update` | Update broadcast settings | `-y`, `-f`, `-a`, `-o`, `-p` |
| `end`    | End active broadcasts     | `-i`                         |

## Command Details

### Create Command

Creates YouTube broadcasts from a JSON configuration file.

```bash
./ytc create [options]
```

**Required Options**:
- `-u, --youtube-user <string>`: Your YouTube User ID
- `-c, --client-secrets <path>`: Path to Google OAuth client secrets file
- `-f, --file <path>`: Path to broadcast configuration JSON file

**Optional Options**:
- `-o, --occurences <int>`: Number of stream events to create (default: 1)
- `-i, --id <value>`: Create specific broadcasts by ID (comma-separated)
- `-s, --starts-on <value>`: Start date for broadcasts (format: MM/dd/yy). **Note**: Date cannot be before today's date. Past dates will result in an error.
- `-e, --output <int>`: Output file options (1=Single, 2=Monthly, 3=Daily, 4=Hourly, 5=Broadcast)
- `-l, --clear-credential`: Clear stored authentication credentials
- `-t, --test-mode`: Create first broadcast as private (for testing)

**Examples**:
```bash
# Create all broadcasts from config file
./ytc create -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.json"

# Create specific broadcasts in test mode
./ytc create -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.json" -i "broadcast1,broadcast2" -t

# Create broadcasts starting on specific date (must be today or in the future)
./ytc create -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.json" -s "01/15/24"

# Create with multiple occurrences and output options
./ytc create -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.json" -o 3 -e 2,3
```

### List Command

Lists existing YouTube broadcasts.

```bash
./ytc list [options]
```

**Required Options**:
- `-u, --youtube-user <string>`: Your YouTube User ID
- `-c, --client-secrets <path>`: Path to Google OAuth client secrets file
- `-f, --file <path>`: Path to broadcast configuration JSON file

**Optional Options**:
- `-s, --filter <value>`: Filter broadcasts by status. Options: `all` (default), `upcoming`, `active`, `completed`
- `-n, --limit <int>`: Limit the number of broadcasts returned. Default: 100. Results are sorted by most recent first (ScheduledStartTime descending).

**Output Format**:

Each broadcast entry displays the following information:
```
Broadcast Title (privacyStatus): broadcastUrl
```

**Note**: The privacy status (e.g., `public`, `private`, `unlisted`) is displayed in parentheses after the broadcast title, followed by the broadcast URL. This allows you to quickly identify the privacy setting for each broadcast. Results are sorted by most recent first (ScheduledStartTime descending).

**Examples**:
```bash
# List all broadcasts (default, limit 100)
./ytc list -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.json"

# List only upcoming broadcasts
./ytc list -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.json" -s upcoming

# List completed broadcasts with custom limit
./ytc list -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.json" -s completed -n 50

# List active broadcasts
./ytc list -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.json" -s active
```

### Update Command

Updates existing broadcast settings.

```bash
./ytc update [options]
```

**Required Options**:
- `-u, --youtube-user <string>`: Your YouTube User ID
- `-c, --client-secrets <path>`: Path to Google OAuth client secrets file

**Broadcast Selection** (choose one):
- `-y, --youtube-id <value>`: YouTube ID of broadcast to update
- `-f, --file <path>`: CSV file containing broadcast IDs

**Update Options**:
- `-a, --auto-start <value>`: Set auto-start (true/false)
- `-o, --auto-stop <value>`: Set auto-stop (true/false)
- `-p, --privacy <value>`: Set privacy (public/unlisted/private)

**Examples**:
```bash
# Update specific broadcast
./ytc update -u "your-youtube-user" -c "client_secrets.json" -y "broadcast_id" -a true -p public

# Update broadcasts from CSV file
./ytc update -u "your-youtube-user" -c "client_secrets.json" -f "broadcasts.csv" -a false
```

### End Command

Ends active YouTube broadcasts.

```bash
./ytc end [options]
```

**Required Options**:
- `-i, --id <value>`: Broadcast ID(s) to end (comma-separated for multiple)

**Examples**:
```bash
# End single broadcast
./ytc end -i "broadcast_id"

# End multiple broadcasts
./ytc end -i "broadcast_id1,broadcast_id2,broadcast_id3"
```

## Platform-Specific Usage

### Windows

**PowerShell**:
```powershell
# Run the executable
.\ytc.exe create -u "your-user" -c "client_secrets.json" -f "broadcasts.json"

# With test mode
.\ytc.exe create -u "your-user" -c "client_secrets.json" -f "broadcasts.json" -t
```

**Command Prompt**:
```cmd
ytc.exe create -u "your-user" -c "client_secrets.json" -f "broadcasts.json"
```

### macOS

**Terminal**:
```bash
# Make executable (if needed)
chmod +x ytc

# Run commands
./ytc create -u "your-user" -c "client_secrets.json" -f "broadcasts.json"

# With test mode
./ytc create -u "your-user" -c "client_secrets.json" -f "broadcasts.json" -t
```

**Using Homebrew** (if installed via package manager):
```bash
ytc create -u "your-user" -c "client_secrets.json" -f "broadcasts.json"
```

### Linux

```bash
# Make executable (if needed)
chmod +x ytc

# Run commands
./ytc create -u "your-user" -c "client_secrets.json" -f "broadcasts.json"

# With test mode
./ytc create -u "your-user" -c "client_secrets.json" -f "broadcasts.json" -t
```

## Configuration Files

### Broadcast Configuration JSON

Create a JSON file with your broadcast settings:

```json
{
  "items": [
    {
      "id": "broadcast1",
      "name": "My Weekly Stream",
      "description": "Weekly programming stream",
      "active": true,
      "title": "Weekly Programming Stream",
      "description": "Join me for weekly programming discussions",
      "thumbnail": "thumbnail.jpg",
      "privacy": "public",
      "autoStart": true,
      "autoStop": true,
      "startTime": "2024-01-15T19:00:00Z",
      "endTime": "2024-01-15T21:00:00Z"
    }
  ]
}
```

### Client Secrets File

Download from Google Cloud Console as `client_secrets.json`:

```json
{
  "installed": {
    "client_id": "your-client-id",
    "project_id": "your-project-id",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://oauth2.googleapis.com/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_secret": "your-client-secret",
    "redirect_uris": ["http://localhost"]
  }
}
```

## Troubleshooting

### Common Issues

1. **Authentication Errors**:
   - Ensure your client secrets file is valid
   - Clear credentials with `-l` flag and re-authenticate
   - Check that YouTube Data API v3 is enabled in Google Cloud Console

2. **File Not Found Errors**:
   - Use absolute paths for configuration files
   - Ensure files exist and are readable

3. **Permission Errors**:
   - Make sure your YouTube channel has live streaming enabled
   - Verify OAuth scopes include YouTube Data API access

4. **Start Date Validation Errors**:
   - If you receive "Start date cannot be before today's date" error:
     - Ensure the date provided with `-s` option is today or in the future
     - Check your system date is correct
     - Use format MM/dd/yy for the start date

5. **Build Issues**:
   - Ensure .NET 8.0 SDK is installed
   - Check that all dependencies are restored

### Getting Help

- Run `./ytc -h` for general help
- Run `./ytc [command] -h` for command-specific help
- Check the [GitHub Issues](https://github.com/hpractv/YouTubeCLI/issues) for known problems
- Review the [main README](README.md) for build and development information

### Logs and Debugging

The tool provides console output for all operations. For detailed debugging:

1. Use `--test-mode` flag for safe testing
2. Check console output for error messages
3. Verify your configuration files are valid JSON
4. Ensure all required parameters are provided
