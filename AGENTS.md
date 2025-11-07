# Agent Development Guide

## Project Overview

FastCdcFS.Net is a .NET library for creating read-only file systems backed by fast content-defined chunking. It provides efficient storage, deduplication, and retrieval of files and directories.

## Project Structure

- `FastCdcFs.Net/` - Core library implementing the file system
- `FastCdcFs.Net.Shell/` - Command-line tool for working with file systems
- `FastCdcFs.Net.Client/` - WPF GUI client (Windows-only)
- `Benchmark/` - Performance benchmarking tool
- `Tests/` - Unit tests using xUnit
- `Test/` - Additional test project

## Build Requirements

- .NET SDK 8.0 or higher (projects target net8.0 and net10.0)
- .NET 10.0 SDK (RC2 or newer) is required to build the net10.0 target framework
- Linux, macOS, or Windows (cross-platform support)

**Note**: The `FastCdcFs.Net.Client` project is a Windows-only WPF GUI client. Agents typically do not need to build or work on this project, and it can be safely ignored on non-Windows platforms.

## Installing .NET 10 SDK

The project requires .NET 10 SDK (RC2 or newer) to build the net10.0 target framework. Here are several methods to install it:

### Method 1: Using the dotnet-install script (Recommended)

A `dotnet-install.sh` script is included in the repository for convenience:

```bash
# On Linux/macOS
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 10.0 --install-dir /usr/share/dotnet

# For local user installation (no sudo required)
./dotnet-install.sh --channel 10.0 --install-dir ~/.dotnet
export PATH="$HOME/.dotnet:$PATH"
```

On Windows, use PowerShell:
```powershell
# Download and run the Windows install script
Invoke-WebRequest -Uri https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1
./dotnet-install.ps1 -Channel 10.0 -InstallDir $env:ProgramFiles\dotnet
```

### Method 2: Using Package Managers

**Ubuntu/Debian:**
```bash
# Add Microsoft package repository (if not already added)
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET 10 SDK (when available in stable repos)
sudo apt update
sudo apt install dotnet-sdk-10.0
```

**macOS (using Homebrew):**
```bash
brew install --cask dotnet-sdk@10
```

**Windows (using winget):**
```powershell
winget install Microsoft.DotNet.SDK.10
```

### Method 3: Manual Download

Download the .NET 10 SDK directly from Microsoft:
- Visit: https://dotnet.microsoft.com/download/dotnet/10.0
- Download the appropriate installer for your operating system
- Run the installer and follow the prompts

### Verifying Installation

After installation, verify that .NET 10 SDK is available:

```bash
dotnet --list-sdks
```

You should see version `10.0.x` in the list. Then verify the project builds:

```bash
dotnet build FastCdcFs.Net -c Release
```

Both `net8.0` and `net10.0` target frameworks should build successfully.

## Building the Project

```bash
# Build core projects (cross-platform)
dotnet build FastCdcFs.Net
dotnet build FastCdcFs.Net.Shell

# Build all projects including tests
dotnet build --no-incremental
```

**Note**: Building the entire solution with `dotnet build` will fail on non-Windows platforms due to the WPF client project. This is expected and can be ignored. Focus on the core library and shell tool which are the primary development targets for agents.

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test Tests/Tests.csproj
```

## Running Benchmarks

The `Benchmark` project demonstrates the deduplication and compression capabilities of the FastCDC chunking algorithm:

```bash
# Run the benchmark
dotnet run --project Benchmark
```

The benchmark:
- Generates test data with intentional duplication patterns:
  - Binary files: 20 files with 4MB of shared data and 1MB unique data each
  - Text files: 100 HTML files with similar structure and embedded SVG content
- Builds FastCdcFS archives from the test data
- Measures and reports compression ratios showing the effectiveness of content-defined chunking and deduplication
- Outputs original size, archive size, and compression ratio for each dataset

This is useful for understanding how FastCdcFS performs with different types of data and validating that deduplication is working correctly.

## Packaging

The `build.cmd` script handles testing and packaging:
- Runs all tests first
- Packages `FastCdcFs.Net` and `FastCdcFs.Net.Shell` as NuGet packages

## Key Technologies

- **Content-Defined Chunking**: FastCDC algorithm for deduplication
- **Compression**: Zstandard (ZstdSharp.Port) for chunk compression
- **Hashing**: xxHash64 (System.IO.Hashing) for chunk identification
- **CLI Parsing**: CommandLineParser for shell tool

## Development Notes

- The project uses modern C# with nullable reference types enabled
- File system format is documented in README.md
- Core library has multi-targeting for .NET 8.0 and .NET 10.0
- Tests use xUnit framework with representative test data generation
