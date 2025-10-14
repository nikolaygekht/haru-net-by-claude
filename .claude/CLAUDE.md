# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# Project Overview

Haru.NET is a complete port of the Haru PDF library from C to .NET 8.0. The library provides comprehensive PDF generation capabilities with full international font support, including CJK languages. The project is ~95% complete and production-ready.

# Build & Test Commands

## Building
```bash
# Build the entire solution
dotnet build cs-src/Haru.sln

# Build only the library
dotnet build cs-src/Haru/Haru.csproj

# Build demos
dotnet build cs-src/Haru.Demos/BasicDemos.csproj
```

## Testing
```bash
# Run all tests (681 tests)
dotnet test cs-src/Haru.Test/Haru.Test.csproj

# Run a single test class
dotnet test cs-src/Haru.Test/Haru.Test.csproj --filter "FullyQualifiedName~HpdfFontTests"

# Run tests with detailed output
dotnet test cs-src/Haru.Test/Haru.Test.csproj -v normal
```

## Running Demos
```bash
# Run demo application (creates PDFs in ./pdfs folder)
dotnet run --project cs-src/Haru.Demos/BasicDemos.csproj
```

# Project Structure

```
cs-src/
├── Haru/                  # Main library
│   ├── Doc/               # Document, pages, catalog, annotations, outlines
│   ├── Font/              # Font support (Base14, TrueType, Type1, CID)
│   │   ├── CID/           # CJK font support (Japanese, Chinese, Korean)
│   │   ├── TrueType/      # TrueType font parsing and embedding
│   │   └── Type1/         # PostScript Type1 font support
│   ├── Objects/           # PDF primitive objects (Null, Boolean, Number, String, Name, Array, Dict, Stream)
│   ├── Streams/           # Stream handling and compression
│   ├── Images/            # Image support (PNG, JPEG)
│   ├── Encryption/        # RC4 and AES encryption
│   └── ...
├── Haru.Test/            # Unit tests (681 tests, xUnit + FluentAssertions)
└── Haru.Demos/           # Demo applications
    └── Resources/        # Fonts, images (auto-copied to output without "Resource" prefix)
```

# Architecture

## Font System
The font system uses a polymorphic design with `IHpdfFontImplementation` interface:
- **HpdfStandardFontImpl** - Base14 fonts (Times, Helvetica, Courier, etc.) with embedded metrics
- **HpdfTrueTypeFontImpl** - TrueType fonts (.ttf) with code page support (CP1251-1258)
- **HpdfType1FontImpl** - PostScript fonts with AFM/PFB parsing
- **HpdfCIDFont** - CJK fonts using Type 0 composite fonts (CP932, CP936, CP949, CP950)

All fonts support:
- Font metrics (ascent, descent, x-height, bounding box)
- Text measurement with word wrapping
- Custom encodings and ToUnicode CMaps

## PDF Object Model
The library uses a complete PDF object hierarchy rooted in `HpdfObject`:
- Primitive types: `HpdfNull`, `HpdfBoolean`, `HpdfNumber`, `HpdfString`, `HpdfName`
- Collections: `HpdfArray`, `HpdfDict`
- Streams: `HpdfStream` (base class with FlateDecode compression)

Documents are managed through:
- **HpdfDocument** - Main entry point, manages Xref table
- **HpdfCatalog** - Document catalog (root object)
- **HpdfPages** - Pages tree
- **HpdfPage** - Individual pages with content streams

## Extension Methods Pattern
The library uses extension methods to organize functionality:
- **HpdfPageGraphics** - Path operations, colors, transformations
- **HpdfPageShapes** - High-level shapes (circles, ellipses, arcs)
- **HpdfPageText** - Text operations
- **HpdfDocumentExtensions** - Document-level operations

# Dependencies

- **System.IO.Compression** - Replaces zlib for PDF stream compression
- **StbImageSharp** - PNG/JPEG image parsing (replaced SixLabors.ImageSharp)
- **System.Text.Encoding.CodePages** - Code page support for international fonts

Test dependencies:
- **xUnit** - Test framework
- **FluentAssertions 7.2.0** - Assertion library (exact version required)
- **Moq** - Mocking framework

# Code Quality Rules

## Non-Negotiable
- Apply SOLID principles
- Find root causes, no workarounds
- Consider the whole scenario, not just the problem point
- No performance anti-patterns
- Load non-changing data once, not repeatedly

## C to .NET Porting Guidelines
- Rely on .NET GC instead of explicit C-style memory management
- Replace C structs with classes/records as appropriate
- Use null-conditional operators instead of manual null checks
- Prefer `Span<T>` and `Memory<T>` for buffer operations
- Use `System.IO.Compression.DeflateStream` instead of zlib

# Session Management

- **LAST.md** - Contains current implementation status and progress. Read at session start, update before ending.
- **REMAINING.md** - Lists remaining work items. Read at session start, update before ending.
- **cs-src/Haru.Demos/CLAUDE.md** - Demo project information (resources, folder structure)

# Key Implementation Notes

## Resource Paths in Demos
Resources in `cs-src/Haru.Demos/Resources/` are auto-copied to output without "Resource" prefix:
- Source: `Resources/demo/ttfont/noto.ttf`
- Runtime: `demo/ttfont/noto.ttf`

Demo PDFs are saved to `./pdfs/` folder relative to the executable.

## PDF Version Management
The library automatically manages PDF versions:
- CID fonts require PDF 1.4+
- AES encryption requires PDF 1.6+
- AcroForms require PDF 1.5+

## Known Issues
- Signature fields work in Adobe Acrobat but not Chrome/Edge

# C Source Reference

Original Haru C library source is in `c-src/` folder. Use for reference when porting features or debugging compatibility issues.
