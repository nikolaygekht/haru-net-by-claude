# CID Font and CJK Support Implementation Plan

## Executive Summary

This document outlines the implementation plan for CID (Character Identifier) fonts and CJK (Chinese, Japanese, Korean) language support in the Haru PDF library .NET port.

**Estimated Effort**: 5-7 days
**Priority**: Medium-High (Required for complete international support)
**Complexity**: High

---

## Background

### Why CID Fonts Are Needed

CJK languages have thousands of characters that cannot fit in 256-byte single-byte encodings:
- Japanese: ~7,000+ characters (Hiragana, Katakana, Kanji)
- Simplified Chinese: ~6,000+ characters (GB2312)
- Traditional Chinese: ~13,000+ characters (Big5)
- Korean: ~11,000+ characters (Hangul)

**Multi-byte character sets required**:
- CP932 (Shift-JIS) for Japanese - 2 bytes per character
- CP936 (GBK) for Simplified Chinese - 2 bytes per character
- CP950 (Big5) for Traditional Chinese - 2 bytes per character
- CP949 (EUC-KR) for Korean - 2 bytes per character

**Current Limitation**: TrueType and Type 1 font implementations only support single-byte encodings (CP1251-1258).

---

## ⚠️ CRITICAL: Adobe Acrobat Compatibility

**This is the second implementation attempt.** The previous attempt had serious compatibility issues:

### Previous Implementation Issues

| PDF Reader | Status | Issue |
|------------|--------|-------|
| **Adobe Acrobat** | ❌ **FAILED** | "Cannot find or create font" errors |
| Edge/Chrome | ✓ Worked | No issues |
| PDF Viewer Plus | ❌ Failed | Broken after object ordering changes |

### Root Causes Identified

1. **Font Naming Conflicts**
   - Adobe validates PostScript font names against system registry
   - Using system font names causes conflicts
   - **Solution**: Use simple, non-conflicting names (e.g., "CIDFont-JP" instead of "NotoSansJP-Regular")

2. **PDF Object Reference Ordering** ⚠️ **CRITICAL**
   - PDF objects must be FULLY POPULATED before being added to xref
   - Objects must be added to xref BEFORE being referenced
   - **Wrong approach**:
     ```
     Add partial object to xref → Reference it → Complete it later
     ```
   - **Correct approach**:
     ```
     Build complete object → Add to xref → Reference it
     ```

3. **Incomplete Dictionary Population**
   - Descendant font was added to DescendantFonts array before FontDescriptor was populated
   - Some PDF readers tolerate this, Adobe does not
   - **Solution**: Complete all child objects before linking them to parent

### Correct Build Order (MUST FOLLOW)

```
1. Create FontFile2 stream (fully complete) → Add to xref
2. Create FontDescriptor (with FontFile2 link) → Add to xref
3. Create CIDFontType2 (with ALL fields + FontDescriptor) → Add to xref
4. Create ToUnicode CMap → Add to xref
5. Create Type 0 font (with DescendantFonts + ToUnicode) → Add to xref
```

### Implementation Requirements

✅ **MUST**: Test in Adobe Acrobat Reader at each phase
✅ **MUST**: Use simple font naming (avoid system font names)
✅ **MUST**: Follow correct object build order from the start
✅ **MUST**: Complete objects before referencing them
✅ **MUST**: Verify no "Cannot find or create font" errors in Adobe

**Adobe compatibility is THE critical success factor. Chrome/Edge working is not sufficient.**

---

## PDF CID Font Architecture

### Type 0 (Composite) Font Structure

```
Type 0 Font Dictionary (Top Level)
├── /Type: /Font
├── /Subtype: /Type0
├── /BaseFont: /FontName
├── /Encoding: /Identity-H (or CMap stream)
├── /ToUnicode: <CMap stream> (for text extraction)
└── /DescendantFonts: [CIDFont]
    │
    └── CIDFont Dictionary (Descendant)
        ├── /Type: /Font
        ├── /Subtype: /CIDFontType2 (TrueType-based)
        ├── /BaseFont: /FontName
        ├── /CIDSystemInfo:
        │   ├── /Registry: (Adobe)
        │   ├── /Ordering: (Identity, Japan1, GB1, CNS1, Korea1)
        │   └── /Supplement: 0
        ├── /DW: <default width>
        ├── /W: [<CID width array>]
        ├── /DW2: [<vPosition> <vWidth>] (vertical writing)
        ├── /CIDToGIDMap: <stream or /Identity>
        └── /FontDescriptor:
            ├── /Type: /FontDescriptor
            ├── /FontName: /FontName
            ├── /Flags: <font flags>
            ├── /FontBBox: [...]
            ├── /Ascent: <ascent>
            ├── /Descent: <descent>
            ├── /CapHeight: <cap height>
            ├── /StemV: <stem v>
            └── /FontFile2: <TrueType font stream>
```

### CMap (Character Map) Structure

CMap maps character codes (from text string) to CIDs (Character IDs):

```
CMap Stream (PostScript format):
/CIDInit /ProcSet findresource begin
12 dict begin
begincmap
/CIDSystemInfo 3 dict dup begin
  /Registry (Adobe) def
  /Ordering (Identity) def
  /Supplement 0 def
end def
/CMapName /Identity-H def
/CMapType 1 def
/WMode 0 def

1 begincodespacerange
<0000> <FFFF>
endcodespacerange

100 begincidrange
<0020> <007E> 1
<3041> <3096> 12353
<30A1> <30FA> 12449
...
endcidrange

endcmap
CMapName currentdict /CMap defineresource pop
end
end
```

---

## .NET Implementation Strategy

### Modernization Approach

Instead of porting massive character data arrays from C, we'll leverage .NET capabilities:

1. **Use .NET Encoding classes** for multi-byte conversion
   - `Encoding.GetEncoding(932)` for Shift-JIS
   - `Encoding.GetEncoding(936)` for GBK
   - `Encoding.GetEncoding(949)` for EUC-KR
   - `Encoding.GetEncoding(950)` for Big5

2. **Load TrueType fonts directly** with CJK glyphs
   - User provides TrueType font with CJK support
   - Parse font to extract glyph IDs and widths
   - No need for hardcoded character arrays

3. **Use Identity-H encoding** with ToUnicode CMap
   - Simplest approach recommended by PDF spec
   - Character code = Unicode code point
   - CID = Glyph ID from TrueType font
   - ToUnicode CMap enables text extraction

### Architecture Components

```
Haru.Font.CID
├── HpdfCIDFont.cs           - Main CID font class
├── CIDFontType.cs           - Enum: Type0, Type2
├── CIDSystemInfo.cs         - Registry/Ordering/Supplement
├── CMapData.cs              - CMap data structures
├── CMapGenerator.cs         - Generate CMap streams
└── MultiByteEncoder.cs      - Multi-byte text encoding

Haru.Font.CID.Predefined
├── JapaneseFont.cs          - Japanese font support
├── ChineseSimplifiedFont.cs - Simplified Chinese support
├── ChineseTraditionalFont.cs- Traditional Chinese support
└── KoreanFont.cs            - Korean font support
```

---

## Implementation Phases

### Phase 1: Core CID Font Infrastructure (2 days)

#### 1.1 Data Structures

**Create: `cs-src/Haru/Font/CID/CIDSystemInfo.cs`**
```csharp
public class CIDSystemInfo
{
    public string Registry { get; set; }  // "Adobe"
    public string Ordering { get; set; }  // "Identity", "Japan1", "GB1", etc.
    public int Supplement { get; set; }   // 0

    public HpdfDict ToDict();
}
```

**Create: `cs-src/Haru/Font/CID/CIDFontType.cs`**
```csharp
public enum CIDFontType
{
    Type0 = 0,  // CID-keyed PostScript font
    Type2 = 2   // CID-keyed TrueType font
}

public enum CIDWritingMode
{
    Horizontal = 0,
    Vertical = 1
}
```

**Create: `cs-src/Haru/Font/CID/CIDWidthData.cs`**
```csharp
public class CIDWidthRange
{
    public ushort StartCID { get; set; }
    public ushort[] Widths { get; set; }
}

public class CIDWidthData
{
    public int DefaultWidth { get; set; }
    public List<CIDWidthRange> Ranges { get; set; }

    public HpdfArray ToWidthArray();
}
```

#### 1.2 Multi-Byte Text Encoder

**Create: `cs-src/Haru/Font/CID/MultiByteEncoder.cs`**
```csharp
public class MultiByteEncoder
{
    private readonly Encoding _encoding;
    private readonly int _codePage;

    public MultiByteEncoder(int codePage)
    {
        _codePage = codePage;
        _encoding = Encoding.GetEncoding(codePage);
    }

    // Convert .NET string to multi-byte byte array
    public byte[] EncodeString(string text);

    // Parse multi-byte text into character codes
    public IEnumerable<ushort> ParseText(string text);

    // Check if byte is lead byte (for DBCS)
    public bool IsLeadByte(byte b);
}
```

#### 1.3 CMap Generator

**Create: `cs-src/Haru/Font/CID/CMapGenerator.cs`**
```csharp
public class CMapGenerator
{
    // Generate Identity-H CMap (Unicode → CID = GID)
    public static HpdfStreamObject CreateIdentityHCMap(
        HpdfXref xref,
        CIDSystemInfo systemInfo,
        Dictionary<ushort, ushort> unicodeToGID);

    // Generate ToUnicode CMap (for text extraction)
    public static HpdfStreamObject CreateToUnicodeCMap(
        HpdfXref xref,
        Dictionary<ushort, ushort> cidToUnicode);

    // Helper: Write CMap PostScript format
    private static void WriteCMapHeader(
        HpdfStream stream,
        CIDSystemInfo systemInfo);

    private static void WriteCodeSpaceRange(
        HpdfStream stream,
        ushort from, ushort to);

    private static void WriteCIDRange(
        HpdfStream stream,
        Dictionary<ushort, ushort> mapping);
}
```

### Phase 2: CID Font Type 2 (TrueType-based) (2 days)

#### 2.1 Main CID Font Class

**Create: `cs-src/Haru/Font/HpdfCIDFont.cs`**
```csharp
public class HpdfCIDFont
{
    private readonly HpdfDict _fontDict;
    private readonly HpdfDict _descendantFont;
    private readonly HpdfTrueTypeFont _ttFont;
    private readonly CIDSystemInfo _systemInfo;
    private readonly MultiByteEncoder _encoder;
    private readonly CIDWritingMode _writingMode;

    // Load TrueType font as CID font
    public static HpdfCIDFont LoadFromTrueTypeFile(
        HpdfXref xref,
        string localName,
        string ttfPath,
        int codePage,
        CIDWritingMode writingMode = CIDWritingMode.Horizontal);

    // Create Type 0 font dictionary
    private void CreateType0Dictionary();

    // Create CIDFontType2 descendant font
    private void CreateCIDFontType2();

    // Generate CIDToGIDMap (CID → Glyph ID)
    private HpdfStreamObject CreateCIDToGIDMap();

    // Extract character widths from TrueType font
    private CIDWidthData ExtractWidthData();

    // Get font dictionary for integration
    public HpdfDict GetFontDict();

    // Wrap in HpdfFont for page API
    public HpdfFont AsFont();

    // Text width calculation
    public int MeasureText(string text);
}
```

#### 2.2 Integration with HpdfFont

**Update: `cs-src/Haru/Font/HpdfFont.cs`**
```csharp
public class HpdfFont
{
    private readonly HpdfTrueTypeFont? _ttFont;
    private readonly HpdfType1Font? _type1Font;
    private readonly HpdfCIDFont? _cidFont;  // NEW

    // Constructor for CID fonts
    internal HpdfFont(HpdfCIDFont cidFont)
    {
        if (cidFont == null)
            throw new HpdfException(
                HpdfErrorCode.InvalidParameter,
                "CID font cannot be null");
        _cidFont = cidFont;
        _dict = cidFont.GetFontDict();
        // ...
    }

    // Update encoding property
    public int? EncodingCodePage =>
        _ttFont?.CodePage ??
        _type1Font?.CodePage ??
        _cidFont?.CodePage;
}
```

### Phase 3: Predefined CJK Font Support (1-2 days)

#### 3.1 Japanese Font Support

**Create: `cs-src/Haru/Font/CID/Predefined/JapaneseFont.cs`**
```csharp
public static class JapaneseFont
{
    // Load Japanese font with Shift-JIS (CP932)
    public static HpdfCIDFont LoadShiftJIS(
        HpdfXref xref,
        string localName,
        string ttfPath)
    {
        return HpdfCIDFont.LoadFromTrueTypeFile(
            xref,
            localName,
            ttfPath,
            932,  // CP932 (Shift-JIS)
            CIDWritingMode.Horizontal);
    }

    // Predefined Japanese fonts (optional)
    public static readonly CIDSystemInfo Japan1 = new CIDSystemInfo
    {
        Registry = "Adobe",
        Ordering = "Japan1",
        Supplement = 6
    };
}
```

#### 3.2 Chinese Font Support

**Create: `cs-src/Haru/Font/CID/Predefined/ChineseSimplifiedFont.cs`**
```csharp
public static class ChineseSimplifiedFont
{
    // Load Simplified Chinese font with GBK (CP936)
    public static HpdfCIDFont LoadGBK(
        HpdfXref xref,
        string localName,
        string ttfPath)
    {
        return HpdfCIDFont.LoadFromTrueTypeFile(
            xref,
            localName,
            ttfPath,
            936,  // CP936 (GBK)
            CIDWritingMode.Horizontal);
    }

    public static readonly CIDSystemInfo GB1 = new CIDSystemInfo
    {
        Registry = "Adobe",
        Ordering = "GB1",
        Supplement = 5
    };
}
```

**Create: `cs-src/Haru/Font/CID/Predefined/ChineseTraditionalFont.cs`**
```csharp
public static class ChineseTraditionalFont
{
    // Load Traditional Chinese font with Big5 (CP950)
    public static HpdfCIDFont LoadBig5(
        HpdfXref xref,
        string localName,
        string ttfPath)
    {
        return HpdfCIDFont.LoadFromTrueTypeFile(
            xref,
            localName,
            ttfPath,
            950,  // CP950 (Big5)
            CIDWritingMode.Horizontal);
    }

    public static readonly CIDSystemInfo CNS1 = new CIDSystemInfo
    {
        Registry = "Adobe",
        Ordering = "CNS1",
        Supplement = 7
    };
}
```

#### 3.3 Korean Font Support

**Create: `cs-src/Haru/Font/CID/Predefined/KoreanFont.cs`**
```csharp
public static class KoreanFont
{
    // Load Korean font with EUC-KR (CP949)
    public static HpdfCIDFont LoadEUCKR(
        HpdfXref xref,
        string localName,
        string ttfPath)
    {
        return HpdfCIDFont.LoadFromTrueTypeFile(
            xref,
            localName,
            ttfPath,
            949,  // CP949 (EUC-KR)
            CIDWritingMode.Horizontal);
    }

    public static readonly CIDSystemInfo Korea1 = new CIDSystemInfo
    {
        Registry = "Adobe",
        Ordering = "Korea1",
        Supplement = 2
    };
}
```

### Phase 4: Testing & Demos (1 day)

#### 4.1 Unit Tests

**Create: `cs-src/Haru.Tests/Font/CID/CIDFontTests.cs`**
```csharp
public class CIDFontTests
{
    [Fact]
    public void LoadJapaneseFont_ShouldSucceed()
    {
        // Test loading Japanese TrueType font
    }

    [Fact]
    public void EncodeShiftJISText_ShouldProduceCorrectBytes()
    {
        // Test Shift-JIS encoding
    }

    [Fact]
    public void CreateCIDToGIDMap_ShouldMapCorrectly()
    {
        // Test CID to GID mapping
    }

    [Fact]
    public void GenerateToUnicodeCMap_ShouldBeValid()
    {
        // Test ToUnicode CMap generation
    }

    [Theory]
    [InlineData(932)]  // Japanese
    [InlineData(936)]  // Simplified Chinese
    [InlineData(949)]  // Korean
    [InlineData(950)]  // Traditional Chinese
    public void LoadCJKFont_WithCodePage_ShouldSucceed(int codePage)
    {
        // Test different code pages
    }
}
```

**Create: `cs-src/Haru.Tests/Font/CID/MultiByteEncoderTests.cs`**
```csharp
public class MultiByteEncoderTests
{
    [Fact]
    public void EncodeJapaneseText_ShouldProduceShiftJIS()
    {
        var encoder = new MultiByteEncoder(932);
        var bytes = encoder.EncodeString("こんにちは");
        // Verify Shift-JIS bytes
    }

    [Fact]
    public void ParseMultiByteText_ShouldReturnCorrectCodes()
    {
        var encoder = new MultiByteEncoder(932);
        var codes = encoder.ParseText("こんにちは").ToList();
        // Verify multi-byte character codes
    }
}
```

#### 4.2 Demo Application

**Create: `cs-src/Haru.Demos/CJKFontDemo.cs`**
```csharp
public static class CJKFontDemo
{
    public static void Run()
    {
        var pdf = new HpdfDocument();
        var page = pdf.AddPage();

        // Japanese
        var japaneseFont = JapaneseFont.LoadShiftJIS(
            pdf.Xref,
            "JapaneseFont",
            "fonts/NotoSansJP-Regular.ttf");

        page.BeginText();
        page.SetFontAndSize(japaneseFont.AsFont(), 16);
        page.MoveTextPos(50, 750);
        page.ShowText("こんにちは世界");  // Hello World

        // Simplified Chinese
        var chineseFont = ChineseSimplifiedFont.LoadGBK(
            pdf.Xref,
            "ChineseFont",
            "fonts/NotoSansSC-Regular.ttf");

        page.SetFontAndSize(chineseFont.AsFont(), 16);
        page.MoveTextPos(0, -30);
        page.ShowText("你好世界");  // Hello World

        // Korean
        var koreanFont = KoreanFont.LoadEUCKR(
            pdf.Xref,
            "KoreanFont",
            "fonts/NotoSansKR-Regular.ttf");

        page.SetFontAndSize(koreanFont.AsFont(), 16);
        page.MoveTextPos(0, -30);
        page.ShowText("안녕하세요");  // Hello

        page.EndText();

        pdf.SaveToFile("CJKFontDemo.pdf");
    }
}
```

**Update: `cs-src/Haru.Demos/InternationalDemo.cs`**
```csharp
// Remove "Note: CJK requires CID fonts" warning
// Add Japanese, Chinese, Korean text with CID fonts
```

### Phase 5: Vertical Writing Support (Optional, 0.5 day)

#### 5.1 Vertical Writing Mode

**Extend: `HpdfCIDFont.cs`**
```csharp
// Support vertical writing (top-to-bottom, right-to-left)
public static HpdfCIDFont LoadFromTrueTypeFile(
    HpdfXref xref,
    string localName,
    string ttfPath,
    int codePage,
    CIDWritingMode writingMode = CIDWritingMode.Vertical)
{
    // Use Identity-V encoding for vertical
    // Add DW2 array for vertical metrics
}
```

---

## Technical Considerations

### 1. Character Code vs CID vs GID

- **Character Code**: Byte value from text string (e.g., Shift-JIS bytes)
- **CID (Character ID)**: Abstract character identifier in CID font
- **GID (Glyph ID)**: Actual glyph index in TrueType font

**Mapping flow**:
```
Text String → Character Codes → CIDs → GIDs → Glyphs
   (CP932)        (2-byte)      (CMap)  (CIDToGIDMap)  (TrueType)
```

**Simplified with Identity-H**:
```
Text String → Unicode → CID=GID → Glyphs
              (UTF-16)    (Identity)  (TrueType)
```

### 2. Identity-H vs Predefined CMaps

**Identity-H Approach** (Recommended):
- Character code = Unicode code point (UTF-16BE)
- CID = Glyph ID in TrueType font
- Simple and modern
- Requires ToUnicode CMap for text extraction

**Predefined CMap Approach**:
- Use predefined CMaps (e.g., 90ms-RKSJ-H for Shift-JIS)
- Character code = Native encoding (e.g., Shift-JIS bytes)
- More complex but traditional
- No ToUnicode CMap needed

**Decision**: Use Identity-H for simplicity and compatibility with existing TrueType font implementation.

### 3. Font Embedding

For CID fonts, we embed the full TrueType font in FontFile2 stream (same as regular TrueType fonts). No special subsetting needed initially.

### 4. Text Rendering

**Single-byte fonts** (current):
```csharp
byte[] bytes = Encoding.GetEncoding(1252).GetBytes("Hello");
// bytes = [0x48, 0x65, 0x6C, 0x6C, 0x6F]
stream.Write(bytes);
```

**Multi-byte CID fonts**:
```csharp
// Convert to UTF-16BE for Identity-H
string text = "こんにちは";
byte[] bytes = Encoding.BigEndianUnicode.GetBytes(text);
// bytes = [0x30, 0x53, 0x30, 0x93, ...]  (2 bytes per char)
stream.Write(bytes);
```

### 5. Width Calculation

For CID fonts, character widths are stored in the W array:
```
/W [start_cid [widths...]]
/W [1 [500] 100 [600 700 800]]
    CID 1 has width 500
    CIDs 100-102 have widths 600, 700, 800
```

Extract from TrueType font:
```csharp
var widthData = new CIDWidthData { DefaultWidth = 1000 };
foreach (var unicode in usedCharacters)
{
    var gid = ttFont.GetGlyphID(unicode);
    var width = ttFont.GetGlyphWidth(gid);
    widthData.AddWidth(unicode, width);
}
```

---

## File Structure

```
cs-src/Haru/Font/CID/
├── CIDFontType.cs            # Enums and constants
├── CIDSystemInfo.cs          # System info data class
├── CIDWidthData.cs           # Width array data class
├── MultiByteEncoder.cs       # Multi-byte text encoding
├── CMapGenerator.cs          # CMap stream generation
├── HpdfCIDFont.cs            # Main CID font class
└── Predefined/
    ├── JapaneseFont.cs       # Japanese helper
    ├── ChineseSimplifiedFont.cs
    ├── ChineseTraditionalFont.cs
    └── KoreanFont.cs

cs-src/Haru.Tests/Font/CID/
├── CIDFontTests.cs
├── MultiByteEncoderTests.cs
├── CMapGeneratorTests.cs
└── Integration/
    └── CJKRenderingTests.cs

cs-src/Haru.Demos/
├── CJKFontDemo.cs            # CJK demo
└── InternationalDemo.cs      # Updated with CJK support
```

---

## Testing Strategy

### Unit Tests
- [ ] Multi-byte encoding (Shift-JIS, GBK, EUC-KR, Big5)
- [ ] CMap generation (Identity-H, ToUnicode)
- [ ] CID width extraction
- [ ] CIDToGIDMap generation
- [ ] CIDSystemInfo serialization

### Integration Tests
- [ ] Load Japanese TrueType font
- [ ] Load Chinese TrueType font
- [ ] Load Korean TrueType font
- [ ] Render Japanese text
- [ ] Render Chinese text
- [ ] Render Korean text
- [ ] Verify PDF structure
- [ ] Verify text extraction

### Manual Testing (⚠️ Adobe Acrobat is CRITICAL)

**Testing Priority Order:**
1. **Adobe Acrobat Reader** ← PRIMARY TEST TARGET
2. PDF Viewer Plus (Windows)
3. Edge/Chrome (PDF.js)

**Test Checklist:**
- [ ] ✅ **CRITICAL**: Open generated PDFs in Adobe Acrobat Reader
  - [ ] Verify NO "Cannot find or create font" errors
  - [ ] Verify NO font substitution warnings
  - [ ] Verify CJK text displays correctly (not dots or boxes)
- [ ] Open generated PDFs in PDF Viewer Plus
  - [ ] Verify no crashes or errors
  - [ ] Verify text displays correctly
- [ ] Open generated PDFs in Chrome/Edge
  - [ ] Verify text displays correctly
- [ ] Copy/paste text from PDF (all readers)
  - [ ] Verify CJK characters copy correctly
  - [ ] Verify ToUnicode CMap works
- [ ] Search text in PDF (all readers)
  - [ ] Verify can find CJK text
- [ ] Visual rendering verification
  - [ ] Compare output across readers
  - [ ] Verify no glyph missing (tofu/boxes)

**Stop Criteria:** If Adobe Acrobat shows ANY font errors, STOP and fix before proceeding.

---

## Dependencies

### NuGet Packages
No additional packages needed! .NET has built-in support:
- `System.Text.Encoding` - Multi-byte encodings
- `System.Text.Encoding.CodePages` - Legacy code pages (CP932, CP936, etc.)

### Test Fonts
Download free CJK fonts for testing:
- **Japanese**: Noto Sans JP (Google Fonts)
- **Simplified Chinese**: Noto Sans SC (Google Fonts)
- **Traditional Chinese**: Noto Sans TC (Google Fonts)
- **Korean**: Noto Sans KR (Google Fonts)

All available at: https://fonts.google.com/noto

---

## Risks and Mitigations

### Risk 1: Large Font Files
**Issue**: CJK fonts are typically 5-10 MB
**Mitigation**: Implement font subsetting (future enhancement)

### Risk 2: Code Page Availability
**Issue**: Some code pages may not be available on all platforms
**Mitigation**: Use `System.Text.Encoding.CodePages` NuGet package if needed

### Risk 3: Complex Glyph Positioning
**Issue**: Some CJK characters require ligatures or complex positioning
**Mitigation**: Start with simple horizontal rendering, add advanced features later

### Risk 4: Vertical Writing Mode
**Issue**: Vertical writing is complex
**Mitigation**: Implement horizontal mode first, vertical mode as Phase 5

---

## Success Criteria

- [ ] Can load TrueType fonts with CJK characters
- [ ] Can render Japanese text (Hiragana, Katakana, Kanji)
- [ ] Can render Simplified Chinese text
- [ ] Can render Traditional Chinese text
- [ ] Can render Korean text (Hangul)
- [ ] Generated PDFs display correctly in all major PDF readers
- [ ] Text can be copied and searched in PDF readers
- [ ] All unit tests pass (target: 20+ tests)
- [ ] Demo application works for all CJK languages
- [ ] Documentation is complete

---

## Future Enhancements (Post-MVP)

1. **Font Subsetting**
   - Reduce file size by embedding only used glyphs
   - Significant for large CJK fonts

2. **Predefined CID Fonts**
   - Use Adobe's predefined CJK fonts
   - No embedding needed (smaller file size)

3. **Vertical Writing Mode**
   - Right-to-left, top-to-bottom layout
   - Required for traditional Japanese/Chinese documents

4. **Ruby Text (Furigana)**
   - Small phonetic annotations above Kanji
   - Common in Japanese documents

5. **CID-keyed Type 1 Fonts**
   - Support CIDFontType0 (PostScript-based)
   - Less common, lower priority

---

## Timeline

| Phase | Duration | Deliverable |
|-------|----------|-------------|
| Phase 1: Core Infrastructure | 2 days | Data structures, encoder, CMap generator |
| Phase 2: CIDFont Type 2 | 2 days | Main CID font class, TrueType integration |
| Phase 3: Predefined Support | 1-2 days | Japanese, Chinese, Korean helpers |
| Phase 4: Testing & Demos | 1 day | Tests, demo application |
| **Total** | **6-7 days** | **Complete CJK support** |

Optional:
| Phase 5: Vertical Writing | 0.5 day | Vertical mode support |

---

## References

- **PDF Reference 1.7**: Section 9.7 (Composite Fonts)
- **Adobe CMap and CIDFont Files Specification**
- **TrueType Font Specification**: Apple and Microsoft documentation
- **Unicode Standard**: Character code mappings
- **.NET Encoding Class**: Multi-byte encoding support

---

## Conclusion

This implementation plan provides a modern, .NET-centric approach to CID fonts and CJK support. By leveraging .NET's built-in encoding support and the existing TrueType font infrastructure, we can implement full CJK support in approximately one week of development effort.

The architecture is designed to be:
- **Simple**: Use Identity-H encoding for straightforward implementation
- **Extensible**: Easy to add new languages and writing modes
- **Testable**: Comprehensive unit and integration tests
- **Maintainable**: Clean separation of concerns

Once implemented, the Haru PDF library will support complete international text rendering for all major world languages.
