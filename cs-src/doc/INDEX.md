# Haru.NET Documentation

**Version 1.0 (Beta)**
A complete .NET 8.0 port of the Haru Free PDF Library

---

## Overview

Haru.NET is a comprehensive, cross-platform PDF generation library for .NET 8.0+. It provides low-level PDF primitives for creating professional-quality PDF documents without licensing fees or restrictions. The library offers full international font support, including CJK (Chinese, Japanese, Korean) languages, complete graphics capabilities, and extensive document security features.

### What is Haru.NET?

Haru.NET is a pure .NET implementation that allows developers to programmatically create PDF documents with complete control over:

- **Document Structure** - Pages, page sizes, layouts, and metadata
- **Typography** - Standard fonts, TrueType fonts, Type1 fonts, and CID fonts for international text
- **Graphics** - Vector graphics, paths, shapes, colors, transformations, and transparency
- **Images** - PNG and JPEG image embedding with positioning and scaling
- **Forms** - Interactive AcroForm fields (text fields, checkboxes, radio buttons, etc.)
- **Security** - RC4 and AES encryption with granular permissions
- **Navigation** - Bookmarks, annotations, and document outlines

### Target Audience

Haru.NET is designed for .NET developers who need:

- Cross-platform PDF generation without licensing restrictions
- Precise control over PDF structure and appearance
- Professional-quality document output
- International language support with complex scripts
- Lightweight library with minimal dependencies

For document-level layout and formatting, consider using **PDF.Flow** library which adds a high-level layout layer on top of Haru.NET.

---

## Quick Start

### Installation

Install via NuGet package:

```bash
dotnet add package Haru
```

**Requirements:**
- .NET 8.0 or later
- No additional prerequisites

**Dependencies:**
- StbImageSharp (permissive license)
- System.Text.Encoding.CodePages

### Your First PDF

```csharp
using Haru.Doc;
using Haru.Font;

// Create a new PDF document
var pdf = new HpdfDocument();

// Create a font
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

// Add a page (default: A4 portrait)
var page = pdf.AddPage();

// Write text
page.SetFontAndSize(font, 24);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Hello, PDF World!");
page.EndText();

// Draw a rectangle
page.SetLineWidth(2);
page.Rectangle(50, 50, 500, 200);
page.Stroke();

// Save the document
pdf.SaveToFile("hello.pdf");
```

---

## Documentation Structure

### Getting Started
- [Quick Start Guide](USAGE.md) - Installation and basic usage
- [Getting Started Tutorial](guides/GettingStarted.md) - Step-by-step first PDF
- [Library Architecture](STRUCTURE.md) - Design patterns and structure with UML diagrams
- [License Information](LICENSE.md) - MIT license details

### User Guides
- [Fonts Guide](guides/FontsGuide.md) - Working with Standard, TrueType, Type1, and CID/CJK fonts
- [Graphics Guide](guides/GraphicsGuide.md) - Drawing paths, shapes, colors, and transformations
- [Text Guide](guides/TextGuide.md) - Text rendering, positioning, and formatting
- [Images Guide](guides/ImagesGuide.md) - Loading and placing PNG/JPEG images
- [Forms Guide](guides/FormsGuide.md) - Creating interactive AcroForm fields
- [Encryption Guide](guides/EncryptionGuide.md) - Document security and permissions

### API Reference

#### Core Classes
- [HpdfDocument](api/core/HpdfDocument.md) - Main entry point for PDF creation
- [HpdfPage](api/core/HpdfPage.md) - Page manipulation and content
- [HpdfFont](api/core/HpdfFont.md) - Font wrapper for typography
- [HpdfImage](api/core/HpdfImage.md) - Image objects for embedding

#### Extension Methods
- [HpdfPageGraphics](api/extensions/HpdfPageGraphics.md) - Low-level graphics operations
- [HpdfPageShapes](api/extensions/HpdfPageShapes.md) - High-level shape drawing
- [HpdfPageText](api/extensions/HpdfPageText.md) - Text operations
- [HpdfPageExtensions](api/extensions/HpdfPageExtensions.md) - Convenience methods
- [HpdfDocumentExtensions](api/extensions/HpdfDocumentExtensions.md) - Document-level operations

#### Types and Enumerations
- [HpdfPageSize](api/types/HpdfPageSize.md) - Standard page sizes
- [HpdfRgbColor](api/types/HpdfRgbColor.md) - RGB color representation
- [HpdfRect](api/types/HpdfRect.md) - Rectangle structures

#### Forms and Annotations
- [HpdfAcroForm](api/forms/HpdfAcroForm.md) - Interactive form management
- [Form Fields](api/forms/) - TextField, Checkbox, RadioButton, etc.

### Code Examples
- [Basic Document Creation](examples/BasicDocument.md)
- [Working with Fonts](examples/WorkingWithFonts.md)
- [Drawing Graphics](examples/DrawingGraphics.md)

---

## Key Features

### Font System
- **Base14 Fonts** - Standard PDF fonts (Times, Helvetica, Courier, Symbol, ZapfDingbats)
- **TrueType Fonts** - Embed .ttf fonts with code page support (CP1251-1258)
- **Type1 Fonts** - PostScript fonts with AFM/PFB parsing
- **CID Fonts** - Complete CJK support (Japanese CP932, Chinese CP936/CP950, Korean CP949)
- **Font Metrics** - Accurate text measurement with word wrapping
- **Custom Encodings** - ToUnicode CMaps for international text

### Graphics Capabilities
- **Vector Graphics** - Lines, curves, Bézier paths, rectangles, circles, ellipses, arcs
- **Color Spaces** - RGB and CMYK color support
- **Transformations** - Rotation, scaling, translation, and arbitrary matrix transformations
- **Transparency** - Blend modes and graphics state management
- **Clipping Paths** - Complex clipping regions
- **Line Styles** - Width, dash patterns, cap styles, join styles

### Document Features
- **Page Management** - Multiple page sizes (A4, Letter, Legal, etc.) and orientations
- **Compression** - FlateDecode compression for efficient file sizes
- **Encryption** - RC4 and AES encryption with user/owner passwords
- **Permissions** - Granular control (print, copy, modify, etc.)
- **Metadata** - Title, author, subject, keywords, creation date
- **Navigation** - Outlines (bookmarks), annotations, links
- **PDF Versions** - Support for PDF 1.2 through 1.7

### Image Support
- **PNG** - Full PNG support including transparency
- **JPEG** - JPEG image embedding
- **Positioning** - Flexible image placement and scaling

### Interactive Forms (AcroForms)
- Text fields, checkboxes, radio buttons, combo boxes
- Signature fields (works in Adobe Acrobat)
- Field validation and formatting
- Requires PDF 1.5+

---

## Architecture Overview

### Main Entry Points

**HpdfDocument** - The primary class for creating PDF documents. Manages document lifecycle, page creation, font loading, image loading, encryption, and file saving.

**HpdfPage** - Represents individual pages. Provides access to page size, content streams, and serves as the target for all graphics and text operations.

### Extension Methods Pattern

Haru.NET organizes functionality using extension methods to keep the API clean and discoverable:

- **HpdfPageGraphics** - Path operations, colors, line styles, transformations
- **HpdfPageShapes** - High-level shapes (circles, ellipses, arcs)
- **HpdfPageText** - Text rendering, positioning, line spacing
- **HpdfDocumentExtensions** - Font loading, image loading, document settings

### Polymorphic Font System

The font system uses a unified interface (`IHpdfFontImplementation`) with specialized implementations:

```
HpdfFont (wrapper)
  └─ IHpdfFontImplementation
      ├─ HpdfStandardFontImpl    (Base14 fonts)
      ├─ HpdfTrueTypeFontImpl    (TrueType .ttf)
      ├─ HpdfType1FontImpl       (PostScript Type1)
      └─ HpdfCIDFont             (CJK composite fonts)
```

All fonts provide consistent text measurement, metrics, and rendering capabilities.

---

## Version Management

Haru.NET automatically manages PDF versions based on features used:

- **PDF 1.2** - Basic documents
- **PDF 1.4+** - Required for CID fonts (CJK support)
- **PDF 1.5+** - Required for AcroForms
- **PDF 1.6+** - Required for AES encryption

The library automatically upgrades the PDF version when you use features that require it.

---

## Font Licensing

**Important:** Fonts are copyrighted material. Ensure you have the right to embed fonts in your PDFs.

**Recommended font sources:**
- Reader-supported fonts (Helvetica, Times, Courier - no embedding needed)
- Free/open-source fonts from [Google Fonts](https://fonts.google.com/)
- Properly licensed commercial fonts

---

## Support and Contributing

### Reporting Issues
Report bugs and issues on GitHub: https://github.com/nikolaygekht/haru-net-by-claude/issues

### Contributing
1. Clone the repository
2. Make your changes
3. Create a pull request

### Building from Source
```bash
# Build the library
dotnet build cs-src/Haru/Haru.csproj

# Run tests (681 unit tests)
dotnet test cs-src/Haru.Test/Haru.Test.csproj

# Run demos (creates PDFs in ./pdfs folder)
dotnet run --project cs-src/Haru.Demos/BasicDemos.csproj
```

---

## License

Haru.NET is licensed under the MIT License:

```
Copyright (c) 1999-2006 Takeshi Kanno <takeshi_kanno@est.hi-ho.ne.jp>
Copyright (c) 2007-2009 Antony Dovgal <tony@daylessday.org>
Copyright (c) 2010-2025 Gehtsoft USA LLC <contact@gehtsoftusa.com>

Permission to use, copy, modify, distribute and sell this software
and its documentation for any purpose is hereby granted without fee,
provided that the above copyright notice appear in all copies and
that both that copyright notice and this permission notice appear
in supporting documentation. It is provided "as is" without express
or implied warranty.
```

See [LICENSE.md](LICENSE.md) for complete details.

---

## Project Status

**Current Version:** 1.0 (Beta)
**Completion:** ~95% production-ready
**Test Coverage:** 681 unit tests
**Platform:** .NET 8.0+ (Windows, Linux, macOS)

---

## Next Steps

1. Read the [Quick Start Guide](USAGE.md) for installation and basic usage
2. Follow the [Getting Started Tutorial](guides/GettingStarted.md) to create your first PDF
3. Explore the [User Guides](guides/) for specific features
4. Reference the [API Documentation](api/) for detailed class information

---

*Last updated: 2025-10-31*
