# CIDFontType0 Implementation Plan for Haru.NET

**Date**: 2025-01-14
**Status**: Planning Phase
**Goal**: Implement CIDFontType0 (predefined CJK fonts) alongside existing CIDFontType2 (embedded TrueType) support

## Executive Summary

This plan outlines the implementation of CIDFontType0 support for predefined CJK fonts in Haru.NET. This will provide users with two options:
- **CIDFontType0**: Predefined fonts with built-in metrics (smaller PDFs, requires viewer font support)
- **CIDFontType2**: Embedded TrueType fonts (larger PDFs, guaranteed rendering)

**Estimated Effort**: 3-4 days
**Priority**: MEDIUM
**Complexity**: HIGH (requires porting large data tables and encoder infrastructure)

---

## Table of Contents

1. [Background and Analysis](#background-and-analysis)
2. [Architecture Design](#architecture-design)
3. [Implementation Phases](#implementation-phases)
4. [File Structure](#file-structure)
5. [Data Porting Strategy](#data-porting-strategy)
6. [API Design](#api-design)
7. [Testing Strategy](#testing-strategy)
8. [Timeline and Milestones](#timeline-and-milestones)
9. [Risk Assessment](#risk-assessment)

---

## Background and Analysis

### What is CIDFontType0?

CIDFontType0 is a PDF font type based on Type 1 (PostScript) fonts with CID (Character ID) keying. Unlike CIDFontType2 (which embeds TrueType fonts), CIDFontType0:

- **Does NOT embed font data** - references system fonts by name
- **Has predefined metrics** - ascent, descent, widths hard-coded
- **Uses CID encoding** - maps byte codes to CID values
- **Requires encoders** - separate encoding tables (CP936, CP932, CP949, CP950)

### C Source Code Analysis

**Files to Port**:

| Category | Files | Size | Lines | Purpose |
|----------|-------|------|-------|---------|
| **Font Definitions** | hpdf_fontdef_cns.c | - | 474 | SimSun, SimHei (Chinese Simplified) |
| | hpdf_fontdef_cnt.c | - | 253 | MingLiU (Chinese Traditional) |
| | hpdf_fontdef_jp.c | - | 1,907 | MS-Gothic, MS-Mincho, MS-PGothic, MS-PMincho (Japanese) |
| | hpdf_fontdef_kr.c | - | 1,575 | Batang, BatangChe, Dotum, DotumChe (Korean) |
| | hpdf_fontdef_cid.c | - | 197 | Base CID font definition infrastructure |
| **Encoders** | hpdf_encoder_cns.c | 845KB | ~36,326 | CP936 (Chinese Simplified) encoding tables |
| | hpdf_encoder_cnt.c | 332KB | ~15,285 | CP950 (Chinese Traditional) encoding tables |
| | hpdf_encoder_jp.c | 351KB | ~16,086 | CP932 (Japanese) encoding tables |
| | hpdf_encoder_kr.c | 642KB | ~27,933 | CP949 (Korean) encoding tables |
| **Total** | 9 files | ~2.1MB | ~98,436 | Full CIDFontType0 support |

**Data Structures**:

```c
// Width array (per-CID character widths)
typedef struct {
    HPDF_UINT16 cid;
    HPDF_INT16 width;
} HPDF_CID_Width;

// Font metrics
typedef struct {
    int ascent;
    int descent;
    int cap_height;
    HPDF_Box font_bbox;
    int flags;
    int italic_angle;
    int stemv;
    HPDF_CID_Width *widths;
} HPDF_CIDFontDefAttr;

// Encoder (byte code → CID/Unicode mapping)
typedef struct {
    HPDF_UnicodeMap_Rec *unicode_map;  // 65536-element array
    char *registry;
    char *ordering;
    int supplement;
    int writing_mode;
} HPDF_CMapEncoderAttr;
```

### Predefined Fonts

**36 Total Fonts** (4 styles × font families):

| Language | Font Family | Styles | Total |
|----------|-------------|--------|-------|
| Chinese Simplified (CNS) | SimSun | Normal, Bold, Italic, BoldItalic | 4 |
| | SimHei | Normal, Bold, Italic, BoldItalic | 4 |
| Chinese Traditional (CNT) | MingLiU | Normal, Bold, Italic, BoldItalic | 4 |
| Japanese (JP) | MS-Gothic | Normal, Bold, Italic, BoldItalic | 4 |
| | MS-Mincho | Normal, Bold, Italic, BoldItalic | 4 |
| | MS-PGothic | Normal, Bold, Italic, BoldItalic | 4 |
| | MS-PMincho | Normal, Bold, Italic, BoldItalic | 4 |
| Korean (KR) | Batang | Normal, Bold, Italic, BoldItalic | 4 |
| | BatangChe | Normal, Bold, Italic, BoldItalic | 4 |
| | Dotum | Normal, Bold, Italic, BoldItalic | 4 |
| | DotumChe | Normal, Bold, Italic, BoldItalic | 4 |

**Style Variations**:
- Bold: Increase `StemV` × 2, add `HPDF_FONT_FORCE_BOLD` flag
- Italic: Set `ItalicAngle` = -11°, add `HPDF_FONT_ITALIC` flag
- BoldItalic: Apply both modifications

---

## Architecture Design

### Component Hierarchy

```
HpdfCIDFontType0 (new)
├── IHpdfFontImplementation (existing interface)
├── PredefinedFontDefinition (new)
│   ├── FontMetrics
│   └── WidthArray[]
├── CIDEncoder (new)
│   └── UnicodeMap[65536]
└── HpdfFont (existing wrapper)

Existing:
HpdfCIDFont (CIDFontType2 - TrueType-based)
├── IHpdfFontImplementation
├── TrueType parsing
└── Font embedding
```

### Class Diagram

```
┌─────────────────────────────┐
│ IHpdfFontImplementation     │  (interface - existing)
├─────────────────────────────┤
│ + Dict                      │
│ + BaseFont                  │
│ + LocalName                 │
│ + CodePage                  │
│ + Ascent                    │
│ + Descent                   │
│ + XHeight                   │
│ + FontBBox                  │
│ + GetCharWidth(byte)        │
│ + MeasureText(string,float) │
└─────────────────────────────┘
         △               △
         │               │
         │               │
┌────────┴──────┐ ┌──────┴───────────┐
│ HpdfCIDFont   │ │ HpdfCIDFontType0 │  (new)
│ (Type2/TT)    │ │ (Type0/Predef)   │
├───────────────┤ ├──────────────────┤
│ - TrueType    │ │ - FontDef        │
│   parsing     │ │ - Encoder        │
│ - Font        │ │ - Metrics        │
│   embedding   │ │ - Width arrays   │
└───────────────┘ └──────────────────┘
```

### Data Storage Strategy

**Option 1: Resource Files** (RECOMMENDED)
- Store width arrays and encoders as embedded resources
- Load on demand (lazy initialization)
- Benefits: Smaller assembly, faster startup
- Implementation: JSON or binary serialization

**Option 2: Code Generation**
- Generate C# classes from C source
- Hard-code all data as static arrays
- Benefits: Type-safe, no deserialization
- Drawbacks: Very large files (~98K lines)

**Option 3: Hybrid**
- Width arrays as resources (small, ~10KB per font)
- Encoders as resources (large, ~2MB total)
- Metrics hard-coded (tiny, ~1KB total)
- **RECOMMENDED APPROACH**

---

## Implementation Phases

### Phase 1: Infrastructure (Day 1)

**Goal**: Create base classes and data structures

**Tasks**:
1. Create `HpdfCIDFontType0.cs` class
   - Implements `IHpdfFontImplementation`
   - Constructor accepts font name and encoder
   - PDF dictionary creation (Type 0 → CIDFontType0)

2. Create `PredefinedFontDefinition.cs`
   - Stores font metrics (ascent, descent, bbox, etc.)
   - Width array structure
   - Factory method for loading definitions

3. Create `CIDEncoder.cs` base class
   - Unicode mapping interface
   - Byte code → CID conversion
   - CMap generation for PDF

4. Create data model classes:
   - `CIDWidth.cs` (CID + width pair)
   - `FontMetrics.cs` (ascent, descent, etc.)
   - `CIDSystemInfo.cs` (registry, ordering, supplement)

**Estimated Effort**: 4-6 hours

---

### Phase 2: Font Definitions (Day 1-2)

**Goal**: Port predefined font metrics and widths

**Tasks**:
1. Port `hpdf_fontdef_cid.c` base infrastructure
   - `GetCIDWidth()` method
   - Width array management
   - Style modification (Bold/Italic)

2. Port Chinese Simplified (CNS) fonts
   - Extract `SIMSUN_W_ARRAY` → JSON/binary
   - Extract `SIMHEI_W_ARRAY` → JSON/binary
   - Create `SimSun_Init()`, `SimHei_Init()` equivalents

3. Port Chinese Traditional (CNT) fonts
   - Extract `MINGLIU_W_ARRAY` → JSON/binary
   - Create `MingLiU_Init()` equivalent

4. Port Japanese (JP) fonts
   - Extract width arrays for MS-Gothic, MS-Mincho, MS-PGothic, MS-PMincho
   - Create initialization methods

5. Port Korean (KR) fonts
   - Extract width arrays for Batang, BatangChe, Dotum, DotumChe
   - Create initialization methods

**Data Extraction Script** (Python/C#):
```python
# Extract width array from C source
def extract_width_array(c_file, array_name):
    pattern = rf'static const HPDF_CID_Width {array_name}\[\] = \{{(.*?)\{{0xFFFF, 0\}}\}};'
    match = re.search(pattern, c_file, re.DOTALL)

    widths = []
    for line in match.group(1).split('\n'):
        m = re.match(r'\s*\{(\d+),\s*(\d+)\}', line)
        if m:
            widths.append({'cid': int(m.group(1)), 'width': int(m.group(2))})

    return widths

# Save as JSON
import json
with open(f'{array_name}.json', 'w') as f:
    json.dump(widths, f)
```

**Estimated Effort**: 8-10 hours

---

### Phase 3: Encoders (Day 2-3)

**Goal**: Port CJK encoding tables

**Challenges**:
- **MASSIVE data tables** (65,536 Unicode mappings per encoder)
- Each encoder file is 300-800KB of C code
- Need efficient storage and lookup

**Tasks**:
1. Design encoder data format
   - Option A: Compressed binary (BitArray for sparse mappings)
   - Option B: JSON with run-length encoding
   - Option C: SQLite database
   - **RECOMMENDED**: Binary format with RLE compression

2. Port Chinese Simplified encoder (CP936)
   - Extract `CP936_UNICODE_ARRAY` (36,326 lines!)
   - Compress and serialize
   - Implement `GBKEucHEncoder.cs`

3. Port Chinese Traditional encoder (CP950)
   - Extract `ETenB5H_UNICODE_ARRAY`
   - Implement `ETenB5HEncoder.cs`

4. Port Japanese encoder (CP932)
   - Extract `EUC_H_UNICODE_ARRAY`, `90ms_RKSJ_H_UNICODE_ARRAY`
   - Implement `EucHEncoder.cs`, `RksjHEncoder.cs`

5. Port Korean encoder (CP949)
   - Extract `KSCms_UHC_H_UNICODE_ARRAY`
   - Implement `KscUhcHEncoder.cs`

**Data Compression Strategy**:
```csharp
// Sparse array compression
public class CompressedUnicodeMap
{
    // Store only non-zero mappings
    public Dictionary<ushort, ushort> Mapping { get; set; }

    // Or use run-length encoding
    public class Range
    {
        public ushort Start;
        public ushort End;
        public ushort BaseCid;
    }

    public List<Range> Ranges { get; set; }
}
```

**Estimated Effort**: 12-16 hours

---

### Phase 4: CMap Generation (Day 3)

**Goal**: Generate CMap streams for encoders

**Background**:
- CMap = Character Map (PDF stream object)
- Maps character codes to CID values
- Required for non-Identity-H encodings

**Tasks**:
1. Create `CMapGenerator.cs` for CIDFontType0
   - Generate CMap header
   - Write codespace ranges
   - Write CID ranges
   - Write Unicode mappings

2. Implement CMap formats:
   - `begincodespacerange` / `endcodespacerange`
   - `begincidrange` / `endcidrange`
   - `beginnotdefrange` / `endnotdefrange`

3. Port from `hpdf_font_cid.c::CreateCMap()`
   - Adobe CMap format compliance
   - Efficient range generation

**Example CMap Output**:
```postscript
%!PS-Adobe-3.0 Resource-CMap
/CIDInit /ProcSet findresource begin
12 dict begin
begincmap
/CIDSystemInfo 3 dict dup begin
  /Registry (Adobe) def
  /Ordering (GB1) def
  /Supplement 2 def
end def
/CMapName /GBK-EUC-H def
1 begincodespacerange
<0020> <D7FF>
endcodespacerange
100 begincidrange
<0020> <007E> 1
<00A0> <00FF> 94
...
endcidrange
endcmap
end end
```

**Estimated Effort**: 6-8 hours

---

### Phase 5: Font Registration (Day 4)

**Goal**: Implement UseCNS/CNT/JP/KRFonts() and GetFont()

**Tasks**:
1. Create `PredefinedFontRegistry.cs`
   - Static registry of loaded fonts
   - Font name → PredefinedFontDefinition mapping

2. Implement `HpdfDocument.UseCNSFonts()`
   ```csharp
   public static void UseCNSFonts(this HpdfDocument doc)
   {
       RegisterFont(doc, "SimSun", LoadSimSunDefinition());
       RegisterFont(doc, "SimSun,Bold", LoadSimSunBoldDefinition());
       RegisterFont(doc, "SimSun,Italic", LoadSimSunItalicDefinition());
       RegisterFont(doc, "SimSun,BoldItalic", LoadSimSunBoldItalicDefinition());

       RegisterFont(doc, "SimHei", LoadSimHeiDefinition());
       RegisterFont(doc, "SimHei,Bold", LoadSimHeiBoldDefinition());
       RegisterFont(doc, "SimHei,Italic", LoadSimHeiItalicDefinition());
       RegisterFont(doc, "SimHei,BoldItalic", LoadSimHeiBoldItalicDefinition());
   }
   ```

3. Implement `UseCNTFonts()`, `UseJPFonts()`, `UseKRFonts()` similarly

4. Implement `UseCNSEncodings()`, `UseCNTEncodings()`, etc.
   ```csharp
   public static void UseCNSEncodings(this HpdfDocument doc)
   {
       RegisterEncoder(doc, "GBK-EUC-H", new GBKEucHEncoder());
       RegisterEncoder(doc, "GBKp-EUC-H", new GBKpEucHEncoder());
       // ... other CNS encodings
   }
   ```

5. Update `GetFont()` to support predefined fonts
   ```csharp
   public static HpdfFont GetFont(this HpdfDocument doc, string fontName, string encodingName)
   {
       // Check if predefined font
       if (PredefinedFontRegistry.TryGetFont(fontName, out var fontDef))
       {
           var encoder = GetEncoder(encodingName);
           var cidFont = HpdfCIDFontType0.Create(doc.Xref, fontName, fontDef, encoder);
           return cidFont.AsFont();
       }

       // Fall back to standard fonts or TrueType
       return GetStandardFont(doc, fontName, encodingName);
   }
   ```

**Estimated Effort**: 4-6 hours

---

### Phase 6: Testing and Validation (Day 4)

**Goal**: Comprehensive testing of predefined fonts

**Test Categories**:

1. **Unit Tests** (per font family)
   - Font metrics (ascent, descent, bbox)
   - Width array lookup
   - Style variations (Bold/Italic)
   - Encoder mappings

2. **Integration Tests** (per language)
   - Chinese Simplified text rendering
   - Chinese Traditional text rendering
   - Japanese text rendering
   - Korean text rendering

3. **Comparison Tests**
   - Compare with C library output
   - Byte-for-byte PDF comparison
   - Visual rendering comparison

4. **Performance Tests**
   - Font loading time
   - Encoder lookup performance
   - Memory footprint

**Test Cases**:
```csharp
[Fact]
public void SimSun_Metrics_MatchCSource()
{
    var doc = new HpdfDocument();
    doc.UseCNSFonts();
    var font = doc.GetFont("SimSun", "GBK-EUC-H");

    font.GetAscent().Should().Be(859);
    font.GetDescent().Should().Be(-140);
    font.GetBBox().Should().BeEquivalentTo(new HpdfBox(0, -140, 996, 855));
}

[Fact]
public void SimSun_ChineseText_RendersCorrectly()
{
    var doc = new HpdfDocument();
    doc.UseCNSFonts();
    doc.UseCNSEncodings();

    var page = doc.AddPage();
    var font = doc.GetFont("SimSun", "GBK-EUC-H");

    page.BeginText();
    page.SetFontAndSize(font, 12);
    page.MoveTextPos(100, 700);
    page.ShowText("你好世界"); // Hello World in Chinese
    page.EndText();

    using var ms = new MemoryStream();
    doc.Save(ms);

    // Validate with PdfPig
    ms.Position = 0;
    using var pdfDoc = PdfDocument.Open(ms);
    var pdfPage = pdfDoc.GetPage(1);
    pdfPage.Text.Should().Contain("你好世界");
}
```

**Estimated Effort**: 6-8 hours

---

## File Structure

```
cs-src/Haru/
├── Font/
│   ├── CID/
│   │   ├── HpdfCIDFont.cs              (existing - CIDFontType2)
│   │   ├── HpdfCIDFontType0.cs         (new)
│   │   ├── PredefinedFontDefinition.cs (new)
│   │   ├── PredefinedFontRegistry.cs   (new)
│   │   ├── CIDEncoder.cs               (new - base class)
│   │   ├── GBKEucHEncoder.cs           (new - CP936)
│   │   ├── ETenB5HEncoder.cs           (new - CP950)
│   │   ├── EucHEncoder.cs              (new - CP932)
│   │   ├── KscUhcHEncoder.cs           (new - CP949)
│   │   ├── CMapGenerator.cs            (existing - extend)
│   │   └── Data/                       (new - embedded resources)
│   │       ├── SimSun.json
│   │       ├── SimHei.json
│   │       ├── MingLiU.json
│   │       ├── MSGothic.json
│   │       ├── MSMincho.json
│   │       ├── MSPGothic.json
│   │       ├── MSPMincho.json
│   │       ├── Batang.json
│   │       ├── BatangChe.json
│   │       ├── Dotum.json
│   │       ├── DotumChe.json
│   │       ├── CP936_Encoder.bin       (compressed)
│   │       ├── CP950_Encoder.bin
│   │       ├── CP932_Encoder.bin
│   │       └── CP949_Encoder.bin
│   └── ...
├── Doc/
│   └── HpdfDocumentExtensions.cs       (extend)
└── ...

cs-src/Haru.Test/
└── Font/
    └── CID/
        ├── HpdfCIDFontType0Tests.cs    (new)
        ├── PredefinedFontsIntegrationTests.cs (new)
        ├── EncoderTests.cs             (new)
        └── ...

cs-src/Haru.Demos/
└── PredefinedCJKFontsDemo.cs          (new)

Tools/
└── CIDFontDataExtractor/              (new - Python/C# tool)
    ├── extract_widths.py
    ├── extract_encoders.py
    ├── compress_encoders.py
    └── generate_resources.py
```

---

## Data Porting Strategy

### Step 1: Extract Width Arrays

**Script**: `extract_widths.py`

```python
import re
import json
import sys

def extract_width_array(c_file_path, array_name, output_json):
    with open(c_file_path, 'r') as f:
        content = f.read()

    # Find width array
    pattern = rf'static const HPDF_CID_Width {array_name}\[\] = \{{(.*?)\{{0xFFFF, 0\}}\}};'
    match = re.search(pattern, content, re.DOTALL)

    if not match:
        print(f"ERROR: Array '{array_name}' not found")
        sys.exit(1)

    # Parse width entries
    widths = []
    for line in match.group(1).split('\n'):
        m = re.match(r'\s*\{(\d+),\s*(\d+)\}', line)
        if m:
            widths.append({
                'cid': int(m.group(1)),
                'width': int(m.group(2))
            })

    # Save as JSON
    with open(output_json, 'w') as f:
        json.dump(widths, f, indent=2)

    print(f"Extracted {len(widths)} widths from {array_name}")

# Usage
extract_width_array('hpdf_fontdef_cns.c', 'SIMSUN_W_ARRAY', 'SimSun.json')
extract_width_array('hpdf_fontdef_cns.c', 'SIMHEI_W_ARRAY', 'SimHei.json')
# ... repeat for all fonts
```

### Step 2: Extract Font Metrics

**Script**: `extract_metrics.py`

```python
def extract_font_metrics(c_file_path, init_function, font_name):
    with open(c_file_path, 'r') as f:
        content = f.read()

    # Find initialization function
    pattern = rf'static HPDF_STATUS\n{init_function}\s*\(HPDF_FontDef\s+fontdef\)\s*\{{(.*?)\}}'
    match = re.search(pattern, content, re.DOTALL)

    if not match:
        print(f"ERROR: Function '{init_function}' not found")
        return None

    func_body = match.group(1)

    # Extract metrics
    metrics = {}

    m = re.search(r'fontdef->ascent\s*=\s*(\d+)', func_body)
    if m: metrics['ascent'] = int(m.group(1))

    m = re.search(r'fontdef->descent\s*=\s*(-?\d+)', func_body)
    if m: metrics['descent'] = int(m.group(1))

    m = re.search(r'fontdef->cap_height\s*=\s*(\d+)', func_body)
    if m: metrics['cap_height'] = int(m.group(1))

    m = re.search(r'fontdef->font_bbox\s*=\s*HPDF_ToBox\(([^)]+)\)', func_body)
    if m:
        bbox_values = [int(x.strip()) for x in m.group(1).split(',')]
        metrics['font_bbox'] = {
            'left': bbox_values[0],
            'bottom': bbox_values[1],
            'right': bbox_values[2],
            'top': bbox_values[3]
        }

    m = re.search(r'fontdef->flags\s*=\s*([^;]+)', func_body)
    if m:
        flags_str = m.group(1).strip()
        # Parse flags (HPDF_FONT_SYMBOLIC + HPDF_FONT_FIXED_WIDTH, etc.)
        metrics['flags'] = flags_str

    m = re.search(r'fontdef->italic_angle\s*=\s*(-?\d+)', func_body)
    if m: metrics['italic_angle'] = int(m.group(1))

    m = re.search(r'fontdef->stemv\s*=\s*(\d+)', func_body)
    if m: metrics['stemv'] = int(m.group(1))

    return metrics

# Usage
metrics = extract_font_metrics('hpdf_fontdef_cns.c', 'SimSun_Init', 'SimSun')
print(json.dumps(metrics, indent=2))
```

### Step 3: Extract Encoders

**Challenge**: Encoders are MASSIVE (36K lines for CP936)

**Strategy**: Binary compression with run-length encoding

```python
import struct

def extract_encoder(c_file_path, array_name):
    # Extract unicode map array
    pattern = rf'static const HPDF_UnicodeMap_Rec {array_name}\[\] = \{{(.*?)\{{0xFFFF, 0xFFFF\}}\}};'
    # ... parse ...

    # Convert to binary format
    # Each entry: 4 bytes (2 for code, 2 for unicode)
    with open(f'{array_name}.bin', 'wb') as f:
        for entry in unicode_map:
            f.write(struct.pack('<HH', entry['code'], entry['unicode']))
```

**Compressed Format**:
```csharp
public class EncoderData
{
    public struct Range
    {
        public ushort StartCode;
        public ushort EndCode;
        public ushort BaseUnicode;
        public bool Sequential; // True if unicode = code + offset
    }

    public List<Range> Ranges { get; set; }
}
```

### Step 4: Generate C# Resource Manifests

**Tool**: `generate_resources.csproj` update

```xml
<ItemGroup>
  <EmbeddedResource Include="Font\CID\Data\SimSun.json" />
  <EmbeddedResource Include="Font\CID\Data\SimHei.json" />
  <EmbeddedResource Include="Font\CID\Data\MingLiU.json" />
  <!-- ... all fonts ... -->
  <EmbeddedResource Include="Font\CID\Data\CP936_Encoder.bin" />
  <EmbeddedResource Include="Font\CID\Data\CP950_Encoder.bin" />
  <EmbeddedResource Include="Font\CID\Data\CP932_Encoder.bin" />
  <EmbeddedResource Include="Font\CID\Data\CP949_Encoder.bin" />
</ItemGroup>
```

---

## API Design

### HpdfCIDFontType0 Class

```csharp
namespace Haru.Font.CID
{
    /// <summary>
    /// Represents a CIDFontType0 (predefined CJK font with built-in metrics).
    /// Does NOT embed font data - references system fonts by name.
    /// </summary>
    public class HpdfCIDFontType0 : IHpdfFontImplementation
    {
        private readonly HpdfDict _dict;               // Type 0 (composite) font
        private readonly HpdfDict _cidFontDict;        // CIDFontType0 descendant font
        private readonly HpdfDict _descriptor;         // Font descriptor
        private readonly string _baseFont;
        private readonly string _localName;
        private readonly PredefinedFontDefinition _fontDef;
        private readonly CIDEncoder _encoder;
        private readonly HpdfXref _xref;

        // IHpdfFontImplementation
        public HpdfDict Dict => _dict;
        public string BaseFont => _baseFont;
        public string LocalName => _localName;
        public int? CodePage => _encoder.CodePage;
        public int Ascent => _fontDef.Metrics.Ascent;
        public int Descent => _fontDef.Metrics.Descent;
        public int XHeight => _fontDef.Metrics.CapHeight; // Approximation
        public HpdfBox FontBBox => _fontDef.Metrics.FontBBox;

        public float GetCharWidth(byte charCode)
        {
            // Map byte code to CID via encoder
            ushort cid = _encoder.ToCID(charCode);

            // Look up width in predefined width array
            return _fontDef.GetCIDWidth(cid);
        }

        public float MeasureText(string text, float fontSize)
        {
            // Encode text to bytes using code page
            var encoding = Encoding.GetEncoding(_encoder.CodePage);
            byte[] bytes = encoding.GetBytes(text);

            float totalWidth = 0;
            foreach (byte b in bytes)
            {
                totalWidth += GetCharWidth(b);
            }

            // Scale from 1000-unit glyph space to user space
            return totalWidth * fontSize / 1000f;
        }

        public HpdfFont AsFont() => new HpdfFont(this);

        /// <summary>
        /// Creates a CIDFontType0 from a predefined font definition and encoder.
        /// </summary>
        public static HpdfCIDFontType0 Create(
            HpdfXref xref,
            string localName,
            string fontName,
            CIDEncoder encoder)
        {
            // Load predefined font definition
            var fontDef = PredefinedFontRegistry.GetDefinition(fontName);

            // Create font instance
            var font = new HpdfCIDFontType0(xref, localName, fontDef, encoder);

            // Build PDF objects
            font.CreateFontDescriptor();
            font.CreateCIDFontDictionary();
            font.CreateCMapStream();
            font.CreateType0FontDictionary();

            return font;
        }

        private void CreateFontDescriptor() { /* ... */ }
        private void CreateCIDFontDictionary() { /* ... */ }
        private void CreateCMapStream() { /* ... */ }
        private void CreateType0FontDictionary() { /* ... */ }
    }
}
```

### PredefinedFontDefinition Class

```csharp
namespace Haru.Font.CID
{
    public class PredefinedFontDefinition
    {
        public string FontName { get; set; }
        public FontMetrics Metrics { get; set; }
        public CIDWidth[] Widths { get; set; }

        public float GetCIDWidth(ushort cid)
        {
            // Binary search in sorted width array
            int left = 0, right = Widths.Length - 1;
            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (Widths[mid].CID == cid)
                    return Widths[mid].Width;
                else if (Widths[mid].CID < cid)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            // Return default width if not found
            return Metrics.DefaultWidth;
        }

        /// <summary>
        /// Loads a predefined font definition from embedded resources.
        /// </summary>
        public static PredefinedFontDefinition Load(string fontName)
        {
            var assembly = typeof(PredefinedFontDefinition).Assembly;
            var resourceName = $"Haru.Font.CID.Data.{fontName}.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<PredefinedFontDefinition>(json);
            }
        }
    }

    public class FontMetrics
    {
        public int Ascent { get; set; }
        public int Descent { get; set; }
        public int CapHeight { get; set; }
        public HpdfBox FontBBox { get; set; }
        public int Flags { get; set; }
        public int ItalicAngle { get; set; }
        public int StemV { get; set; }
        public int DefaultWidth { get; set; } = 1000;
    }

    public struct CIDWidth
    {
        public ushort CID { get; set; }
        public short Width { get; set; }
    }
}
```

### CIDEncoder Base Class

```csharp
namespace Haru.Font.CID
{
    public abstract class CIDEncoder
    {
        public string Name { get; protected set; }
        public string Registry { get; protected set; }
        public string Ordering { get; protected set; }
        public int Supplement { get; protected set; }
        public int CodePage { get; protected set; }

        /// <summary>
        /// Converts a byte code to a CID (Character ID).
        /// </summary>
        public abstract ushort ToCID(byte code);

        /// <summary>
        /// Converts a byte code to Unicode.
        /// </summary>
        public abstract ushort ToUnicode(byte code);

        /// <summary>
        /// Generates a CMap stream for this encoder.
        /// </summary>
        public abstract string GenerateCMap();

        /// <summary>
        /// Loads encoder data from embedded resources.
        /// </summary>
        protected Dictionary<ushort, ushort> LoadUnicodeMap(string resourceName)
        {
            var assembly = typeof(CIDEncoder).Assembly;
            var fullName = $"Haru.Font.CID.Data.{resourceName}";

            using (var stream = assembly.GetManifestResourceStream(fullName))
            {
                // Deserialize compressed binary format
                return DecompressUnicodeMap(stream);
            }
        }

        private Dictionary<ushort, ushort> DecompressUnicodeMap(Stream stream)
        {
            // Read compressed format (RLE or dictionary)
            // ...
        }
    }
}
```

### Concrete Encoder Classes

```csharp
namespace Haru.Font.CID
{
    /// <summary>
    /// GBK-EUC-H encoder for Simplified Chinese (CP936).
    /// </summary>
    public class GBKEucHEncoder : CIDEncoder
    {
        private readonly Dictionary<ushort, ushort> _unicodeMap;

        public GBKEucHEncoder()
        {
            Name = "GBK-EUC-H";
            Registry = "Adobe";
            Ordering = "GB1";
            Supplement = 2;
            CodePage = 936;

            // Load from embedded resource
            _unicodeMap = LoadUnicodeMap("CP936_Encoder.bin");
        }

        public override ushort ToCID(byte code)
        {
            // For GBK-EUC-H, CID = code (simple mapping)
            return code;
        }

        public override ushort ToUnicode(byte code)
        {
            if (_unicodeMap.TryGetValue(code, out ushort unicode))
                return unicode;
            return 0;
        }

        public override string GenerateCMap()
        {
            // Generate CMap PostScript code
            return CMapGenerator.Generate(this, _unicodeMap);
        }
    }

    /// <summary>
    /// ETen-B5-H encoder for Traditional Chinese (CP950).
    /// </summary>
    public class ETenB5HEncoder : CIDEncoder
    {
        // Similar implementation...
    }

    // EucHEncoder (CP932 - Japanese)
    // KscUhcHEncoder (CP949 - Korean)
}
```

### Document Extension Methods

```csharp
namespace Haru.Doc
{
    public static partial class HpdfDocumentExtensions
    {
        // Chinese Simplified
        public static void UseCNSFonts(this HpdfDocument doc)
        {
            PredefinedFontRegistry.RegisterFont(doc, "SimSun");
            PredefinedFontRegistry.RegisterFont(doc, "SimSun,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "SimSun,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "SimSun,BoldItalic");

            PredefinedFontRegistry.RegisterFont(doc, "SimHei");
            PredefinedFontRegistry.RegisterFont(doc, "SimHei,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "SimHei,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "SimHei,BoldItalic");
        }

        public static void UseCNSEncodings(this HpdfDocument doc)
        {
            EncoderRegistry.RegisterEncoder("GBK-EUC-H", new GBKEucHEncoder());
            EncoderRegistry.RegisterEncoder("GBKp-EUC-H", new GBKpEucHEncoder());
        }

        // Chinese Traditional
        public static void UseCNTFonts(this HpdfDocument doc)
        {
            PredefinedFontRegistry.RegisterFont(doc, "MingLiU");
            PredefinedFontRegistry.RegisterFont(doc, "MingLiU,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "MingLiU,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "MingLiU,BoldItalic");
        }

        public static void UseCNTEncodings(this HpdfDocument doc)
        {
            EncoderRegistry.RegisterEncoder("ETen-B5-H", new ETenB5HEncoder());
        }

        // Japanese
        public static void UseJPFonts(this HpdfDocument doc)
        {
            // MS-Gothic family (4 styles)
            PredefinedFontRegistry.RegisterFont(doc, "MS-Gothic");
            PredefinedFontRegistry.RegisterFont(doc, "MS-Gothic,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "MS-Gothic,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "MS-Gothic,BoldItalic");

            // MS-Mincho family (4 styles)
            PredefinedFontRegistry.RegisterFont(doc, "MS-Mincho");
            PredefinedFontRegistry.RegisterFont(doc, "MS-Mincho,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "MS-Mincho,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "MS-Mincho,BoldItalic");

            // MS-PGothic family (4 styles)
            PredefinedFontRegistry.RegisterFont(doc, "MS-PGothic");
            PredefinedFontRegistry.RegisterFont(doc, "MS-PGothic,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "MS-PGothic,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "MS-PGothic,BoldItalic");

            // MS-PMincho family (4 styles)
            PredefinedFontRegistry.RegisterFont(doc, "MS-PMincho");
            PredefinedFontRegistry.RegisterFont(doc, "MS-PMincho,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "MS-PMincho,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "MS-PMincho,BoldItalic");
        }

        public static void UseJPEncodings(this HpdfDocument doc)
        {
            EncoderRegistry.RegisterEncoder("EUC-H", new EucHEncoder());
            EncoderRegistry.RegisterEncoder("90ms-RKSJ-H", new RksjHEncoder());
        }

        // Korean
        public static void UseKRFonts(this HpdfDocument doc)
        {
            // Batang family (4 styles)
            PredefinedFontRegistry.RegisterFont(doc, "Batang");
            PredefinedFontRegistry.RegisterFont(doc, "Batang,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "Batang,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "Batang,BoldItalic");

            // BatangChe family (4 styles)
            PredefinedFontRegistry.RegisterFont(doc, "BatangChe");
            PredefinedFontRegistry.RegisterFont(doc, "BatangChe,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "BatangChe,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "BatangChe,BoldItalic");

            // Dotum family (4 styles)
            PredefinedFontRegistry.RegisterFont(doc, "Dotum");
            PredefinedFontRegistry.RegisterFont(doc, "Dotum,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "Dotum,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "Dotum,BoldItalic");

            // DotumChe family (4 styles)
            PredefinedFontRegistry.RegisterFont(doc, "DotumChe");
            PredefinedFontRegistry.RegisterFont(doc, "DotumChe,Bold");
            PredefinedFontRegistry.RegisterFont(doc, "DotumChe,Italic");
            PredefinedFontRegistry.RegisterFont(doc, "DotumChe,BoldItalic");
        }

        public static void UseKREncodings(this HpdfDocument doc)
        {
            EncoderRegistry.RegisterEncoder("KSCms-UHC-H", new KscUhcHEncoder());
        }
    }
}
```

---

## Testing Strategy

### Unit Tests

```csharp
namespace Haru.Test.Font.CID
{
    public class HpdfCIDFontType0Tests
    {
        [Theory]
        [InlineData("SimSun", 859, -140)]
        [InlineData("SimHei", 859, -140)]
        [InlineData("MingLiU", 880, -120)]
        public void PredefinedFont_Metrics_MatchCSource(string fontName, int expectedAscent, int expectedDescent)
        {
            var fontDef = PredefinedFontDefinition.Load(fontName);

            fontDef.Metrics.Ascent.Should().Be(expectedAscent);
            fontDef.Metrics.Descent.Should().Be(expectedDescent);
        }

        [Fact]
        public void SimSun_WidthLookup_ReturnsCorrectWidth()
        {
            var fontDef = PredefinedFontDefinition.Load("SimSun");

            // CID 668 should have width 500 (from C source)
            fontDef.GetCIDWidth(668).Should().Be(500);
        }

        [Fact]
        public void SimSun_Bold_HasModifiedMetrics()
        {
            var normal = PredefinedFontDefinition.Load("SimSun");
            var bold = PredefinedFontDefinition.Load("SimSun,Bold");

            // Bold should have double StemV
            bold.Metrics.StemV.Should().Be(normal.Metrics.StemV * 2);

            // Bold should have FORCE_BOLD flag
            (bold.Metrics.Flags & FontFlags.ForceBold).Should().NotBe(0);
        }

        [Fact]
        public void SimSun_Italic_HasModifiedAngle()
        {
            var normal = PredefinedFontDefinition.Load("SimSun");
            var italic = PredefinedFontDefinition.Load("SimSun,Italic");

            // Italic should have negative angle
            italic.Metrics.ItalicAngle.Should().Be(-11);

            // Italic should have ITALIC flag
            (italic.Metrics.Flags & FontFlags.Italic).Should().NotBe(0);
        }
    }

    public class CIDEncoderTests
    {
        [Theory]
        [InlineData(0x20, 0x0020)]  // Space
        [InlineData(0x41, 0x0041)]  // 'A'
        public void GBKEucHEncoder_ASCIIRange_MapsCorrectly(byte code, ushort expectedUnicode)
        {
            var encoder = new GBKEucHEncoder();

            encoder.ToUnicode(code).Should().Be(expectedUnicode);
        }

        [Fact]
        public void GBKEucHEncoder_GeneratesCMap()
        {
            var encoder = new GBKEucHEncoder();

            string cmap = encoder.GenerateCMap();

            cmap.Should().Contain("%!PS-Adobe-3.0 Resource-CMap");
            cmap.Should().Contain("/CMapName /GBK-EUC-H");
            cmap.Should().Contain("/Registry (Adobe)");
            cmap.Should().Contain("/Ordering (GB1)");
        }
    }
}
```

### Integration Tests

```csharp
namespace Haru.Test.Font.CID
{
    public class PredefinedFontsIntegrationTests
    {
        [Fact]
        public void ChineseSimplified_FullWorkflow()
        {
            var doc = new HpdfDocument();
            doc.UseCNSFonts();
            doc.UseCNSEncodings();

            var page = doc.AddPage();
            var font = doc.GetFont("SimSun", "GBK-EUC-H");

            page.BeginText();
            page.SetFontAndSize(font, 12);
            page.MoveTextPos(100, 700);
            page.ShowText("你好世界"); // Hello World
            page.EndText();

            using var ms = new MemoryStream();
            doc.Save(ms);

            ms.Position = 0;

            // Validate with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);

            // Check that text is present (may not extract correctly due to CID font)
            pdfPage.Text.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("SimSun", "GBK-EUC-H", 936)]
        [InlineData("MingLiU", "ETen-B5-H", 950)]
        public void PredefinedFont_CreatesValidPDF(string fontName, string encoding, int codePage)
        {
            var doc = new HpdfDocument();

            // Register fonts based on language
            if (codePage == 936) doc.UseCNSFonts();
            else if (codePage == 950) doc.UseCNTFonts();

            // Register encodings
            if (codePage == 936) doc.UseCNSEncodings();
            else if (codePage == 950) doc.UseCNTEncodings();

            var page = doc.AddPage();
            var font = doc.GetFont(fontName, encoding);

            font.Should().NotBeNull();
            font.BaseFont.Should().Be(fontName);

            page.BeginText();
            page.SetFontAndSize(font, 12);
            page.MoveTextPos(100, 700);
            page.ShowText("Test");
            page.EndText();

            using var ms = new MemoryStream();
            Action act = () => doc.Save(ms);
            act.Should().NotThrow();
        }
    }
}
```

---

## Timeline and Milestones

### Week 1 (Days 1-4)

| Day | Phase | Tasks | Hours | Status |
|-----|-------|-------|-------|--------|
| **Day 1** | Infrastructure | • Create base classes<br>• Data model classes<br>• Font definition loader | 4-6 | Pending |
| | Font Definitions (Start) | • Port CID base infrastructure<br>• Extract CNS fonts (SimSun, SimHei) | 4-6 | Pending |
| **Day 2** | Font Definitions (Continue) | • Extract CNT fonts (MingLiU)<br>• Extract JP fonts (4 families)<br>• Extract KR fonts (4 families) | 8-10 | Pending |
| **Day 3** | Encoders | • Design compression format<br>• Port CP936 encoder<br>• Port CP950 encoder | 6-8 | Pending |
| | Encoders (Continue) | • Port CP932 encoder<br>• Port CP949 encoder | 6-8 | Pending |
| **Day 4** | CMap Generation | • CMapGenerator for CIDFontType0<br>• CMap format implementation<br>• Range generation | 6-8 | Pending |
| | Font Registration | • PredefinedFontRegistry<br>• Use* extension methods<br>• GetFont() updates | 4-6 | Pending |
| | Testing | • Unit tests<br>• Integration tests<br>• Validation | 6-8 | Pending |

**Total Estimated Hours**: 44-60 hours (3-4 full days of work)

### Milestones

✅ **Milestone 1**: Infrastructure Complete (Day 1 EOD)
- All base classes implemented
- Data loading mechanism working
- Can load one font definition from JSON

✅ **Milestone 2**: All Fonts Defined (Day 2 EOD)
- All 36 font definitions extracted and loaded
- Width arrays validated against C source
- Metrics validated

✅ **Milestone 3**: Encoders Complete (Day 3 EOD)
- All 4 encoders (CP936, CP950, CP932, CP949) implemented
- Unicode mapping functional
- CID conversion working

✅ **Milestone 4**: PDF Generation Working (Day 4 Morning)
- Can create PDFs with predefined fonts
- CMap generation complete
- Font registration API working

✅ **Milestone 5**: Tests Passing (Day 4 Afternoon)
- All unit tests passing
- Integration tests with PdfPig passing
- Demo application working

---

## Risk Assessment

### High Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| **Encoder data size** | Assembly bloat (2MB+) | HIGH | Use compressed binary format with lazy loading |
| **Width array lookup performance** | Slow text measurement | MEDIUM | Use binary search on sorted arrays, consider caching |
| **CMap generation complexity** | Incorrect PDF output | MEDIUM | Port C code carefully, validate with PdfPig |
| **Encoding bugs** | Text displays incorrectly | HIGH | Extensive testing with real CJK text, compare with C library |

### Medium Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| **Missing width entries** | Default width used for some chars | MEDIUM | Validate extracted data completeness |
| **Metric extraction errors** | Font rendering issues | MEDIUM | Cross-reference with multiple sources |
| **Style variation bugs** | Bold/Italic incorrect | LOW | Test all 36 font combinations |

### Low Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| **Resource loading failures** | Runtime errors | LOW | Comprehensive error handling and fallbacks |
| **PDF version compatibility** | Some viewers fail | LOW | Target PDF 1.4+ (CID fonts standard) |

---

## Success Criteria

Implementation is successful when:

1. ✅ All 36 predefined fonts can be loaded
2. ✅ All 4 encoders (CP936, CP950, CP932, CP949) are functional
3. ✅ PDFs with predefined CJK fonts render correctly in Adobe Acrobat
4. ✅ PDFs with predefined CJK fonts render correctly in Chrome/Edge
5. ✅ Text measurement matches C library output (±1%)
6. ✅ All unit tests pass (target: 100+ tests)
7. ✅ All integration tests pass (PdfPig validation)
8. ✅ Demo application shows all 36 fonts working
9. ✅ Assembly size increase is acceptable (<5MB with compression)
10. ✅ Performance is acceptable (font loading <100ms, text rendering within 2x of C library)

---

## Next Steps

1. **Review and Approval**
   - Review this implementation plan
   - Approve architecture and approach
   - Prioritize vs other features

2. **Environment Setup**
   - Create `Tools/CIDFontDataExtractor` project
   - Set up Python scripts for data extraction
   - Prepare C source files for parsing

3. **Proof of Concept**
   - Implement Phase 1 (Infrastructure) for ONE font (SimSun)
   - Extract SimSun width array manually
   - Create minimal working example
   - Validate approach before full implementation

4. **Full Implementation**
   - Follow phases 1-6 as outlined
   - Track progress using TodoWrite
   - Update this document with actual hours spent

---

## Appendix A: Encoding Details

### CP936 (Chinese Simplified - GBK-EUC-H)

- **Registry**: Adobe
- **Ordering**: GB1
- **Supplement**: 2
- **Code Page**: 936
- **Character Range**: GB2312 + GBK extensions
- **Total Mappings**: ~21,000 characters
- **File Size**: 845KB (C source)

### CP950 (Chinese Traditional - ETen-B5-H)

- **Registry**: Adobe
- **Ordering**: CNS1
- **Supplement**: 0
- **Code Page**: 950
- **Character Range**: Big5
- **Total Mappings**: ~13,000 characters
- **File Size**: 332KB (C source)

### CP932 (Japanese - EUC-H / 90ms-RKSJ-H)

- **Registry**: Adobe
- **Ordering**: Japan1
- **Supplement**: 2
- **Code Page**: 932
- **Character Range**: Shift-JIS
- **Total Mappings**: ~11,000 characters
- **File Size**: 351KB (C source)

### CP949 (Korean - KSCms-UHC-H)

- **Registry**: Adobe
- **Ordering**: Korea1
- **Supplement**: 1
- **Code Page**: 949
- **Character Range**: EUC-KR + UHC extensions
- **Total Mappings**: ~17,000 characters
- **File Size**: 642KB (C source)

---

## Appendix B: References

1. **PDF Reference 1.7** - Adobe Systems
   - Section 5.6: CID-Keyed Fonts
   - Section 5.6.3: CIDFont Dictionaries

2. **Adobe CMap and CIDFont Files Specification**
   - CMap file format
   - Character collection standards

3. **Libharu C Source Code**
   - `c-src/hpdf_fontdef_cid.c`
   - `c-src/hpdf_fontdef_cns.c`
   - `c-src/hpdf_fontdef_cnt.c`
   - `c-src/hpdf_fontdef_jp.c`
   - `c-src/hpdf_fontdef_kr.c`
   - `c-src/hpdf_encoder_cns.c`
   - `c-src/hpdf_encoder_cnt.c`
   - `c-src/hpdf_encoder_jp.c`
   - `c-src/hpdf_encoder_kr.c`
   - `c-src/hpdf_font_cid.c`

---

**End of Implementation Plan**
