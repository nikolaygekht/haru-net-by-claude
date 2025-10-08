# Building and Running Tests

## Prerequisites

Install the .NET SDK (version 6.0 or later recommended for .NET Standard 2.0 support):
- Windows: Download from https://dotnet.microsoft.com/download
- Linux: `sudo apt-get install dotnet-sdk-8.0` or similar
- macOS: `brew install dotnet`

## Build the Solution

From the project root directory:

```bash
# Restore dependencies
dotnet restore Haru.sln

# Build the entire solution
dotnet build Haru.sln

# Or build in Release mode
dotnet build Haru.sln -c Release
```

## Build Individual Projects

```bash
# Build Haru library
dotnet build cs-src/Haru/Haru.csproj

# Build Haru.Test project
dotnet build cs-src/Haru.Test/Haru.Test.csproj
```

## Run Tests

```bash
# Run all tests
dotnet test Haru.sln

# Run tests with detailed output
dotnet test Haru.sln --verbosity normal

# Run tests in a specific project
dotnet test cs-src/Haru.Test/Haru.Test.csproj

# Run tests with coverage (requires coverlet.collector)
dotnet test Haru.sln --collect:"XPlat Code Coverage"
```

## Run Specific Test Classes

```bash
# Run only PNG reader basic tests
dotnet test --filter "FullyQualifiedName~PngReaderBasicTests"

# Run only grayscale tests
dotnet test --filter "FullyQualifiedName~GrayscalePngTests"

# Run only palette tests
dotnet test --filter "FullyQualifiedName~PalettePngTests"

# Run only RGB tests
dotnet test --filter "FullyQualifiedName~RgbPngTests"

# Run only transparency tests
dotnet test --filter "FullyQualifiedName~TransparencyPngTests"
```

## Project Structure

```
claude3/
├── Haru.sln                          # Solution file
├── cs-src/
│   ├── Haru/                         # Main library project
│   │   ├── Haru.csproj
│   │   └── Png/                      # PNG facade
│   │       ├── IPngReader.cs
│   │       ├── ImageSharpPngReader.cs
│   │       ├── PngColorType.cs
│   │       ├── PngImageInfo.cs
│   │       ├── PngPalette.cs
│   │       ├── PngReaderFactory.cs
│   │       └── PngTransparency.cs
│   └── Haru.Test/                    # Test project
│       ├── Haru.Test.csproj
│       ├── TestHelper.cs             # Embedded resource helper
│       ├── Resources/                # Test PNG files (embedded)
│       │   ├── test_grayscale_2x2.png
│       │   ├── test_grayscale_alpha_2x2.png
│       │   ├── test_palette_2x2.png
│       │   ├── test_palette_trans_2x2.png
│       │   ├── test_rgb_2x2.png
│       │   └── test_rgba_2x2.png
│       └── Png/                      # PNG tests
│           ├── PngReaderBasicTests.cs
│           ├── GrayscalePngTests.cs
│           ├── PalettePngTests.cs
│           ├── RgbPngTests.cs
│           └── TransparencyPngTests.cs
└── c-src/                            # Original C source code

```

## Expected Test Results

All tests should pass once the .NET SDK is installed and the projects are built. The test suite includes:

- **Basic validation tests**: Signature validation, null checks, argument validation
- **Grayscale tests**: 8 tests for grayscale PNG functionality
- **Palette tests**: 7 tests for 256-color indexed PNG
- **RGB tests**: 8 tests for truecolor RGB PNG
- **Transparency tests**: 10+ tests for RGBA, grayscale+alpha, and palette+tRNS

## CI/CD Notes

The test resources are embedded in the assembly as `EmbeddedResource`, so the tests will work in CI/CD pipelines where only compiled binaries are present (no need to copy PNG files).

## Troubleshooting

If tests fail due to missing resources, check:
1. Resources are embedded: `<EmbeddedResource Include="Resources\**\*" />` in .csproj
2. Resource names match: `Haru.Test.Resources.<filename>`
3. Build the test project before running tests
