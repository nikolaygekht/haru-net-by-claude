# Future Demo Ideas

**Last Updated**: 2025-10-28

This file tracks ideas for new demonstration programs to showcase Haru.NET features.

## Completed Demos

- ✅ **ShapesDemo** (2025-10-28) - High-level shape drawing (Circles, Ellipses, Rectangles, Arcs, combined shapes)
- ✅ **TransparencyDemo** (2025-10-28) - Alpha transparency and blend modes (watermarks, highlights, shadows, glass effects)
- ✅ **AnnotationsDemo** (2025-10-28) - Text annotations/sticky notes with different icons
- ✅ **PdfADemo** (2025-10-28) - PDF/A-1b archival documents with embedded fonts, color profiles, and XMP metadata
- ✅ **MetadataDemo** (2025-10-28) - Document metadata (standard and custom fields)
- ✅ **AdvancedTextDemo** (2025-10-28) - Text rendering modes, transformations, positioning, and spacing
- ✅ **ColorSpacesDemo** (2025-10-28) - RGB, CMYK, and Grayscale color spaces with rich blacks for professional printing
- ✅ **CompressionDemo** (2025-10-28) - PDF compression modes for file size optimization

## High Priority - Core Visual Features

### TransparencyDemo - Alpha Blending & Blend Modes
**Status**: ✅ COMPLETED (2025-10-28)
**Estimated Time**: 1-2 hours
**Priority**: HIGH

**Why**: ExtGState transparency features are powerful but not demonstrated
**What to show**:
- Alpha transparency for fills and strokes (SetAlphaFill, SetAlphaStroke)
- Different blend modes (Normal, Multiply, Screen, Overlay, Darken, Lighten, ColorDodge, ColorBurn, HardLight, SoftLight, Difference, Exclusion)
- Overlapping shapes with transparency
- Watermarks and semi-transparent overlays
- Text with alpha transparency
- Practical examples: watermarks, overlays, shadows

**Impact**: Essential for modern PDF graphics and watermarking
**Files**: `Haru/Doc/HpdfExtGState.cs` - 12 blend modes available
**API Methods**:
- `HpdfExtGState.SetAlphaFill(float alpha)`
- `HpdfExtGState.SetAlphaStroke(float alpha)`
- `HpdfExtGState.SetBlendMode(HpdfBlendMode mode)`

---

### AnnotationsDemo - Text Annotations (Sticky Notes)
**Status**: ✅ COMPLETED (2025-10-28)
**Estimated Time**: 1 hour
**Priority**: HIGH

**Why**: Text annotations are implemented but not shown
**What to show**:
- Creating sticky notes with different icons (Comment, Key, Note, Help, NewParagraph, Paragraph, Insert)
- Opened vs closed annotations (SetOpened)
- Multiple annotations on a page
- Annotation colors and positioning
- Practical use case: document review/commenting

**Impact**: Important for collaborative document workflows
**Files**: `Haru/Annotations/HpdfTextAnnotation.cs` - 7 icon types available
**API Methods**:
- `new HpdfTextAnnotation(xref, rect, text, icon)`
- `SetOpened(bool open)`

---

## Medium Priority - Color & Advanced Graphics

### ColorSpacesDemo - CMYK and Advanced Color
**Status**: ✅ COMPLETED (2025-10-28)
**Estimated Time**: 1-2 hours
**Priority**: MEDIUM

**Why**: Only RGB and grayscale demonstrated; CMYK is critical for print
**What to show**:
- CMYK color model (SetCmykFill, SetCmykStroke)
- Side-by-side comparison: RGB vs CMYK
- Grayscale mode (SetGrayFill, SetGrayStroke)
- Rich blacks and color mixing
- Print-ready documents
- Color swatches showing different color spaces

**Impact**: Essential for professional printing workflows
**Files**: `Haru/Doc/HpdfPageGraphics.cs`
**API Methods**:
- `SetCmykFill(float c, float m, float y, float k)`
- `SetCmykStroke(float c, float m, float y, float k)`
- `SetGrayFill(float gray)`
- `SetGrayStroke(float gray)`

---

### PDF_A_Demo - Archival PDF Creation
**Status**: ✅ COMPLETED (2025-10-28)
**Estimated Time**: 1 hour
**Priority**: MEDIUM

**Why**: PDF/A compliance is implemented but not demonstrated
**What to show**:
- Creating PDF/A-1b compliant documents
- Embedding required metadata (XMP)
- Font embedding requirements
- Output intent profiles
- Use case: long-term archival
- Validation and compliance

**Impact**: Critical for archival, legal, and regulatory compliance
**Files**: Based on REMAINING.md, PDF/A Phase 1 is complete

---

### AdvancedTextDemo - Text Layout Features
**Status**: ✅ COMPLETED (2025-10-28)
**Estimated Time**: 2 hours
**Priority**: MEDIUM

**Why**: Several text features aren't demonstrated
**What to show**:
- Text alignment (left, right, center) if available
- Text in rectangles with alignment
- Multi-column text layout
- Combining text with graphics
- Text clipping paths
- Text rendering modes (fill, stroke, fill+stroke, invisible)

**Impact**: Professional document layout capabilities
**Files**: `Haru/Doc/HpdfPageText.cs`
**API Methods**:
- Text rendering modes
- Text positioning
- Text transformations

---

## Lower Priority - Specialized Features

### PageLayoutDemo - Page Display Modes
**Status**: Planned
**Estimated Time**: 30 minutes
**Priority**: LOW

**What to show**:
- Page layout modes (single page, continuous, two-page spread)
- Page mode settings (UseNone, UseOutlines, UseThumbs, FullScreen)
- Initial view settings
- Practical use cases for each mode

**Impact**: Controls user experience when opening PDFs
**Files**: `Haru/Doc/HpdfCatalog.cs`
**API Methods**:
- `SetPageLayout()`
- `SetPageMode()`

---

### MetadataDemo - Document Properties
**Status**: ✅ COMPLETED (2025-10-28)
**Estimated Time**: 30 minutes
**Priority**: LOW

**What to show**:
- All Info dictionary fields (Title, Author, Subject, Keywords, Creator, Producer)
- Creation and modification dates
- Trapped status
- Custom metadata
- How metadata appears in PDF viewers

**Impact**: Professional document management and SEO
**Files**: `Haru/Doc/HpdfInfo.cs`
**Enum**: `HpdfInfoType` (9 metadata types)

---

### AdvancedImageDemo - Special Image Features
**Status**: Planned
**Estimated Time**: 1 hour
**Priority**: LOW

**What to show**:
- Loading images from memory (LoadPngImageFromMem)
- Raw image data (LoadRawImageFromMem - grayscale, RGB, CMYK)
- CMYK images for print
- Image transformations
- Large image handling

**Impact**: Important for dynamic image generation and print workflows
**Files**: `Haru/Doc/HpdfImage.cs`
**API Methods**:
- `LoadPngImageFromMem()`
- `LoadRawImageFromMem()`

---

### CompressionDemo - File Size Optimization
**Status**: ✅ COMPLETED (2025-10-28)
**Estimated Time**: 30 minutes
**Priority**: LOW

**What to show**:
- Different compression modes (None, Text, Image, Metadata, All)
- When to use each mode
- Usage guidelines and best practices
- Mode combinations

**Impact**: Helps users optimize PDF file sizes
**Files**: `HpdfCompressionMode` enum
**API Methods**:
- `SetCompressionMode(HpdfCompressionMode mode)`

---

## Demo Implementation Notes

### General Guidelines
- Keep each demo focused on one feature area
- Use practical, real-world examples
- Include comments explaining what's happening
- Show both basic and advanced usage
- Create visually appealing output
- Keep code under 500-600 lines if possible
- Add to LAST.md after completion

### Demo Structure Template
```csharp
public static class [Feature]Demo
{
    public static void Run()
    {
        try
        {
            var pdf = new HpdfDocument();
            pdf.SetCompressionMode(HpdfCompressionMode.All);

            var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
            var boldFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");

            // Create demo pages
            CreatePage1(pdf, font, boldFont);
            CreatePage2(pdf, font, boldFont);

            pdf.SaveToFile($"pdfs/{Feature}Demo.pdf");
            Console.WriteLine($"{Feature}Demo completed successfully!");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error in {Feature}Demo: {e.Message}");
            Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
            throw;
        }
    }
}
```

---

## Summary

**Total Planned Demos**: 8
**High Priority**: 2 (TransparencyDemo, AnnotationsDemo)
**Medium Priority**: 3 (ColorSpacesDemo, PDF_A_Demo, AdvancedTextDemo)
**Low Priority**: 3 (PageLayoutDemo, MetadataDemo, AdvancedImageDemo, CompressionDemo)

**Next to Implement**: TransparencyDemo
