# Haru P/Invoke to C# Port Migration Plan for PDF.Flow Library

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
| `HPdfDoc.SetCompressionMode()` | `HpdfDocument.SetCompressionMode()` | ⚠️ Partial | Extension method exists but not fully implemented |
| `HPdfDoc.UseUTFEncodings()` | N/A | ⚠️ Not Needed | C# port handles encoding differently |
| `HPdfDoc.SaveToFile()` | `HpdfDocument.SaveToFile()` | ✅ Available | |
| `HPdfDoc.SaveToStream()` | `HpdfDocument.Save(Stream)` | ✅ Available | |
| `HPdfDoc.FreeDocAll()` | Automatic (GC) | ✅ Available | No manual cleanup needed |
| `HPdfDoc.AddPage()` | `HpdfDocument.AddPage()` | ✅ Available | Returns HpdfPage |
| `HPdfDoc.GetPageByIndex()` | `HpdfDocument.Pages[index]` | ✅ Available | Use property indexer |
| `HPdfDoc.LoadTTFontFromFile()` | `HpdfTrueTypeFont.LoadFromFile()` | ❌ Missing | Needs implementation |
| `HPdfDoc.LoadType1FontFromFile()` | `HpdfType1Font.LoadFromFile()` | ❌ Missing | Needs implementation |
| `HPdfDoc.GetFont()` | `HpdfDocument.GetFont()` | ✅ Available | Extension method |
| `HPdfDoc.SetCurrentEncoder()` | N/A | ⚠️ Not Needed | Fonts have built-in encoding |
| `HPdfDoc.UseCNSFonts()/Encodings()` | CID font support | ❌ Missing | Needs CID font implementation |
| `HPdfDoc.UseCNTFonts()/Encodings()` | CID font support | ❌ Missing | Needs CID font implementation |
| `HPdfDoc.UseJPFonts()/Encodings()` | CID font support | ❌ Missing | Needs CID font implementation |
| `HPdfDoc.UseKRFonts()/Encodings()` | CID font support | ❌ Missing | Needs CID font implementation |
| `HPdfDoc.LoadJpegImageFromFile()` | `HpdfImage.LoadJpegImageFromFile()` | ❌ Missing | Needs JPEG support |
| `HPdfDoc.LoadPngImageFromFile()` | `HpdfImage.LoadPngImageFromFile()` | ✅ Available | Extension method |
| `HPdfDoc.LoadPngImageFromMem()` | `HpdfImage.LoadPngImageFromMem()` | ❌ Missing | Needs implementation |
| `HPdfDoc.LoadRawImageFromMem()` | `HpdfImage.LoadRawImageFromMem()` | ❌ Missing | Needs implementation |
| `HPdfDoc.GetPageMode()` | `HpdfCatalog.PageMode` | ✅ Available | Via Catalog property |
| `HPdfDoc.SetPageMode()` | `HpdfCatalog.SetPageMode()` | ✅ Available | Via Catalog property |
| `HPdfDoc.CreateOutline()` | `HpdfDocument.CreateOutline()` | ✅ Available | |

### Page Level Mapping

| HPdf P/Invoke API | C# Port Equivalent | Status | Notes |
|------------------|-------------------|--------|-------|
| `HPdfPage.SetHeight()` | `HpdfPage.Height = value` | ✅ Available | Property setter |
| `HPdfPage.SetWidth()` | `HpdfPage.Width = value` | ✅ Available | Property setter |
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
| `HPdfPage.MeasureText()` | Font-based measurement | ⚠️ Different | Use `HpdfFont.MeasureText()` |
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
| `HPdfFont.GetEncodingName()` | `HpdfFont.EncodingCodePage` | ⚠️ Different | Returns code page int, not name |
| `HPdfFont.MeasureText()` | `HpdfFont.MeasureText()` | ⚠️ Partial | Basic implementation, may need wrapping support |
| `HPdfFont.GetAscent()` | Font metrics | ❌ Missing | Needs implementation |
| `HPdfFont.GetDescent()` | Font metrics | ❌ Missing | Needs implementation |
| `HPdfFont.GetXHeight()` | Font metrics | ❌ Missing | Needs implementation |
| `HPdfFont.GetBBox()` | Font metrics | ❌ Missing | Needs implementation |

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

3. **JPEG Image Loading**
   - **Required**: `HpdfImage.LoadJpegImageFromFile(xref, localName, filePath)` static method
   - **Current**: Only PNG loading is implemented
   - **Priority**: HIGH
   - **Usage**: LibharuGraphics line 661

4. **Raw Image Loading (from memory)**
   - **Required**: `HpdfImage.LoadRawImageFromMem(xref, localName, data, width, height, colorSpace, bitsPerComponent)`
   - **Current**: Not implemented
   - **Priority**: MEDIUM
   - **Usage**: LibharuGraphics line 626

5. **PNG Loading from Memory**
   - **Required**: `HpdfImage.LoadPngImageFromMem(xref, localName, data)`
   - **Current**: Only file-based PNG loading exists
   - **Priority**: MEDIUM
   - **Usage**: LibharuGraphics line 631

### Important (Affects Functionality)

6. **Font Metrics**
   - **Required**:
     - `HpdfFont.GetAscent()` - returns int
     - `HpdfFont.GetDescent()` - returns int
     - `HpdfFont.GetXHeight()` - returns float
     - `HpdfFont.GetBBox()` - returns HpdfBox structure
   - **Current**: Not implemented
   - **Priority**: MEDIUM
   - **Usage**: LibharuGraphics lines 511-545
   - **Note**: Required for accurate text layout and positioning

7. **Text Measurement with Wrapping**
   - **Required**: `HpdfFont.MeasureText(text, width, wordWrap, ref realWidth)`
   - **Current**: Basic MeasureText exists but doesn't support wrapping or realWidth output
   - **Priority**: MEDIUM
   - **Usage**: LibharuGraphics lines 445-481
   - **Note**: Critical for automatic text wrapping in PDF.Flow

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

9. **Compression Mode**
   - **Required**: Full `SetCompressionMode()` implementation
   - **Current**: Stub exists but doesn't apply compression
   - **Priority**: LOW
   - **Usage**: LibharuDocument line 35
   - **Note**: PDF will work without compression, just larger file size

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

### 2. Page Size Setting Compatibility

Add extension method for easier migration:

```csharp
// HpdfPageExtensions.cs additions
public static void SetHeight(this HpdfPage page, float height)
{
    page.Height = height;
}

public static void SetWidth(this HpdfPage page, float width)
{
    page.Width = width;
}
```

### 3. Font Metrics API

Add extension methods or properties to HpdfFont:

```csharp
// HpdfFont.cs additions needed
public int GetAscent()
{
    // Implementation based on font type
    if (_ttFont != null)
        return _ttFont.GetAscent();
    if (_type1Font != null)
        return _type1Font.GetAscent();
    if (_cidFont != null)
        return _cidFont.GetAscent();

    return 750; // Default for standard fonts
}

public int GetDescent()
{
    // Similar pattern
}

public float GetXHeight(float fontSize)
{
    // Similar pattern
}

public HpdfBox GetBBox(float fontSize)
{
    // Similar pattern
}
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

### Phase 1: Core Infrastructure (Week 1-2)

**Goal**: Implement missing critical functionality

1. ✅ ~~Implement TrueType font loading~~ **ALREADY COMPLETE**
   - ✅ `HpdfTrueTypeFont.LoadFromFile()` fully implemented (784 lines)
   - ✅ Embedding and code page support included
   - **Only needed**: API compatibility wrapper (see Section 1)

2. ✅ ~~Implement Type 1 font loading~~ **ALREADY COMPLETE**
   - ✅ `HpdfType1Font.LoadFromFile()` fully implemented (454 lines)
   - ✅ AFM and PFB parsing complete
   - **Only needed**: API compatibility wrapper (see Section 1)

3. Implement JPEG image loading
   - Add JPEG decoder or use external library (e.g., System.Drawing, ImageSharp)
   - Add `HpdfImage.LoadJpegImageFromFile()` static method
   - Handle color space detection

4. Implement image loading from memory
   - Add `HpdfImage.LoadPngImageFromMem()`
   - Add `HpdfImage.LoadRawImageFromMem()`
   - Reuse existing PNG reader infrastructure

### Phase 2: Enhanced APIs (Week 3)

**Goal**: Add compatibility layer and missing features

5. Add font metrics support
   - Implement GetAscent/GetDescent/GetXHeight/GetBBox
   - Extract metrics from TrueType tables
   - Use standard metrics for Type 1 fonts

6. Enhanced text measurement
   - Add word-wrapping support to MeasureText
   - Return character count and actual width
   - Handle different encodings properly

7. Add extension methods for API compatibility
   - Document extension methods (LoadTTFontFromFile, etc.)
   - Page extension methods (SetHeight/SetWidth)
   - Encoder compatibility stubs

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

#### Haru/Font/HpdfFont.cs
```csharp
public int GetAscent() { }
public int GetDescent() { }
public float GetXHeight() { }
public HpdfBox GetBBox() { }
public int MeasureText(string text, float fontSize, float width, bool wordWrap, out float realWidth) { }
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
| Document Management | 100% | 90% | Compression mode |
| Page Operations | 100% | 100% | None |
| Standard Fonts | 100% | 100% | None |
| TrueType Fonts | 100% | 95% | API wrapper, metrics |
| Type 1 Fonts | 100% | 95% | API wrapper, metrics |
| CID Fonts | 100% | 95% | API compatibility wrappers |
| PNG Images | 100% | 80% | Memory loading |
| JPEG Images | 100% | 0% | Full implementation |
| Raw Images | 100% | 0% | Full implementation |
| Text Operations | 100% | 90% | Wrapping measurement |
| Graphics Primitives | 100% | 100% | None |
| Colors | 100% | 100% | None |
| Transformations | 100% | 100% | None |
| Links/Annotations | 100% | 100% | None |
| Outlines/Bookmarks | 100% | 100% | None |
| Encoding Support | 100% | 80% | CID encodings |

**Overall Completion**: ~85% (significantly higher due to complete font implementations)

## Appendix B: File Structure Changes

### New Files to Create

```
Haru.Net/cs-src/Haru/
├── Font/
│   ├── HpdfTrueTypeFont.cs (enhance)
│   ├── HpdfType1Font.cs (enhance)
│   └── HpdfFontMetrics.cs (new)
├── Doc/
│   ├── HpdfImage.cs (enhance)
│   └── HpdfDocumentExtensions.cs (enhance)
└── Imaging/
    └── JpegDecoder.cs (new, or use ImageSharp)

Gehtsoft.PDFFlow/Gehtsoft.PDFFlow/
├── Core/
│   └── HaruCsDocument.cs (new)
└── Rendering/Native/
    └── HaruCsGraphics.cs (new)
```

### Modified Files

```
Haru.Net/cs-src/Haru/
├── Font/HpdfFont.cs
├── Doc/HpdfPage.cs
├── Doc/HpdfPageExtensions.cs
└── Doc/HpdfDocumentExtensions.cs
```

---

**Document Version**: 1.0
**Last Updated**: 2025-01-XX
**Author**: Migration Analysis Tool
**Status**: Draft
