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
- Install the .NET 10.0 rc1 or newer SDK to work with the project
- Linux, macOS, or Windows (cross-platform support)

**Note**: The `FastCdcFs.Net.Client` project is a Windows-only WPF GUI client. Agents typically do not need to build or work on this project, and it can be safely ignored on non-Windows platforms.

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
