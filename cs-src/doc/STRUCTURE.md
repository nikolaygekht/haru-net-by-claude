# Library Architecture

[← Back to Documentation Index](INDEX.md)

---

## Overview

Haru.NET is organized around a clean, hierarchical architecture that mirrors the PDF specification while providing a developer-friendly API. The library uses modern .NET design patterns including extension methods, polymorphic interfaces, and SOLID principles.

---

## High-Level Architecture

### System Overview

```mermaid
graph TB
    subgraph "User Code"
        APP[Application]
    end

    subgraph "Haru.NET Public API"
        DOC[HpdfDocument]
        PAGE[HpdfPage]
        FONT[HpdfFont]
        IMG[HpdfImage]
        EXT[Extension Methods]
    end

    subgraph "Internal Systems"
        XREF[HpdfXref<br/>Cross-Reference Table]
        OBJECTS[PDF Objects<br/>Dict, Array, Stream]
        FONTS[Font Implementations<br/>Standard, TrueType, CID]
        STREAMS[Stream Management<br/>Compression]
    end

    subgraph "External Dependencies"
        STB[StbImageSharp<br/>Image Parsing]
        CP[CodePages<br/>Encodings]
        ZIP[System.IO.Compression<br/>Deflate]
    end

    APP --> DOC
    APP --> PAGE
    APP --> FONT
    APP --> IMG
    APP --> EXT

    DOC --> XREF
    PAGE --> XREF
    FONT --> FONTS
    IMG --> STB

    XREF --> OBJECTS
    OBJECTS --> STREAMS
    STREAMS --> ZIP
    FONTS --> CP

    style APP fill:#e1f5ff
    style DOC fill:#fff3e0
    style PAGE fill:#fff3e0
    style FONT fill:#fff3e0
    style IMG fill:#fff3e0
    style EXT fill:#fff3e0
```

---

## Core Class Hierarchy

### Document Object Model

```mermaid
classDiagram
    class HpdfDocument {
        +HpdfXref Xref
        +HpdfCatalog Catalog
        +HpdfInfo Info
        +HpdfVersion Version
        +AddPage() HpdfPage
        +SaveToFile(filename)
        +SaveToMemory() byte[]
        +SetCompressionMode(mode)
        +SetEncryption(password, permissions)
    }

    class HpdfPage {
        +HpdfDict Dict
        +HpdfStreamObject Contents
        +HpdfGraphicsState GraphicsState
        +float Width
        +float Height
        +SetSize(width, height)
        +SetFontAndSize(font, size)
        +MoveTo(x, y)
        +LineTo(x, y)
        +BeginText()
        +EndText()
    }

    class HpdfPages {
        +HpdfDict Dict
        +List~HpdfPage~ Kids
        +AddPage(page)
        +GetPageCount() int
    }

    class HpdfCatalog {
        +HpdfDict Dict
        +HpdfPages Pages
        +SetPageLayout(layout)
        +SetPageMode(mode)
    }

    class HpdfXref {
        +AddObject(obj) int
        +GetObject(id) HpdfObject
        +WriteTo(stream)
    }

    HpdfDocument "1" --> "1" HpdfXref : manages
    HpdfDocument "1" --> "1" HpdfCatalog : contains
    HpdfDocument "1" --> "1" HpdfPages : root pages
    HpdfCatalog "1" --> "1" HpdfPages : references
    HpdfPages "1" --> "*" HpdfPage : contains
    HpdfPage "1" --> "1" HpdfPages : parent
    HpdfXref "1" --> "*" HpdfObject : tracks
```

### Extension Methods Organization

```mermaid
graph LR
    subgraph "Target Classes"
        PAGE[HpdfPage]
        DOC[HpdfDocument]
    end

    subgraph "Extension Method Classes"
        GRAPHICS[HpdfPageGraphics<br/>Graphics Operations]
        SHAPES[HpdfPageShapes<br/>Shape Drawing]
        TEXT[HpdfPageText<br/>Text Operations]
        PAGEEXT[HpdfPageExtensions<br/>Convenience Methods]
        DOCEXT[HpdfDocumentExtensions<br/>Document Operations]
    end

    GRAPHICS -.extends.-> PAGE
    SHAPES -.extends.-> PAGE
    TEXT -.extends.-> PAGE
    PAGEEXT -.extends.-> PAGE
    DOCEXT -.extends.-> DOC

    style PAGE fill:#fff3e0
    style DOC fill:#fff3e0
    style GRAPHICS fill:#e8f5e9
    style SHAPES fill:#e8f5e9
    style TEXT fill:#e8f5e9
    style PAGEEXT fill:#e8f5e9
    style DOCEXT fill:#e8f5e9
```

**Extension Method Categories:**

- **HpdfPageGraphics** - Low-level graphics primitives
  - Path operations: `MoveTo`, `LineTo`, `CurveTo`, `Rectangle`, `Circle`, `Arc`
  - Color: `SetRgbStroke`, `SetRgbFill`, `SetCmykStroke`, `SetCmykFill`
  - Line styles: `SetLineWidth`, `SetLineCap`, `SetLineJoin`, `SetDash`
  - Transformations: `Concat`, `SetTextMatrix`
  - Path painting: `Stroke`, `Fill`, `FillStroke`, `Clip`
  - State: `GSave`, `GRestore`

- **HpdfPageShapes** - High-level shape drawing
  - `Circle(x, y, radius)`
  - `Ellipse(x, y, xRadius, yRadius)`
  - `Arc(x, y, radius, startAngle, endAngle)`

- **HpdfPageText** - Text operations
  - `BeginText`, `EndText`
  - `MoveTextPos(x, y)`
  - `ShowText(text)`
  - `ShowTextNextLine(text)`
  - `SetTextLeading(leading)`
  - `SetTextRenderingMode(mode)`

- **HpdfPageExtensions** - Convenience methods
  - `DrawImage(image, x, y, width, height)`

- **HpdfDocumentExtensions** - Document-level operations
  - `GetFont(fontPath, encoding)` - Load TrueType/Type1 fonts
  - `LoadPngImageFromFile(path)` - Load PNG images
  - `LoadJpegImageFromFile(path)` - Load JPEG images

---

## Font System Architecture

### Font Interface Hierarchy

```mermaid
classDiagram
    class IHpdfFontImplementation {
        <<interface>>
        +HpdfDict Dict
        +string BaseFont
        +string LocalName
        +int? CodePage
        +int Ascent
        +int Descent
        +int XHeight
        +HpdfBox FontBBox
        +GetCharWidth(charCode) float
        +MeasureText(text, fontSize) float
        +ConvertTextToGlyphIDs(text) byte[]
    }

    class HpdfFont {
        +IHpdfFontImplementation Implementation
        +HpdfDict Dict
        +string BaseFont
        +string LocalName
        +MeasureText(text, fontSize) float
        +GetCharWidth(charCode) float
    }

    class HpdfStandardFontImpl {
        +HpdfStandardFont FontType
        -FontMetrics metrics
    }

    class HpdfTrueTypeFontImpl {
        +string FontPath
        +int CodePage
        -TrueTypeParser parser
        -GlyphMetrics glyphs
    }

    class HpdfType1FontImpl {
        +string AfmPath
        +string PfbPath
        -AfmParser parser
        -Type1Metrics metrics
    }

    class HpdfCIDFont {
        +string CmapName
        +int CodePage
        -CIDFontDescriptor descriptor
    }

    HpdfFont "1" --> "1" IHpdfFontImplementation : wraps
    IHpdfFontImplementation <|.. HpdfStandardFontImpl : implements
    IHpdfFontImplementation <|.. HpdfTrueTypeFontImpl : implements
    IHpdfFontImplementation <|.. HpdfType1FontImpl : implements
    IHpdfFontImplementation <|.. HpdfCIDFont : implements
```

### Font Types and Use Cases

| Font Type | Implementation | Use Case | Embedding | Code Pages |
|-----------|---------------|----------|-----------|------------|
| **Standard** | HpdfStandardFontImpl | Base14 PDF fonts | No | - |
| **TrueType** | HpdfTrueTypeFontImpl | Custom .ttf fonts | Yes | CP1251-1258 |
| **Type1** | HpdfType1FontImpl | PostScript fonts | Yes | Standard |
| **CID** | HpdfCIDFont | CJK languages | Yes | CP932, CP936, CP949, CP950 |

**Base14 Standard Fonts:**
- Times-Roman, Times-Bold, Times-Italic, Times-BoldItalic
- Helvetica, Helvetica-Bold, Helvetica-Oblique, Helvetica-BoldOblique
- Courier, Courier-Bold, Courier-Oblique, Courier-BoldOblique
- Symbol, ZapfDingbats

---

## PDF Object Hierarchy

### Object Model

```mermaid
classDiagram
    class HpdfObject {
        <<abstract>>
        +int ObjectID
        +int Generation
        +WriteTo(stream)
    }

    class HpdfNull {
        +WriteTo(stream)
    }

    class HpdfBoolean {
        +bool Value
        +WriteTo(stream)
    }

    class HpdfNumber {
        +float Value
        +WriteTo(stream)
    }

    class HpdfString {
        +string Value
        +WriteTo(stream)
    }

    class HpdfName {
        +string Value
        +WriteTo(stream)
    }

    class HpdfArray {
        +List~HpdfObject~ Items
        +Add(obj)
        +Get(index) HpdfObject
        +WriteTo(stream)
    }

    class HpdfDict {
        +Dictionary~string,HpdfObject~ Items
        +Add(key, value)
        +Get(key) HpdfObject
        +WriteTo(stream)
    }

    class HpdfStream {
        +HpdfDict Dict
        +HpdfMemoryStream Stream
        +WriteTo(stream)
    }

    HpdfObject <|-- HpdfNull
    HpdfObject <|-- HpdfBoolean
    HpdfObject <|-- HpdfNumber
    HpdfObject <|-- HpdfString
    HpdfObject <|-- HpdfName
    HpdfObject <|-- HpdfArray
    HpdfObject <|-- HpdfDict
    HpdfDict <|-- HpdfStream

    HpdfArray "1" --> "*" HpdfObject : contains
    HpdfDict "1" --> "*" HpdfObject : contains
    HpdfStream "1" --> "1" HpdfMemoryStream : has
```

**Object Types:**

- **HpdfNull** - PDF null object
- **HpdfBoolean** - PDF boolean (true/false)
- **HpdfNumber** - PDF numeric values (integer or real)
- **HpdfString** - PDF strings (literal or hexadecimal)
- **HpdfName** - PDF name objects (identifiers)
- **HpdfArray** - PDF arrays (ordered collections)
- **HpdfDict** - PDF dictionaries (key-value pairs)
- **HpdfStream** - PDF streams (dictionary + binary data)

---

## Graphics State Management

### Graphics State Structure

```mermaid
graph TB
    subgraph "Graphics State Stack"
        GS1[Graphics State 1<br/>Initial]
        GS2[Graphics State 2<br/>After GSave]
        GS3[Graphics State 3<br/>After GSave]
    end

    subgraph "State Properties"
        COLOR[Color<br/>RGB/CMYK]
        LINE[Line Style<br/>Width, Dash, Cap, Join]
        FONT[Font & Size]
        TRANS[Transformation Matrix]
        BLEND[Blend Mode & Opacity]
    end

    GS1 --> GS2
    GS2 --> GS3

    GS3 -.contains.-> COLOR
    GS3 -.contains.-> LINE
    GS3 -.contains.-> FONT
    GS3 -.contains.-> TRANS
    GS3 -.contains.-> BLEND

    GRESTORE[GRestore] -.pops.-> GS3

    style GS1 fill:#e3f2fd
    style GS2 fill:#bbdefb
    style GS3 fill:#90caf9
```

**Graphics State Properties:**

```csharp
public class HpdfGraphicsState
{
    // Color
    public HpdfRgbColor RgbStroke { get; set; }
    public HpdfRgbColor RgbFill { get; set; }
    public HpdfCmykColor CmykStroke { get; set; }
    public HpdfCmykColor CmykFill { get; set; }

    // Line style
    public float LineWidth { get; set; }
    public HpdfLineCap LineCap { get; set; }
    public HpdfLineJoin LineJoin { get; set; }
    public float MiterLimit { get; set; }
    public DashPattern Dash { get; set; }

    // Font
    public HpdfFont? Font { get; set; }
    public float FontSize { get; set; }

    // Text
    public float CharSpace { get; set; }
    public float WordSpace { get; set; }
    public float HorizontalScaling { get; set; }
    public float TextLeading { get; set; }
    public HpdfTextRenderingMode TextRenderingMode { get; set; }

    // Transformation
    public HpdfTransMatrix TransMatrix { get; set; }

    // Transparency
    public HpdfBlendMode BlendMode { get; set; }
    public float StrokeAlpha { get; set; }
    public float FillAlpha { get; set; }
}
```

---

## Document Creation Workflow

### Typical Document Creation Flow

```mermaid
sequenceDiagram
    actor User
    participant Doc as HpdfDocument
    participant Xref as HpdfXref
    participant Page as HpdfPage
    participant Stream as Content Stream
    participant File as PDF File

    User->>Doc: new HpdfDocument()
    Doc->>Xref: Initialize Xref
    Doc->>Doc: Create Catalog
    Doc->>Doc: Create Root Pages

    User->>Doc: new HpdfFont(...)
    Doc->>Xref: Register Font

    User->>Doc: AddPage()
    Doc->>Page: Create Page
    Doc->>Xref: Register Page
    Page->>Stream: Create Content Stream
    Doc-->>User: return Page

    User->>Page: SetFontAndSize(font, 24)
    Page->>Stream: Write font operator

    User->>Page: BeginText()
    Page->>Stream: Write BT

    User->>Page: MoveTextPos(100, 700)
    Page->>Stream: Write Td operator

    User->>Page: ShowText("Hello")
    Page->>Stream: Write Tj operator

    User->>Page: EndText()
    Page->>Stream: Write ET

    User->>Page: Rectangle(50, 50, 200, 100)
    Page->>Stream: Write re operator

    User->>Page: Stroke()
    Page->>Stream: Write S operator

    User->>Doc: SaveToFile("output.pdf")
    Doc->>Xref: Generate Xref Table
    Doc->>File: Write Header
    Doc->>File: Write Objects
    Doc->>File: Write Xref
    Doc->>File: Write Trailer
    File-->>User: PDF Created
```

---

## Type System

### Geometric Types

```csharp
// 2D Point
public struct HpdfPoint
{
    public float X { get; set; }
    public float Y { get; set; }
}

// Rectangle (x, y, width, height)
public struct HpdfRect
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}

// Bounding Box (left, bottom, right, top)
public struct HpdfBox
{
    public float Left { get; set; }
    public float Bottom { get; set; }
    public float Right { get; set; }
    public float Top { get; set; }
}

// 2D Transformation Matrix
public struct HpdfTransMatrix
{
    public float A { get; set; }  // Horizontal scaling
    public float B { get; set; }  // Vertical skewing
    public float C { get; set; }  // Horizontal skewing
    public float D { get; set; }  // Vertical scaling
    public float X { get; set; }  // Horizontal translation
    public float Y { get; set; }  // Vertical translation
}
```

### Color Types

```csharp
// RGB Color (0.0 to 1.0)
public struct HpdfRgbColor
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
}

// CMYK Color (0.0 to 1.0)
public struct HpdfCmykColor
{
    public float C { get; set; }
    public float M { get; set; }
    public float Y { get; set; }
    public float K { get; set; }
}
```

### Page Types

```csharp
// Standard Page Sizes
public enum HpdfPageSize
{
    Letter,      // 612 x 792 points
    Legal,       // 612 x 1008 points
    A3,          // 842 x 1191 points
    A4,          // 595 x 842 points
    A5,          // 420 x 595 points
    B4,          // 729 x 1032 points
    B5,          // 516 x 729 points
    // ... more sizes
}

// Page Orientation
public enum HpdfPageDirection
{
    Portrait,
    Landscape
}
```

---

## Stream Processing

### Compression Pipeline

```mermaid
graph LR
    INPUT[Content Data] --> MEMORY[HpdfMemoryStream]
    MEMORY --> CHECK{Compression<br/>Enabled?}
    CHECK -->|Yes| DEFLATE[DeflateStream<br/>Compression]
    CHECK -->|No| RAW[Raw Data]
    DEFLATE --> FILTER[/FlateDecode Filter/]
    RAW --> OUTPUT[PDF Stream Object]
    FILTER --> OUTPUT

    style INPUT fill:#e1f5ff
    style OUTPUT fill:#c8e6c9
    style DEFLATE fill:#fff3e0
```

**Compression Modes:**

```csharp
[Flags]
public enum HpdfCompressionMode
{
    None = 0,
    Text = 1,      // Compress page content streams
    Image = 2,     // Compress image data
    Metadata = 4,  // Compress metadata streams
    All = 7        // Compress everything
}
```

---

## Encryption Architecture

### Encryption Flow

```mermaid
graph TB
    DOC[HpdfDocument] --> ENCRYPT{SetEncryption<br/>Called?}
    ENCRYPT -->|Yes| MODE{Encryption<br/>Mode}
    ENCRYPT -->|No| NOSEC[No Encryption]

    MODE --> R2[R2 - RC4 40-bit]
    MODE --> R3[R3 - RC4 128-bit]
    MODE --> R4[R4 - AES 128-bit]

    R2 --> DICT[HpdfEncryptDict]
    R3 --> DICT
    R4 --> DICT

    DICT --> PERMS[Permissions]
    DICT --> KEYS[User/Owner Keys]

    PERMS --> PRINT[Print]
    PERMS --> COPY[Copy]
    PERMS --> MODIFY[Modify]
    PERMS --> ANNOT[Annotate]

    style DOC fill:#fff3e0
    style DICT fill:#e1f5ff
    style R4 fill:#c8e6c9
```

**Permission Flags:**

```csharp
[Flags]
public enum HpdfPermission
{
    Read = 0,
    Print = 4,
    Modify = 8,
    Copy = 16,
    Annotate = 32,
    FillForm = 256,
    Extract = 512,
    Assemble = 1024
}
```

---

## Design Patterns

### Patterns Used in Haru.NET

**1. Composite Pattern** - Document/Pages/Page hierarchy
```csharp
HpdfDocument → HpdfPages → HpdfPage (tree structure)
```

**2. Strategy Pattern** - Font implementations
```csharp
IHpdfFontImplementation → HpdfStandardFontImpl, HpdfTrueTypeFontImpl, etc.
```

**3. Builder Pattern** - Page content construction
```csharp
page.BeginText()
    .SetFontAndSize(font, 12)
    .MoveTextPos(100, 700)
    .ShowText("Hello")
    .EndText();
```

**4. Extension Method Pattern** - API organization
```csharp
static class HpdfPageGraphics extends HpdfPage
static class HpdfPageText extends HpdfPage
```

**5. Facade Pattern** - HpdfFont wraps IHpdfFontImplementation
```csharp
HpdfFont → IHpdfFontImplementation (simplified interface)
```

**6. Object Pool Pattern** - Xref table manages PDF objects
```csharp
HpdfXref tracks and reuses object IDs
```

---

## Porting from C to .NET

### Key Differences from Original C Library

| Aspect | C Library | .NET Port |
|--------|-----------|-----------|
| **Memory Management** | Manual malloc/free | Automatic GC |
| **Strings** | char* pointers | System.String |
| **Collections** | Manual arrays | List<T>, Dictionary<K,V> |
| **Compression** | zlib library | System.IO.Compression |
| **Error Handling** | Error codes | Exceptions |
| **Buffers** | Manual buffer management | Span<T>, Memory<T> |
| **Encoding** | Manual encoding | Encoding.GetEncoding() |

### .NET-Specific Enhancements

- **Fluent API** - Method chaining where appropriate
- **Properties** - C-style getters/setters converted to properties
- **Extension Methods** - Organized API without monolithic classes
- **Nullable Reference Types** - Compile-time null safety
- **Span<T>** - Efficient buffer operations without allocation
- **async/await** - Future enhancement for I/O operations

---

## Performance Considerations

### Optimization Strategies

**1. Object Reuse**
```csharp
// Good - reuse font across pages
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
foreach (var page in pages)
{
    page.SetFontAndSize(font, 12);
}
```

**2. Compression**
```csharp
// Enable compression for smaller files
pdf.SetCompressionMode(HpdfCompressionMode.All);
```

**3. Stream Writing**
```csharp
// Content streams write directly to memory
// No intermediate string allocations
```

**4. Font Metric Caching**
```csharp
// Font metrics loaded once and cached
// Fast text measurement operations
```

---

## Extending Haru.NET

### Adding Custom Functionality

**Custom Extension Methods:**

```csharp
public static class MyPageExtensions
{
    public static void DrawGrid(this HpdfPage page,
        float spacing, HpdfRgbColor color)
    {
        page.GSave();
        page.SetRgbStroke(color.R, color.G, color.B);
        page.SetLineWidth(0.5f);

        for (float x = 0; x < page.Width; x += spacing)
        {
            page.MoveTo(x, 0);
            page.LineTo(x, page.Height);
            page.Stroke();
        }

        for (float y = 0; y < page.Height; y += spacing)
        {
            page.MoveTo(0, y);
            page.LineTo(page.Width, y);
            page.Stroke();
        }

        page.GRestore();
    }
}
```

**Custom Font Implementation:**

```csharp
public class MyCustomFont : IHpdfFontImplementation
{
    public HpdfDict Dict { get; }
    public string BaseFont { get; }
    public string LocalName { get; }
    // ... implement interface methods
}
```

---

## Related Documentation

### Core Concepts
- [Quick Start Guide](USAGE.md) - Basic usage patterns
- [Getting Started](guides/GettingStarted.md) - Step-by-step tutorial

### Detailed Guides
- [Fonts Guide](guides/FontsGuide.md) - Font system in detail
- [Graphics Guide](guides/GraphicsGuide.md) - Graphics operations
- [Text Guide](guides/TextGuide.md) - Text rendering

### API Reference
- [HpdfDocument](api/core/HpdfDocument.md) - Document class reference
- [HpdfPage](api/core/HpdfPage.md) - Page class reference
- [HpdfFont](api/core/HpdfFont.md) - Font class reference

---

[← Back to Documentation Index](INDEX.md)

*Last updated: 2025-10-31*
