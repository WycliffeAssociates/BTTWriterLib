# BTTWriterLib

A .NET Standard 2.0 library for reading and processing files from Bible Translation Tools Writer (BTTWriter/translationStudio). This library provides utilities to load translation projects and convert them to USFM format.

## Overview

BTTWriterLib enables you to:
- Load Bible translation projects from BTTWriter/translationStudio archive files (`.tstudio`)
- Load projects directly from file system directories
- Parse project manifests and translation chunks
- Generate USFM documents from translation data
- Filter content based on completion status

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package BTTWriterLib
```

Or via Package Manager Console:

```powershell
Install-Package BTTWriterLib
```

## Usage

### Loading from a File System Directory

```csharp
using BTTWriterLib;
using USFMToolsSharp;

// Create a resource container from a directory
var container = new FileSystemResourceContainer("/path/to/project/directory");

// Get the manifest
var manifest = container.GetManifest();
Console.WriteLine($"Project: {manifest.project.name} ({manifest.project.id})");

// Get all files (including incomplete chunks)
var allFiles = container.GetFiles(onlyFinished: false);

// Get only completed chunks
var finishedFiles = container.GetFiles(onlyFinished: true);

// Get content of a specific chunk (format: "chapter-chunk", e.g., "01-01")
string chunkContent = container.GetFile("01-01");

// Create a USFM document from the container
USFMDocument usfmDoc = BTTWriterLoader.CreateUSFMDocumentFromContainer(
    container, 
    onlyComplete: false
);
```

### Loading from a .tstudio Archive File

```csharp
using BTTWriterLib;

// Load from a .tstudio or .bttwriter archive
using (var fileLoader = new TStudioFileLoader("/path/to/project.tstudio"))
{
    // Get the manifest
    var manifest = fileLoader.GetManifest();
    
    // Get files
    var files = fileLoader.GetFiles(onlyFinished: false);
    
    // Create USFM document
    var usfmDoc = BTTWriterLoader.CreateUSFMDocumentFromContainer(
        fileLoader, 
        onlyComplete: true
    );
    
    // Export to USFM string
    string usfmContent = usfmDoc.ToString();
}
```

### Working with USFM Documents

```csharp
using BTTWriterLib;
using USFMToolsSharp;
using USFMToolsSharp.Models.Markers;

var container = new FileSystemResourceContainer("/path/to/project");

// Create USFM document with custom parser configuration
var customParser = new USFMParser(new List<string> { "s5", "fqa*" });
var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(
    container, 
    onlyComplete: false,
    parser: customParser
);

// Access USFM markers
var chapters = document.GetChildMarkers<CMarker>();
foreach (var chapter in chapters)
{
    Console.WriteLine($"Chapter {chapter.Number}");
}

// Convert to USFM string
string usfm = document.ToString();
```

### Accessing Manifest Information

```csharp
using BTTWriterLib;
using BTTWriterLib.Models;

var container = new FileSystemResourceContainer("/path/to/project");
var manifest = container.GetManifest();

// Access project information
Console.WriteLine($"Project ID: {manifest.project.id}");
Console.WriteLine($"Project Name: {manifest.project.name}");
Console.WriteLine($"Target Language: {manifest.target_language.name}");
Console.WriteLine($"Format: {manifest.format}");

// Check translators
foreach (var translator in manifest.translators)
{
    Console.WriteLine($"Translator: {translator}");
}

// Check finished chunks
foreach (var chunk in manifest.finished_chunks)
{
    Console.WriteLine($"Finished: {chunk}");
}
```

## Building from Source

### Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) or later
- Git

### Build Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/WycliffeAssociates/BTTWriterLib.git
   cd BTTWriterLib
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Build in Release mode:
   ```bash
   dotnet build -c Release
   ```

### Running Tests

Run all tests:
```bash
dotnet test
```

Run tests with detailed output:
```bash
dotnet test --verbosity normal
```

Run tests without rebuilding:
```bash
dotnet test --no-build
```

## Contributing

We welcome contributions to BTTWriterLib! Here's how you can help:

### Getting Started

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/BTTWriterLib.git
   cd BTTWriterLib
   ```
3. Create a branch for your changes:
   ```bash
   git checkout -b feature/your-feature-name
   ```

### Development Workflow

1. Make your changes in your feature branch
2. Add tests for any new functionality
3. Ensure all tests pass:
   ```bash
   dotnet test
   ```
4. Build the project to verify:
   ```bash
   dotnet build
   ```
5. Commit your changes with clear, descriptive commit messages:
   ```bash
   git commit -m "Add feature: description of your changes"
   ```

### Submitting Changes

1. Push your changes to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```
2. Open a Pull Request against the `develop` branch
3. Ensure your PR description clearly describes the problem and solution
4. Link any relevant issues in your PR description

### Code Style

- Follow existing code conventions in the project
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and concise

### Testing Guidelines

- Write unit tests for all new functionality
- Ensure tests are independent and can run in any order
- Use descriptive test method names that explain what is being tested
- Follow the existing test patterns in `BTTWriterLibTests`

### Reporting Issues

If you find a bug or have a feature request:
1. Check if the issue already exists in the [issue tracker](https://github.com/WycliffeAssociates/BTTWriterLib/issues)
2. If not, create a new issue with:
   - A clear, descriptive title
   - Steps to reproduce (for bugs)
   - Expected vs actual behavior
   - Your environment details (.NET version, OS, etc.)

## License

This project is licensed under the terms specified in the [LICENSE](LICENSE) file.

Copyright 2025 Wycliffe Associates

## Project Information

- **Repository**: [https://github.com/WycliffeAssociates/BTTWriterLib](https://github.com/WycliffeAssociates/BTTWriterLib)
- **NuGet Package**: [BTTWriterLib](https://www.nuget.org/packages/BTTWriterLib/)
- **Target Framework**: .NET Standard 2.0
- **Version**: 0.10.1
