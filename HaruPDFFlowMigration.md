# Haru P/Invoke to C# Port Migration Plan for PDF.Flow Library

## Recent Updates (January 2025)

**Major Features Completed:**

1. ✅ **JPEG Image Support** - Full implementation
   - Direct JPEG embedding with DCTDecode filter
   - Supports RGB, Grayscale, and CMYK color spaces
   - Zero re-compression, preserves quality and size
   - Uses StbImageSharp (replaced ImageSharp dependency)

2. ✅ **Compression Mode** - Fully functional
   - Page content stream compression with FlateDecode
   - Achieves >50% size reduction on text-heavy PDFs
   - Modes: None, Text, Image, Metadata, All
   - 19 tests (10 unit + 9 integration) all passing

3. ✅ **Image Library Migration**
   - Replaced SixLabors.ImageSharp with StbImageSharp
   - Pure .NET, no dependencies, public domain license
   - Supports both PNG and JPEG formats
   - Lighter weight and more suitable for library use

4. ✅ **Font Metrics and MeasureText** - Fully implemented
   - GetAscent(), GetDescent(), GetXHeight(), GetBBox() for all font types
   - Accurate character width data ported from C source
   - MeasureText() with proper scaling from glyph space to user space
   - Works with Standard, TrueType, Type1, and CID fonts

5. ✅ **Font Architecture Refactoring** - Clean interface-based design
   - Created IHpdfFontImplementation interface
   - All font types (Standard, TrueType, Type1, CID) implement unified interface
   - Eliminated runtime null-checking chains with polymorphic design
   - HpdfFont now uses single implementation field instead of separate font type fields
   - Better maintainability, performance, and extensibility

6. ✅ **Text Wrapping Support** - Complete word wrapping implementation
   - HpdfFontExtensions.MeasureText with full word wrapping support
   - Compatible with libharu's HPDF_Font_MeasureText API
   - Supports word wrap mode and character-by-character breaking
   - Character spacing and word spacing calculations
   - Line feed detection
   - 12 comprehensive unit tests
   - TextWrappingDemo for visual demonstration

7. ✅ **Image Loading from Memory** - Complete implementation (2025-01-14)
   - LoadPngImageFromMem() - Load PNG from byte array
   - LoadRawImageFromMem() - Load raw image data (DeviceGray, DeviceRgb, DeviceCmyk)
   - 14 comprehensive unit tests covering all scenarios
   - Full validation and error handling

8. ✅ **Page Size Convenience Methods** - API compatibility (2025-01-14)
   - SetHeight() and SetWidth() extension methods
   - Matches p/invoke API signature expectations
   - Maintains compatibility while keeping property-based API

9. ✅ **PdfPig Integration Tests** - Third-party validation (2025-01-14)
   - 14 integration tests using PdfPig library for independent PDF validation
   - Tests for text extraction, image extraction, page sizes, compression
   - Ensures PDFs are readable by external tools (not just our own parser)
   - Caught and fixed empty page bug (StreamToken vs ObjectToken issue)

10. ✅ **PDF Spec Compliance Fix** - Empty page bug (2025-01-14)
    - Fixed HpdfStreamObject to always write stream/endstream section
    - Empty streams now correctly recognized as StreamToken (not ObjectToken)
    - Ensures compatibility with strict PDF parsers like PdfPig

**Overall Progress: ~97% Complete** (up from ~95%)

## Executive Summary

This document outlines the migration strategy for replacing the native p/invoke facade (`Haru.Net/Haru.Net/hpdf.cs`) with the pure C# port (`Haru.Net/cs-src`) in the PDF.Flow library. The migration will eliminate native library dependencies while maintaining full API compatibility.

**Current Architecture:**
- `LibharuDocument.cs` implements `INativeDocument` interface
- `LibharuGraphics.cs` extends `Graphics` base class
- Both classes use HPdf namespace (p/invoke wrapper around libharu23 native library)

**Target Architecture:**
- Replace HPdf namespace usage with Haru namespace (pure C# implementation)
- Maintain same INativeDocument interface
- Maintain same Graphics base class abstraction

## Current Usage Analysis

### LibharuDocument Usage (Core/LibharuDocument.cs)

The `LibharuDocument` class uses the following HPdf APIs:

| Current P/Invoke API | Usage | Line Reference |
|---------------------|-------|----------------|
| `HPdfDoc` constructor | Document creation with error handler | 34 |
| `HPdfDoc.HasDoc()` | Check document validity | 32 |
| `HPdfDoc.SetCompressionMode()` | Set PDF compression | 35 |
| `HPdfDoc.UseUTFEncodings()` | Enable UTF-8 encoding | 39 |
| `HPdfDoc.SaveToFile()` | Save PDF to file | 59 |
| `HPdfDoc.SaveToStream()` | Save PDF to stream | 64 |
| `HPdfDoc.FreeDocAll()` | Free document resources | 69, 112 |
| `HPdfDoc.LoadTTFontFromFile()` | Load TrueType font | 81 |
| `HPdfDoc.AddPage()` | Add new page | 89 |
| `HPdfPage.SetHeight()` | Set page height | 90 |
| `HPdfPage.SetWidth()` | Set page width | 91 |
| `HPdfDoc.GetPageByIndex()` | Get page by index | 40 (via LibharuGraphics) |

### LibharuGraphics Usage (Rendering/Native/LibharuGraphics.cs)

The `LibharuGraphics` class uses extensive HPdf APIs:

#### Document Operations
| Current P/Invoke API | Usage | Line Reference |
|---------------------|-------|----------------|
| `HPdfDoc.GetPageByIndex()` | Get page for rendering | 40 |
| `HPdfDoc.LoadTTFontFromFile()` | Load TrueType font | 71, 111 |
| `HPdfDoc.LoadType1FontFromFile()` | Load Type1 font (.afm/.pfb) | 84 |
| `HPdfDoc.SetCurrentEncoder()` | Set encoder (UTF-8, FontSpecific) | 91, 731 |
| `HPdfDoc.GetFont()` | Get font instance | 239, 462, 514, 521, 535 |
| `HPdfDoc.UseCNSFonts()/UseCNSEncodings()` | Chinese Simplified support | 189-190 |
| `HPdfDoc.UseCNTFonts()/UseCNTEncodings()` | Chinese Traditional support | 201-202 |
| `HPdfDoc.UseJPFonts()/UseJPEncodings()` | Japanese support | 213-214 |
| `HPdfDoc.UseKRFonts()/UseKREncodings()` | Korean support | 225-226 |
| `HPdfDoc.LoadJpegImageFromFile()` | Load JPEG image | 661 |
| `HPdfDoc.LoadPngImageFromFile()` | Load PNG image | 666 |
| `HPdfDoc.LoadPngImageFromMem()` | Load PNG from memory | 631 |
| `HPdfDoc.LoadRawImageFromMem()` | Load raw grayscale image | 626 |
| `HPdfDoc.GetPageMode()` | Get page mode | 862 |
| `HPdfDoc.SetPageMode()` | Set page mode (outline) | 863 |
| `HPdfDoc.CreateOutline()` | Create outline/bookmark | 872 |

#### Page Operations
| Current P/Invoke API | Usage | Line Reference |
|---------------------|-------|----------------|
| `HPdfPage.GetHeight()` | Get page height | 310, 318, 604, 700, 718, 842, 876 |
| `HPdfPage.MoveTo()` | Move to position | 250 |
| `HPdfPage.LineTo()` | Draw line | 304, 312 |
| `HPdfPage.Stroke()` | Stroke path | 305, 313, 599 |
| `HPdfPage.SetLineCap()` | Set line cap style | 257, 262, 269 |
| `HPdfPage.SetDash()` | Set dash pattern | 258, 263, 270 |
| `HPdfPage.SetRGBStroke()` | Set stroke RGB color | 286 |
| `HPdfPage.SetRGBFill()` | Set fill RGB color | 295, 621 |
| `HPdfPage.SetLineWidth()` | Set line width | 298 |
| `HPdfPage.Rectangle()` | Draw rectangle | 597, 613 |
| `HPdfPage.Fill()` | Fill path | 619 |
| `HPdfPage.GetRGBFill()` | Get current fill color | 609 |
| `HPdfPage.BeginText()` | Begin text object | 722 |
| `HPdfPage.EndText()` | End text object | 724 |
| `HPdfPage.MoveTextPos()` | Move text position | 726 |
| `HPdfPage.SetFontAndSize()` | Set font and size | 744 |
| `HPdfPage.GetCurrentFont()` | Get current font | 740, 742 |
| `HPdfPage.GetCurrentFontSize()` | Get current font size | 741 |
| `HPdfPage.ShowText()` | Show text (string) | 754, 830 |
| `HPdfPage.SetWordSpace()` | Set word spacing | 589 |
| `HPdfPage.MeasureText()` | Measure text width | 463, 468, 478 |
| `HPdfPage.TextWidth()` | Get text width | 502, 506 |
| `HPdfPage.DrawImage()` | Draw image | 701, 708, 719 |
| `HPdfPage.GSave()` | Save graphics state | 833 |
| `HPdfPage.GRestore()` | Restore graphics state | 835 |
| `HPdfPage.Concat()` | Concatenate matrix | 837 |
| `HPdfPage.CreateURILinkAnnot()` | Create URI link | 843 |
| `HPdfPage.CreateLinkAnnot()` | Create internal link | 897 |
| `HPdfPage.CreateDestination()` | Create destination | 874 |

#### Font Operations
| Current P/Invoke API | Usage | Line Reference |
|---------------------|-------|----------------|
| `HPdfFont.GetFontName()` | Get PostScript font name | 740, 742 |
| `HPdfFont.GetEncodingName()` | Get encoding name | 742 |
| `HPdfFont.MeasureText()` | Measure text for wrapping | 463 |
| `HPdfFont.GetAscent()` | Get font ascent | 515 |
| `HPdfFont.GetDescent()` | Get font descent | 522 |
| `HPdfFont.GetXHeight()` | Get x-height | 529 |
| `HPdfFont.GetBBox()` | Get font bounding box | 536 |

#### Image Operations
| Current P/Invoke API | Usage | Line Reference |
|---------------------|-------|----------------|
| `HPdfImage.GetWidth()` | Get image width | 680, 691 |
| `HPdfImage.GetHeight()` | Get image height | 681, 692 |

#### Outline/Bookmark Operations
| Current P/Invoke API | Usage | Line Reference |
|---------------------|-------|----------------|
| `HPdfOutline.SetOpened()` | Set outline opened state | 873 |
| `HPdfOutline.SetDestination()` | Set outline destination | 877 |

#### Destination Operations
| Current P/Invoke API | Usage | Line Reference |
|---------------------|-------|----------------|
| `HPdfDestination.SetXYZ()` | Set XYZ destination | 876 |

#### Annotation Operations
| Current P/Invoke API | Usage | Line Reference |
|---------------------|-------|----------------|
| `HPdfAnnotation.SetBorderStyle()` | Set annotation border | 852, 898 |

## C# Port API Mapping

### Document Level Mapping

| HPdf P/Invoke API | C# Port Equivalent | Status | Notes |
|------------------|-------------------|--------|-------|
| `new HPdfDoc(errorHandler)` | `new HpdfDocument()` | ✅ Available | Error handling integrated, no separate handler needed |
| `HPdfDoc.HasDoc()` | Check `HpdfDocument != null` | ✅ Available | Use null check instead |
| `HPdfDoc.SetCompressionMode()` | `HpdfDocument.SetCompressionMode()` | ✅ Available | Fully implemented with FlateDecode compression |
| `HPdfDoc.UseUTFEncodings()` | N/A | ⚠️ Not Needed | C# port handles encoding differently |
| `HPdfDoc.SaveToFile()` | `HpdfDocument.SaveToFile()` | ✅ Available | |
| `HPdfDoc.SaveToStream()` | `HpdfDocument.Save(Stream)` | ✅ Available | |
| `HPdfDoc.FreeDocAll()` | Automatic (GC) | ✅ Available | No manual cleanup needed |
| `HPdfDoc.AddPage()` | `HpdfDocument.AddPage()` | ✅ Available | Returns HpdfPage |
| `HPdfDoc.GetPageByIndex()` | `HpdfDocument.Pages[index]` | ✅ Available | Use property indexer |
| `HPdfDoc.LoadTTFontFromFile()` | `HpdfTrueTypeFont.LoadFromFile()` | ❌ Missing | Needs wrapper only |
| `HPdfDoc.LoadType1FontFromFile()` | `HpdfType1Font.LoadFromFile()` | ❌ Missing | Needs wrapper only |
| `HPdfDoc.GetFont()` | `HpdfDocument.GetFont()` | ✅ Available | Extension method |
| `HPdfDoc.SetCurrentEncoder()` | N/A | ⚠️ Not Needed | Fonts have built-in encoding |
| `HPdfDoc.UseCNSFonts()/Encodings()` | CID font support | ❌ Missing | Needs wrapper only |
| `HPdfDoc.UseCNTFonts()/Encodings()` | CID font support | ❌ Missing | Needs wrapper only |
| `HPdfDoc.UseJPFonts()/Encodings()` | CID font support | ❌ Missing | Needs wrapper only |
| `HPdfDoc.UseKRFonts()/Encodings()` | CID font support | ❌ Missing | Needs wrapper only |
| `HPdfDoc.LoadJpegImageFromFile()` | `HpdfImage.LoadJpegImageFromFile()` | ✅ Available | Full JPEG support with DCTDecode |
| `HPdfDoc.LoadPngImageFromFile()` | `HpdfImage.LoadPngImageFromFile()` | ✅ Available | Extension method |
| `HPdfDoc.LoadPngImageFromMem()` | `HpdfImage.LoadPngImageFromMem()` | ✅ Available | Implemented 2025-01-14 |
| `HPdfDoc.LoadRawImageFromMem()` | `HpdfImage.LoadRawImageFromMem()` | ✅ Available | Implemented 2025-01-14 |
| `HPdfDoc.GetPageMode()` | `HpdfCatalog.PageMode` | ✅ Available | Via Catalog property |
| `HPdfDoc.SetPageMode()` | `HpdfCatalog.SetPageMode()` | ✅ Available | Via Catalog property |
| `HPdfDoc.CreateOutline()` | `HpdfDocument.CreateOutline()` | ✅ Available | |

### Page Level Mapping

| HPdf P/Invoke API | C# Port Equivalent | Status | Notes |
|------------------|-------------------|--------|-------|
| `HPdfPage.SetHeight()` | `HpdfPage.SetHeight()` or `HpdfPage.Height = value` | ✅ Available | Extension method + property setter |
| `HPdfPage.SetWidth()` | `HpdfPage.SetWidth()` or `HpdfPage.Width = value` | ✅ Available | Extension method + property setter |
| `HPdfPage.GetHeight()` | `HpdfPage.Height` | ✅ Available | Property getter |
| `HPdfPage.GetWidth()` | `HpdfPage.Width` | ✅ Available | Property getter |
| `HPdfPage.MoveTo()` | `HpdfPage.MoveTo()` | ✅ Available | Extension method |
| `HPdfPage.LineTo()` | `HpdfPage.LineTo()` | ✅ Available | Extension method |
| `HPdfPage.Stroke()` | `HpdfPage.Stroke()` | ✅ Available | Extension method |
| `HPdfPage.Fill()` | `HpdfPage.Fill()` | ✅ Available | Extension method |
| `HPdfPage.Rectangle()` | `HpdfPage.Rectangle()` | ✅ Available | Extension method |
| `HPdfPage.SetLineCap()` | `HpdfPage.SetLineCap()` | ✅ Available | Extension method |
| `HPdfPage.SetDash()` | `HpdfPage.SetDash()` | ✅ Available | Extension method |
| `HPdfPage.SetRGBStroke()` | `HpdfPage.SetRgbStroke()` | ✅ Available | Extension method (note case change) |
| `HPdfPage.SetRGBFill()` | `HpdfPage.SetRgbFill()` | ✅ Available | Extension method (note case change) |
| `HPdfPage.SetLineWidth()` | `HpdfPage.SetLineWidth()` | ✅ Available | Extension method |
| `HPdfPage.GetRGBFill()` | `HpdfPage.GetRGBFill()` | ✅ Available | Extension method |
| `HPdfPage.GetRGBStroke()` | `HpdfPage.GetRGBStroke()` | ✅ Available | Extension method |
| `HPdfPage.BeginText()` | `HpdfPage.BeginText()` | ✅ Available | Extension method |
| `HPdfPage.EndText()` | `HpdfPage.EndText()` | ✅ Available | Extension method |
| `HPdfPage.MoveTextPos()` | `HpdfPage.MoveTextPos()` | ✅ Available | Extension method |
| `HPdfPage.SetFontAndSize()` | `HpdfPage.SetFontAndSize()` | ✅ Available | Extension method |
| `HPdfPage.ShowText()` | `HpdfPage.ShowText()` | ✅ Available | Extension method |
| `HPdfPage.SetWordSpace()` | `HpdfPage.SetWordSpace()` | ✅ Available | Extension method |
| `HPdfPage.TextWidth()` | `HpdfPage.TextWidth()` | ✅ Available | Extension method |
| `HPdfPage.MeasureText()` | `HpdfFont.MeasureText()` | ✅ Available | Extension method with word wrapping support |
| `HPdfPage.DrawImage()` | `HpdfPage.DrawImage()` | ✅ Available | Extension method |
| `HPdfPage.GSave()` | `HpdfPage.GSave()` | ✅ Available | Extension method |
| `HPdfPage.GRestore()` | `HpdfPage.GRestore()` | ✅ Available | Extension method |
| `HPdfPage.Concat()` | `HpdfPage.Concat()` | ✅ Available | Extension method |
| `HPdfPage.GetCurrentFont()` | `HpdfPage.GetCurrentFont()` | ✅ Available | Extension method |
| `HPdfPage.GetCurrentFontSize()` | `HpdfPage.GetCurrentFontSize()` | ✅ Available | Extension method |
| `HPdfPage.CreateURILinkAnnot()` | `HpdfPage.CreateLinkAnnotation()` | ✅ Available | |
| `HPdfPage.CreateLinkAnnot()` | `HpdfPage.CreateLinkAnnotation()` | ✅ Available | |
| `HPdfPage.CreateDestination()` | `HpdfPage.CreateDestination()` | ✅ Available | Extension method |

### Font Level Mapping

| HPdf P/Invoke API | C# Port Equivalent | Status | Notes |
|------------------|-------------------|--------|-------|
| `HPdfFont.GetFontName()` | `HpdfFont.BaseFont` | ✅ Available | Property |
| `HPdfFont.GetEncodingName()` | `HpdfFont.EncodingCodePage` | ⚠️ Different | Returns code page int?, not name |
| `HPdfFont.MeasureText()` | `HpdfFont.MeasureText()` | ✅ Available | Implemented for all font types |
| `HPdfFont.GetAscent()` | `HpdfFont.GetAscent()` | ✅ Available | Returns int in 1000-unit glyph space |
| `HPdfFont.GetDescent()` | `HpdfFont.GetDescent()` | ✅ Available | Returns int in 1000-unit glyph space |
| `HPdfFont.GetXHeight()` | `HpdfFont.GetXHeight()` | ✅ Available | Returns int in 1000-unit glyph space |
| `HPdfFont.GetBBox()` | `HpdfFont.GetBBox()` | ✅ Available | Returns HpdfBox in 1000-unit glyph space |

### Image Level Mapping

| HPdf P/Invoke API | C# Port Equivalent | Status | Notes |
|------------------|-------------------|--------|-------|
| `HPdfImage.GetWidth()` | `HpdfImage.Width` | ✅ Available | Property |
| `HPdfImage.GetHeight()` | `HpdfImage.Height` | ✅ Available | Property |

### Outline/Annotation Mapping

| HPdf P/Invoke API | C# Port Equivalent | Status | Notes |
|------------------|-------------------|--------|-------|
| `HPdfOutline.SetOpened()` | `HpdfOutline.SetOpened()` | ✅ Available | |
| `HPdfOutline.SetDestination()` | `HpdfOutline.SetDestination()` | ✅ Available | |
| `HPdfDestination.SetXYZ()` | `HpdfDestination.SetXYZ()` | ✅ Available | |
| `HPdfAnnotation.SetBorderStyle()` | `HpdfAnnotation.SetBorderStyle()` | ✅ Available | |

## Missing Functionality in C# Port

### Critical (Blocking Migration)

1. **TrueType Font Loading - API Compatibility Wrapper**
   - **Required**: Extension method `HpdfDocument.LoadTTFontFromFile(string path, bool embedding)` that returns **font name string**
   - **Current**: ✅ **HpdfTrueTypeFont.LoadFromFile() FULLY IMPLEMENTED** (784 lines in HpdfTrueTypeFont.cs)
     - Full TTF parsing (head, hhea, maxp, OS/2, cmap, name, post, hmtx tables)
     - Code page support (437, 1251, 1252, etc.)
     - Custom encoding dictionaries with Differences arrays
     - Font embedding with compression
     - ToUnicode CMap generation
     - Glyph metrics and text measurement
   - **What's Missing**: Only wrapper to match p/invoke API signature (returns string, not object)
   - **Priority**: HIGH
   - **Usage**: LibharuGraphics lines 71, 111

2. **Type 1 Font Loading - API Compatibility Wrapper**
   - **Required**: Extension method `HpdfDocument.LoadType1FontFromFile(string afmPath, string pfbPath)` that returns **font name string**
   - **Current**: ✅ **HpdfType1Font.LoadFromFile() FULLY IMPLEMENTED** (454 lines in HpdfType1Font.cs)
     - AFM parsing for metrics
     - PFB parsing for font program
     - Code page support with encoding dictionaries
     - Font embedding with Length1/Length2/Length3
     - ToUnicode CMap generation
     - Text measurement
   - **What's Missing**: Only wrapper to match p/invoke API signature (returns string, not object)
   - **Priority**: HIGH
   - **Usage**: LibharuGraphics line 84

3. ✅ ~~**JPEG Image Loading**~~ **COMPLETE**
   - **Implemented**: `HpdfImage.LoadJpegImageFromFile(xref, localName, filePath)` static method
   - **Status**: Fully functional with DCTDecode filter
   - **Features**:
     - Direct JPEG embedding (no re-compression)
     - Supports RGB, Grayscale, and CMYK color spaces
     - Parses JPEG headers for metadata
     - Preserves original quality and file size
   - **Usage**: LibharuGraphics line 661
   - **Note**: Uses StbImageSharp for metadata parsing, raw JPEG data embedded directly

4. ✅ ~~**Raw Image Loading (from memory)**~~ **COMPLETE**
   - **Implemented**: `HpdfImage.LoadRawImageFromMem(xref, localName, data, width, height, colorSpace, bitsPerComponent)`
   - **Status**: Fully functional for DeviceGray, DeviceRgb, and DeviceCmyk color spaces
   - **Features**:
     - Validates input data size matches expected dimensions
     - Supports 8-bit per component images
     - Applies FlateDecode compression automatically
     - Creates proper image XObject with ColorSpace, Width, Height, BitsPerComponent
   - **Testing**: 14 comprehensive unit tests covering all color spaces and edge cases
   - **Usage**: LibharuGraphics line 626

5. ✅ ~~**PNG Loading from Memory**~~ **COMPLETE**
   - **Implemented**: `HpdfImage.LoadPngImageFromMem(xref, localName, data)`
   - **Status**: Fully functional, reuses existing PNG reader infrastructure
   - **Features**:
     - Loads PNG from byte array
     - Supports all PNG color types (Grayscale, RGB, RGBA, Palette)
     - Handles transparency (alpha channel, tRNS chunk)
     - Same quality as file-based loading
   - **Testing**: Unit tests for valid data, null data, empty data scenarios
   - **Usage**: LibharuGraphics line 631

### Important (Affects Functionality)

6. ✅ ~~**Font Metrics**~~ **COMPLETE**
   - **Implemented**:
     - `HpdfFont.GetAscent()` - returns int in 1000-unit glyph space
     - `HpdfFont.GetDescent()` - returns int in 1000-unit glyph space
     - `HpdfFont.GetXHeight()` - returns int in 1000-unit glyph space
     - `HpdfFont.GetBBox()` - returns HpdfBox in 1000-unit glyph space
   - **Status**: Fully functional for all font types (Standard, TrueType, Type1, CID)
   - **Implementation**:
     - Standard fonts: Metrics table with data for all 14 fonts
     - TrueType/CID: Extracted from font tables with proper scaling
     - Type1: Parsed from AFM files
     - Unified through IHpdfFontImplementation interface
   - **Usage**: LibharuGraphics lines 511-545

7. ✅ ~~**Text Measurement with Wrapping**~~ **COMPLETE**
   - **Implemented**: `HpdfFont.MeasureText(text, fontSize, width, charSpace, wordSpace, wordWrap, out realWidth)`
   - **Status**: Fully functional extension method in HpdfFontExtensions.cs
   - **Features**:
     - Word wrap mode: breaks only at whitespace boundaries
     - Character mode: breaks anywhere in text
     - Character spacing and word spacing support
     - Line feed detection and handling
     - Returns character count that fits within width
     - Output parameter for actual width used
     - Compatible with libharu's HPDF_Font_MeasureText
   - **Testing**: 12 comprehensive unit tests all passing
   - **Demo**: TextWrappingDemo.cs shows word wrapping vs character breaking
   - **Usage**: LibharuGraphics lines 445-481

8. **CID Font Support (Asian Languages)**
   - **Required**: Document-level API compatibility layer for `UseCNS/CNT/JP/KR` methods
   - **Current**: **CID font infrastructure is COMPLETE** - `HpdfCIDFont.cs` fully implemented (1056 lines)
   - **Priority**: LOW (infrastructure exists, only needs API compatibility methods)
   - **Usage**: LibharuGraphics lines 183-229, 809-821
   - **Implementation Status**:
     - ✅ CID font loading from TrueType files
     - ✅ Support for code pages: 932 (Japanese), 936 (Chinese Simplified), 949 (Korean), 950 (Chinese Traditional)
     - ✅ Identity-H encoding with CIDToGIDMap
     - ✅ ToUnicode CMap generation
     - ✅ Font embedding and subsetting
     - ✅ Text measurement for multibyte characters
     - ✅ Glyph ID conversion for content streams
   - **What's Missing**: Only compatibility extension methods needed (see Section 6.1 below)

### Nice to Have (Compatibility)

9. ✅ ~~**Compression Mode**~~ **COMPLETE**
   - **Implemented**: Full `SetCompressionMode()` implementation
   - **Status**: Fully functional with FlateDecode compression
   - **Features**:
     - `HpdfCompressionMode.None` - No compression (default)
     - `HpdfCompressionMode.Text` - Compress page content streams
     - `HpdfCompressionMode.Image` - Compress images (already implemented)
     - `HpdfCompressionMode.Metadata` - Compress metadata (placeholder)
     - `HpdfCompressionMode.All` - All compression types
   - **Implementation**:
     - Page content streams compressed with zlib (FlateDecode)
     - Achieves >50% size reduction on text-heavy documents
     - Verified with zlib header (0x78 0x9C) detection
   - **Testing**: 19 tests (10 unit + 9 integration) all passing
   - **Usage**: LibharuDocument line 35

## Interface Refactoring Suggestions

### 1. Unified Loading Pattern

The C# port uses static factory methods which is cleaner than the original API. Recommend adding:

```csharp
// HpdfDocumentExtensions.cs additions

// Font registry to track loaded fonts (mimics p/invoke behavior)
private static readonly Dictionary<HpdfDocument, Dictionary<string, HpdfFont>> _loadedFonts
    = new Dictionary<HpdfDocument, Dictionary<string, HpdfFont>>();

// Returns font name (PostScript name) for p/invoke API compatibility
public static string LoadTTFontFromFile(this HpdfDocument document, string filePath, bool embedding)
{
    var localName = $"F{document.Xref.Entries.Count}";
    var ttFont = HpdfTrueTypeFont.LoadFromFile(document.Xref, localName, filePath, embedding);
    var font = ttFont.AsFont();

    // Store font in registry
    if (!_loadedFonts.ContainsKey(document))
        _loadedFonts[document] = new Dictionary<string, HpdfFont>();
    _loadedFonts[document][ttFont.BaseFont] = font;

    // Return PostScript name (matches p/invoke behavior)
    return ttFont.BaseFont;
}

// Returns font name (PostScript name) for p/invoke API compatibility
public static string LoadType1FontFromFile(this HpdfDocument document, string afmPath, string pfbPath)
{
    var localName = $"F{document.Xref.Entries.Count}";
    var t1Font = HpdfType1Font.LoadFromFile(document.Xref, localName, afmPath, pfbPath);
    var font = t1Font.AsFont();

    // Store font in registry
    if (!_loadedFonts.ContainsKey(document))
        _loadedFonts[document] = new Dictionary<string, HpdfFont>();
    _loadedFonts[document][t1Font.BaseFont] = font;

    // Return PostScript name (matches p/invoke behavior)
    return t1Font.BaseFont;
}

public static HpdfImage LoadJpegImageFromFile(this HpdfDocument document, string filePath)
{
    var localName = $"Im{document.Xref.Entries.Count}";
    return HpdfImage.LoadJpegImageFromFile(document.Xref, localName, filePath);
}

public static HpdfImage LoadPngImageFromMem(this HpdfDocument document, byte[] data)
{
    var localName = $"Im{document.Xref.Entries.Count}";
    return HpdfImage.LoadPngImageFromMem(document.Xref, localName, data);
}

public static HpdfImage LoadRawImageFromMem(this HpdfDocument document, byte[] data, uint width, uint height, HpdfColorSpace colorSpace, byte bitsPerComponent)
{
    var localName = $"Im{document.Xref.Entries.Count}";
    return HpdfImage.LoadRawImageFromMem(document.Xref, localName, data, width, height, colorSpace, bitsPerComponent);
}
```

### 2. ✅ Page Size Setting Compatibility - **IMPLEMENTED**

Extension methods added for p/invoke API compatibility:

```csharp
// HpdfPageExtensions.cs - IMPLEMENTED (2025-01-14)
public static void SetHeight(this HpdfPage page, float height)
{
    page.Height = height;
}

public static void SetWidth(this HpdfPage page, float width)
{
    page.Width = width;
}
```

### 3. ✅ Font Metrics API - **IMPLEMENTED**

Font metrics fully implemented through unified interface:

```csharp
// HpdfFont.cs - IMPLEMENTED
public int GetAscent() => _implementation.Ascent;
public int GetDescent() => _implementation.Descent;
public int GetXHeight() => _implementation.XHeight;
public HpdfBox GetBBox() => _implementation.FontBBox;

// IHpdfFontImplementation interface - all font types implement this
public interface IHpdfFontImplementation
{
    int Ascent { get; }
    int Descent { get; }
    int XHeight { get; }
    HpdfBox FontBBox { get; }
    float GetCharWidth(byte charCode);
    float MeasureText(string text, float fontSize);
}

// Implementations:
// - HpdfStandardFontImpl: Uses metrics table from C source
// - HpdfTrueTypeFont: Extracts from font tables with scaling
// - HpdfType1Font: Parses from AFM files
// - HpdfCIDFont: Extracts from TrueType tables with scaling
```

### 4. Enhanced Text Measurement

Add overload to HpdfFont for compatibility:

```csharp
// HpdfFont.cs addition
public int MeasureText(string text, float fontSize, float width, bool wordWrap, out float realWidth)
{
    // Implementation that handles word wrapping and returns character count
    // Sets realWidth to actual width used
}
```

### 5. Encoder Compatibility Layer

Since the C# port doesn't need explicit encoder calls, add no-op methods for compatibility:

```csharp
// HpdfDocumentExtensions.cs additions
public static int UseUTFEncodings(this HpdfDocument document)
{
    // C# port handles UTF-8 natively
    return 0; // Success
}

public static void SetCurrentEncoder(this HpdfDocument document, string encodingName)
{
    // C# port uses font-specific encoding
    // No-op for compatibility
}
```

### 6. CID Font Support Layer

**GOOD NEWS**: The C# port has **complete CID font infrastructure** implemented in `HpdfCIDFont.cs`. Only compatibility wrapper methods are needed.

#### 6.1 CID Font Implementation Status

The C# port includes full support for:
- **CIDFontType2** (TrueType-based CID fonts)
- **Identity-H encoding** for horizontal writing
- **ToUnicode CMap** generation for text extraction
- **Code pages**: 932 (Japanese Shift-JIS), 936 (Chinese Simplified GBK), 949 (Korean EUC-KR), 950 (Chinese Traditional Big5)
- **Font embedding** with full TrueType parsing
- **Glyph ID mapping** for content streams
- **Multibyte text measurement**

#### 6.2 How CJK Fonts Work in C# Port

Unlike the p/invoke version which uses `UseCNSFonts()/UseCNSEncodings()` to enable built-in fonts:

1. **Load a TrueType font with CID support**:
   ```csharp
   var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(document, "CJK1", "path/to/font.ttf", 936);
   var font = cidFont.AsFont();
   ```

2. **Use the font normally**:
   ```csharp
   page.SetFontAndSize(font, 12);
   page.ShowText("中文文本"); // Chinese text
   ```

3. **The CID font automatically**:
   - Maps Unicode characters to glyph IDs
   - Generates proper PDF Type 0 font structure
   - Creates ToUnicode CMap for text extraction
   - Embeds font with proper CID tables

#### 6.3 Compatibility Wrapper Methods Needed

The p/invoke API uses predefined font names (`SimSun`, `MS-Gothic`, etc.) with separate encoding calls. For compatibility:

```csharp
// HpdfDocumentExtensions.cs additions

// Dictionary to map font names to TTF paths (user-configurable)
private static Dictionary<string, string> _cjkFontPaths = new Dictionary<string, string>();

public static void ConfigureCJKFontPath(string fontName, string ttfPath)
{
    _cjkFontPaths[fontName] = ttfPath;
}

// Chinese Simplified support
public static void UseCNSFonts(this HpdfDocument document)
{
    // In C# port, this is a no-op - fonts are loaded on demand
    // Just validate that required font files are configured
}

public static void UseCNSEncodings(this HpdfDocument document)
{
    // In C# port, encodings are handled automatically by CID fonts
    // No-op for compatibility
}

// Chinese Traditional support
public static void UseCNTFonts(this HpdfDocument document)
{
    // No-op - fonts loaded on demand
}

public static void UseCNTEncodings(this HpdfDocument document)
{
    // No-op - handled by CID fonts
}

// Japanese support
public static void UseJPFonts(this HpdfDocument document)
{
    // No-op - fonts loaded on demand
}

public static void UseJPEncodings(this HpdfDocument document)
{
    // No-op - handled by CID fonts
}

// Korean support
public static void UseKRFonts(this HpdfDocument document)
{
    // No-op - fonts loaded on demand
}

public static void UseKREncodings(this HpdfDocument document)
{
    // No-op - handled by CID fonts
}

// Enhanced GetFont for CJK fonts
public static HpdfFont GetFont(this HpdfDocument document, string fontName, string encodingName)
{
    // Map to code page
    int codePage = GetCodePageFromEncoding(encodingName);

    // If this is a CJK font name, load as CID font
    if (IsCJKFont(fontName))
    {
        string ttfPath = GetCJKFontPath(fontName);
        if (ttfPath != null && File.Exists(ttfPath))
        {
            var localName = $"F{document.Xref.Entries.Count}";
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(document, localName, ttfPath, codePage);
            return cidFont.AsFont();
        }
    }

    // Otherwise use standard font loading
    return GetStandardFont(document, fontName, encodingName);
}

private static bool IsCJKFont(string fontName)
{
    return fontName == "SimSun" || fontName == "SimHei" ||      // Chinese Simplified
           fontName == "MingLiU" ||                             // Chinese Traditional
           fontName == "MS-Mincho" || fontName == "MS-Gothic" || // Japanese
           fontName == "MS-PMincho" || fontName == "MS-PGothic" ||
           fontName == "Batang" || fontName == "Dotum" ||       // Korean
           fontName == "BatangChe" || fontName == "DotumChe";
}

private static int GetCodePageFromEncoding(string encodingName)
{
    return encodingName switch
    {
        "GBK-EUC-H" => 936,        // Chinese Simplified
        "ETen-B5-H" => 950,        // Chinese Traditional
        "EUC-H" => 932,            // Japanese
        "KSCms-UHC-H" => 949,      // Korean
        _ => 0
    };
}

private static string GetCJKFontPath(string fontName)
{
    if (_cjkFontPaths.TryGetValue(fontName, out string path))
        return path;

    // Try common system paths (Windows, Linux, macOS)
    string[] searchPaths = GetSystemFontPaths();
    string[] possibleNames = GetFontFileNames(fontName);

    foreach (string basePath in searchPaths)
    {
        foreach (string fileName in possibleNames)
        {
            string fullPath = Path.Combine(basePath, fileName);
            if (File.Exists(fullPath))
                return fullPath;
        }
    }

    return null;
}
```

#### 6.4 Migration Impact for CJK Support

**Low Impact** - The CID font infrastructure is complete. Migration only requires:

1. **Configuration**: Users need to configure paths to CJK TrueType fonts
   ```csharp
   HpdfDocumentExtensions.ConfigureCJKFontPath("SimSun", "/path/to/simsun.ttf");
   ```

2. **Font files**: Unlike p/invoke version which relied on built-in fonts in libharu, users must provide TrueType font files
   - **Advantage**: More control, better embedding, cross-platform
   - **Disadvantage**: Requires font files to be distributed or installed

3. **API remains the same**: Existing code using `EnableMultibyteEncodingAndCIDFont()` continues to work

## Migration Strategy

### Phase 1: Core Infrastructure (Week 1-2) - ✅ **MOSTLY COMPLETE**

**Goal**: Implement missing critical functionality

1. ✅ ~~Implement TrueType font loading~~ **COMPLETE**
   - ✅ `HpdfTrueTypeFont.LoadFromFile()` fully implemented (784 lines)
   - ✅ Embedding and code page support included
   - **Only needed**: API compatibility wrapper (see Section 1)

2. ✅ ~~Implement Type 1 font loading~~ **COMPLETE**
   - ✅ `HpdfType1Font.LoadFromFile()` fully implemented (454 lines)
   - ✅ AFM and PFB parsing complete
   - **Only needed**: API compatibility wrapper (see Section 1)

3. ✅ ~~Implement JPEG image loading~~ **COMPLETE**
   - ✅ Uses StbImageSharp for JPEG metadata parsing
   - ✅ `HpdfImage.LoadJpegImageFromFile()` static method implemented
   - ✅ Supports RGB, Grayscale, and CMYK color spaces
   - ✅ Direct DCTDecode embedding (no re-compression)

4. ✅ ~~Implement image loading from memory~~ **COMPLETE**
   - ✅ `HpdfImage.LoadPngImageFromMem()` implemented (2025-01-14)
   - ✅ `HpdfImage.LoadRawImageFromMem()` implemented (2025-01-14)
   - ✅ 14 comprehensive unit tests covering all scenarios

### Phase 2: Enhanced APIs (Week 3)

**Goal**: Add compatibility layer and missing features

5. ✅ ~~Add font metrics support~~ **COMPLETE**
   - ✅ Implemented GetAscent/GetDescent/GetXHeight/GetBBox
   - ✅ Extract metrics from TrueType tables
   - ✅ Use standard metrics for Type 1 fonts
   - ✅ Created IHpdfFontImplementation interface
   - ✅ All font types implement unified interface

6. ✅ ~~Enhanced text measurement~~ **COMPLETE**
   - ✅ Implemented HpdfFontExtensions.MeasureText with word wrapping
   - ✅ Returns character count and actual width via out parameter
   - ✅ Supports word wrap mode (breaks at whitespace) and character mode (breaks anywhere)
   - ✅ Handles character spacing and word spacing
   - ✅ Detects and breaks at line feeds (0x0A)
   - ✅ Compatible with libharu's HPDF_Font_MeasureText API
   - ✅ 12 comprehensive unit tests passing

7. Add extension methods for API compatibility (PARTIAL)
   - ✅ Page extension methods (SetHeight/SetWidth) - DONE (2025-01-14)
   - ❌ Document extension methods (LoadTTFontFromFile, etc.) - REMAINING
   - ❌ Encoder compatibility stubs - REMAINING

### Phase 3: LibharuDocument Migration (Week 4)

**Goal**: Create new C# port-based implementation

8. Create new implementation class
   - Create `HaruCsDocument.cs` implementing `INativeDocument`
   - Replace HPdf types with Haru types
   - Maintain same public interface

9. Test document operations
   - Document creation/saving
   - Page management
   - Font loading
   - Encoding support

### Phase 4: LibharuGraphics Migration (Week 5-6)

**Goal**: Migrate graphics operations to C# port

10. Create new graphics implementation
    - Create `HaruCsGraphics.cs` extending `Graphics`
    - Map all drawing operations
    - Handle font operations with new API

11. Test graphics operations
    - Line drawing
    - Text rendering
    - Image drawing
    - Transformations
    - Links and annotations

### Phase 5: CID Font Support (Week 7, Optional)

**Goal**: Enable international language support

12. Implement CID font infrastructure
    - Add UseCNS/CNT/JP/KR methods
    - Support CID font loading
    - Test with Chinese/Japanese/Korean text

### Phase 6: Integration and Testing (Week 8)

**Goal**: Full integration with PDF.Flow

13. Integration testing
    - Run full PDF.Flow test suite
    - Compare output PDFs byte-by-byte if possible
    - Test all encoding scenarios
    - Test all font types
    - Test all image formats

14. Performance testing
    - Compare performance vs native implementation
    - Identify bottlenecks
    - Optimize critical paths

15. Documentation
    - Update API documentation
    - Create migration guide for other components
    - Document any breaking changes

## Implementation Roadmap

### Required New Types/Methods in C# Port

#### Haru/Font/HpdfTrueTypeFont.cs
**✅ ALREADY IMPLEMENTED** - 784 lines, fully functional
```csharp
// Already exists:
public static HpdfTrueTypeFont LoadFromFile(HpdfXref xref, string localName, string filePath, bool embedding, int codePage = 437)
{
    // ✅ Parses TTF file (all tables)
    // ✅ Creates font dictionary with encoding
    // ✅ Subsets and embeds font if requested
    // ✅ Returns HpdfTrueTypeFont with AsFont() method
}
```

#### Haru/Font/HpdfType1Font.cs
**✅ ALREADY IMPLEMENTED** - 454 lines, fully functional
```csharp
// Already exists:
public static HpdfType1Font LoadFromFile(HpdfXref xref, string localName, string afmPath, string pfbPath, int codePage = 1252)
{
    // ✅ Parses AFM for metrics
    // ✅ Parses PFB for font program
    // ✅ Creates font dictionary with encoding
    // ✅ Returns HpdfType1Font with AsFont() method
}
```

#### Haru/Doc/HpdfImage.cs
```csharp
public static HpdfImage LoadJpegImageFromFile(HpdfXref xref, string localName, string filePath)
{
    // Decode JPEG metadata
    // Create XObject stream
    // Store compressed JPEG data directly
}

public static HpdfImage LoadPngImageFromMem(HpdfXref xref, string localName, byte[] data)
{
    // Reuse existing PNG reader
    // Create from memory stream
}

public static HpdfImage LoadRawImageFromMem(HpdfXref xref, string localName, byte[] data,
    uint width, uint height, HpdfColorSpace colorSpace, byte bitsPerComponent)
{
    // Create image XObject with raw data
    // No decoding needed
}
```

#### Haru/Font/HpdfFont.cs and HpdfFontExtensions.cs
**✅ FULLY IMPLEMENTED** - Font metrics and text wrapping complete
```csharp
// HpdfFont.cs - Already exists:
public int GetAscent() => _implementation.Ascent;
public int GetDescent() => _implementation.Descent;
public int GetXHeight() => _implementation.XHeight;
public HpdfBox GetBBox() => _implementation.FontBBox;
public float MeasureText(string text, float fontSize) => _implementation.MeasureText(text, fontSize);

// HpdfFontExtensions.cs - ✅ IMPLEMENTED:
public static int MeasureText(this HpdfFont font, string text, float fontSize, float width,
    float charSpace, float wordSpace, bool wordWrap, out float realWidth)
{
    // ✅ Full implementation with word wrapping support
    // ✅ Compatible with libharu's HPDF_Font_MeasureText
    // ✅ Returns character count that fits within width
    // ✅ Sets realWidth to actual width used
}
```

#### Haru/Doc/HpdfDocumentExtensions.cs
```csharp
// All extension methods listed in "Unified Loading Pattern" section above
```

### New Implementation Files

Create two new implementation files in PDF.Flow:

1. **Gehtsoft.PDFFlow/Core/HaruCsDocument.cs**
   - Implements INativeDocument interface
   - Uses Haru namespace instead of HPdf
   - Maintains compatibility with existing code

2. **Gehtsoft.PDFFlow/Rendering/Native/HaruCsGraphics.cs**
   - Extends Graphics base class
   - Uses Haru namespace instead of HPdf
   - Maintains compatibility with existing Graphics interface

### Configuration Switch

Add configuration to switch between implementations:

```csharp
// In DocumentBuilder or configuration
public enum HaruImplementation
{
    Native,    // Current p/invoke implementation
    CSharp     // New C# port implementation
}

// Factory method
INativeDocument CreateNativeDocument()
{
    if (Configuration.HaruImplementation == HaruImplementation.CSharp)
        return new HaruCsDocument();
    else
        return new LibharuDocument();
}
```

## Testing Strategy

### Unit Tests

1. **Font Loading Tests**
   - Load various TrueType fonts
   - Load Type 1 fonts with AFM/PFB
   - Verify font metrics
   - Test font subsetting

2. **Image Loading Tests**
   - Load JPEG images (various color spaces)
   - Load PNG images from file and memory
   - Load raw image data
   - Verify dimensions and color spaces

3. **Text Measurement Tests**
   - Measure simple text
   - Measure with word wrapping
   - Test different encodings
   - Verify character counts

### Integration Tests

1. **Document Creation**
   - Create multi-page documents
   - Test various page sizes
   - Verify PDF structure

2. **Graphics Operations**
   - Draw lines, rectangles, curves
   - Fill and stroke operations
   - Transformation matrices
   - Clipping paths

3. **Text Rendering**
   - Render with various fonts
   - Test international characters
   - Verify text positioning
   - Test word wrapping

4. **Image Rendering**
   - Draw JPEG images
   - Draw PNG images with transparency
   - Test image scaling and positioning

### Compatibility Tests

1. **Side-by-Side Comparison**
   - Generate same PDF with both implementations
   - Compare file sizes
   - Compare rendering in PDF viewers
   - Validate PDF structure

2. **Performance Benchmarks**
   - Document creation speed
   - Font loading time
   - Image processing time
   - Overall rendering performance

## Risk Assessment

### High Risk
- **Font metrics accuracy**: Font rendering depends on accurate metrics
  - *Mitigation*: Extensive testing with reference fonts, comparison with native output

- **CID font support**: Complex specification, multiple Asian languages
  - *Mitigation*: Phase as optional, focus on common use cases first

### Medium Risk
- **JPEG handling**: Color space conversion, metadata parsing
  - *Mitigation*: Use well-tested library (ImageSharp recommended)

- **Performance regression**: C# may be slower than native code
  - *Mitigation*: Profile and optimize critical paths, consider spans/unsafe code if needed

### Low Risk
- **PNG/raw images**: Infrastructure already exists
- **Text measurement**: Straightforward implementation
- **API compatibility**: Extension methods make migration smooth

## Recommendations

### Immediate Actions (Before Starting Migration)

1. **Set up comprehensive test suite** for current p/invoke implementation
   - Capture reference outputs for comparison
   - Document all edge cases and special behaviors
   - Create performance baselines

2. **Evaluate image processing libraries**
   - Recommended: **ImageSharp** (cross-platform, no native dependencies)
   - Alternative: System.Drawing (Windows-only, has native dependencies)
   - Decision criteria: Cross-platform support, licensing, performance

3. **Review CID font requirements**
   - Identify actual usage in PDF.Flow
   - Determine if full CID support is needed initially
   - Consider phased implementation

### Long-term Considerations

1. **Maintain both implementations** during transition period
   - Keep p/invoke version as fallback
   - Use configuration switch for gradual rollout
   - Plan deprecation timeline

2. **Performance optimization opportunities**
   - Use Span<T> for memory efficiency
   - Consider unsafe code for critical paths
   - Implement caching where appropriate

3. **API evolution**
   - C# port has cleaner API in places
   - Consider gradual API modernization
   - Maintain backward compatibility layer

## Success Criteria

Migration is successful when:

1. ✅ All PDF.Flow unit tests pass with C# implementation
2. ✅ Generated PDFs are visually identical to p/invoke version
3. ✅ Performance is within 20% of p/invoke implementation
4. ✅ No native dependencies required
5. ✅ All supported encodings work correctly
6. ✅ All supported fonts (TrueType, Type1, Standard) work
7. ✅ All supported image formats (PNG, JPEG) work
8. ✅ Cross-platform compatibility (Windows, Linux, macOS)

## Appendix A: API Compatibility Matrix

### Complete Feature Mapping

| Feature Category | P/Invoke Coverage | C# Port Status | Gap |
|-----------------|-------------------|----------------|-----|
| Document Management | 100% | 100% | ✅ None (compression complete) |
| Page Operations | 100% | 100% | ✅ None (SetHeight/SetWidth added) |
| Standard Fonts | 100% | 100% | ✅ None (metrics complete) |
| TrueType Fonts | 100% | 98% | API wrapper only |
| Type 1 Fonts | 100% | 98% | API wrapper only |
| CID Fonts | 100% | 98% | API compatibility wrappers |
| PNG Images | 100% | 100% | ✅ None (memory loading complete) |
| JPEG Images | 100% | 100% | ✅ None (fully implemented) |
| Raw Images | 100% | 100% | ✅ None (memory loading complete) |
| Text Operations | 100% | 100% | ✅ None (wrapping complete) |
| Graphics Primitives | 100% | 100% | ✅ None |
| Colors | 100% | 100% | ✅ None |
| Transformations | 100% | 100% | ✅ None |
| Links/Annotations | 100% | 100% | ✅ None |
| Outlines/Bookmarks | 100% | 100% | ✅ None |
| Encoding Support | 100% | 80% | CID encodings |

**Overall Completion**: ~97% (increased due to image loading from memory and page convenience methods)

## Appendix B: File Structure Changes

### New Files Created

```
Haru.Net/cs-src/Haru/
├── Font/
│   ├── IHpdfFontImplementation.cs ✅ CREATED (interface for all font types)
│   ├── HpdfStandardFontImpl.cs ✅ CREATED (Standard 14 fonts implementation)
│   ├── HpdfStandardFontMetrics.cs ✅ EXISTS (metrics table)
│   ├── HpdfStandardFontWidths.cs ✅ EXISTS (character width tables)
│   ├── HpdfTrueTypeFont.cs ✅ EXISTS (implements IHpdfFontImplementation)
│   ├── HpdfType1Font.cs ✅ EXISTS (implements IHpdfFontImplementation)
│   ├── HpdfCIDFont.cs ✅ EXISTS (implements IHpdfFontImplementation)
│   └── HpdfFontExtensions.cs ✅ CREATED (text wrapping support)
├── Doc/
│   ├── HpdfImage.cs ✅ EXISTS (with JPEG support)
│   └── HpdfDocumentExtensions.cs (needs enhancement for API wrappers)
└── Imaging/
    └── Uses StbImageSharp (external library)

Haru.Net/cs-src/Haru.Test/
└── Font/
    └── HpdfFontExtensionsTests.cs ✅ CREATED (12 comprehensive tests for text wrapping)

Haru.Net/cs-src/Haru.Demos/
└── TextWrappingDemo.cs ✅ CREATED (visual demonstration of word wrapping)

Gehtsoft.PDFFlow/Gehtsoft.PDFFlow/
├── Core/
│   └── HaruCsDocument.cs (TODO: new)
└── Rendering/Native/
    └── HaruCsGraphics.cs (TODO: new)
```

### Modified Files

```
Haru.Net/cs-src/Haru/
├── Font/
│   ├── HpdfFont.cs ✅ REFACTORED (now uses IHpdfFontImplementation)
│   ├── HpdfTrueTypeFont.cs ✅ UPDATED (implements interface)
│   ├── HpdfType1Font.cs ✅ UPDATED (implements interface)
│   └── HpdfCIDFont.cs ✅ UPDATED (implements interface)
├── Doc/
│   ├── HpdfPage.cs (no changes needed)
│   ├── HpdfPageExtensions.cs (no changes needed)
│   └── HpdfDocumentExtensions.cs (needs API wrappers)
```

---

**Document Version**: 1.2
**Last Updated**: 2025-01-14
**Author**: Migration Analysis Tool
**Status**: Draft

## Recent Architecture Improvements (2025-01-14)

### Font System Refactoring - Interface-Based Design

**Completed**: Complete refactoring of font architecture using polymorphic design

**Changes Made**:

1. **Created IHpdfFontImplementation Interface**
   - Unified interface for all font types
   - Properties: `Dict`, `BaseFont`, `LocalName`, `CodePage`, `Ascent`, `Descent`, `XHeight`, `FontBBox`
   - Methods: `GetCharWidth(byte)`, `MeasureText(string, float)`

2. **Created HpdfStandardFontImpl Class**
   - New implementation class for PDF Standard 14 fonts
   - Encapsulates font dictionary creation, metrics, and width tables
   - Implements `IHpdfFontImplementation`

3. **Updated All Font Implementation Classes**
   - `HpdfTrueTypeFont`: Now implements `IHpdfFontImplementation`
   - `HpdfType1Font`: Now implements `IHpdfFontImplementation`
   - `HpdfCIDFont`: Now implements `IHpdfFontImplementation`
   - Changed `CodePage` property from `int` to `int?` for interface compatibility

4. **Refactored HpdfFont Wrapper Class**
   - **Before**: Separate fields for each font type (`_ttFont`, `_type1Font`, `_cidFont`, `_standardFont`) with runtime null-checking
   - **After**: Single `_implementation` field of type `IHpdfFontImplementation`
   - Eliminated all null-checking chains and runtime type checking
   - All methods now directly delegate to implementation
   - Simpler constructors - just assign the implementation

**Benefits**:
- ✅ Cleaner code - no null-checking chains
- ✅ Better performance - no runtime type checking
- ✅ Proper polymorphism - unified interface
- ✅ Better maintainability - easier to add new font types
- ✅ Better extensibility - clean abstraction

**Testing**:
- ✅ All 738 unit tests pass (726 + 12 new text wrapping tests)
- ✅ All demos run successfully including FontMetricsDemo and TextWrappingDemo
- ✅ Build successful with 0 errors, 0 warnings

### Text Wrapping Implementation - Complete libharu API Compatibility

**Completed**: Full implementation of text measurement with word wrapping support

**Changes Made**:

1. **Created HpdfFontExtensions.cs**
   - Extension method `MeasureText(text, fontSize, width, charSpace, wordSpace, wordWrap, out realWidth)`
   - Compatible with libharu's `HPDF_Font_MeasureText` C function
   - Returns character count that fits within specified width
   - Output parameter for actual width used

2. **Algorithm Implementation** (ported from hpdf_font_type1.c)
   - Word wrap mode: breaks only after whitespace characters
   - Character mode: can break anywhere in text
   - Whitespace detection: 0x00, 0x09, 0x0A, 0x0C, 0x0D, 0x20
   - Always breaks at line feed (0x0A)
   - Scales character widths from 1000-unit glyph space to user space
   - Adds character spacing between characters
   - Adds word spacing to whitespace characters

3. **Comprehensive Test Suite**
   - 12 unit tests covering all scenarios
   - Tests for word wrap mode vs character mode
   - Tests for character spacing and word spacing
   - Tests for line feed detection
   - Tests for empty/null strings
   - Tests for edge cases (very small width)
   - Tests verifying different fonts produce different results

4. **TextWrappingDemo.cs**
   - Visual demonstration of word wrapping
   - Comparison of word wrap vs character-by-character breaking
   - Shows text fitting within constrained width boxes
   - Displays metrics (max width, font, lines rendered)

**Benefits**:
- ✅ Full API compatibility with libharu
- ✅ Enables automatic text wrapping in PDF.Flow
- ✅ Supports both wrapping modes (word boundaries and anywhere)
- ✅ Proper handling of character and word spacing
- ✅ Accurate text measurement for layout calculations

**Testing**:
- ✅ All 738 unit tests pass (12 new text wrapping tests)
- ✅ TextWrappingDemo demonstrates functionality visually
- ✅ Build successful with 0 errors, 0 warnings
