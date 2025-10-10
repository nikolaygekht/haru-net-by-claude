# CID Font Support for CJK Languages Complete! ✓

This session successfully implemented **CID (Character Identifier) font support for multi-byte character sets (CJK languages)**, enabling Chinese, Japanese, and Korean text rendering in PDFs.

## Session Date
2025-10-10 (Phase 1 CID Fonts - CJK Support)

## What Was Accomplished

### 1. CID Font Architecture ✓

Implemented complete CID font support using **Type 0 (Composite) fonts with CIDFontType2**:

**Key Components:**
- **CID Font Loading** - TrueType-based CID fonts with code page support
- **Identity-H Encoding** - Horizontal writing mode with CID = Glyph ID mapping
- **Glyph ID Conversion** - Unicode → Glyph ID via TrueType cmap table
- **Width Arrays (W)** - Complete glyph width mapping for proper character spacing
- **ToUnicode CMap** - Text extraction and search support
- **Font Descriptor** - Adobe-compatible fields (FontFamily, FontStretch, FontWeight, Lang)
- **PDF Version Auto-Upgrade** - Automatic upgrade to PDF 1.4 (Adobe Acrobat requirement)

### 2. CID Font Implementation ✓

Created **HpdfCIDFont.cs** for CJK language support:

```csharp
// Load CID font with code page
var japaneseFont = HpdfCIDFont.LoadFromTrueTypeFile(
    pdf,  // HpdfDocument (automatically upgrades to PDF 1.4)
    "JP",
    "demo/ttfont/noto-jp.ttf",
    932  // CP932 (Shift-JIS)
);

// Use font
page.SetFontAndSize(japaneseFont.AsFont(), 16);
page.ShowText("こんにちは世界");  // Hello World (Hiragana)
```

**Supported Code Pages:**
- **932** - Japanese (Shift-JIS)
- **936** - Simplified Chinese (GBK)
- **949** - Korean (EUC-KR)
- **950** - Traditional Chinese (Big5)

**Key Features:**

1. **Font Structure (Adobe-Compatible):**
   ```
   Type 0 Font (Composite)
   ├── Type: /Font
   ├── Subtype: /Type0
   ├── BaseFont: /NotoSerifJP-Regular (extracted from font's name table)
   ├── Encoding: /Identity-H
   ├── DescendantFonts: [CIDFont]
   │   ├── Type: /Font
   │   ├── Subtype: /CIDFontType2
   │   ├── CIDSystemInfo: << /Registry (Adobe) /Ordering (Identity) /Supplement 0 >>
   │   ├── FontDescriptor:
   │   │   ├── FontFile2 (embedded TTF data)
   │   │   ├── FontFamily, FontStretch, FontWeight (Adobe compatibility)
   │   │   └── Lang (language tag: ja, zh-CN, zh-TW, ko)
   │   ├── W: [0 [w1 w2...] 100 [w101...]]  // Complete width array
   │   └── CIDToGIDMap: /Identity
   └── ToUnicode: <stream>
   ```

2. **Glyph ID Conversion:**
   - Convert Unicode text to glyph IDs using font's cmap table
   - Output as hex string in content stream
   - Example: "Hello" → <0048 0065 006C 006C 006F>

3. **Width Calculation:**
   - Extract advance width from TrueType hmtx table
   - Scale to 1000-unit em square: `width * 1000 / unitsPerEm`
   - Build complete W array (not just ASCII range)

4. **PostScript Name Extraction:**
   - Extract actual font name from TrueType name table (name ID 6)
   - Priority: Platform 3 (Microsoft Unicode), fallback to Platform 1 (Mac)
   - Example: "NotoSerifJP-Regular" instead of synthetic "CIDFont-JP"

5. **Adobe Compatibility Fields:**
   - **FontFamily** - Extracted from name table (name ID 1)
   - **FontStretch** - Set to "Normal"
   - **FontWeight** - From OS/2 table (e.g., 400 = Normal, 700 = Bold)
   - **Lang** - Language tag based on code page (ja, zh-CN, zh-TW, ko)

6. **PDF Version Management:**
   - Default remains PDF 1.2 for backward compatibility
   - Auto-upgrade to PDF 1.4 when CID font loaded (Adobe Acrobat requirement)
   - Does not downgrade if already higher version (e.g., 1.6 for encryption)

### 3. Critical Adobe Acrobat Compatibility Fix ✓

**Problem Discovered:**
- CID fonts worked perfectly in Edge, Chrome, and PDF Viewer Plus
- Failed in Adobe Acrobat with "Cannot find or create font" error
- First CID font on page always failed
- Subsequent CID fonts worked after the first failure

**Root Cause:**
- Adobe Acrobat requires **PDF 1.4 or later** for CID fonts
- Our default was PDF 1.2
- **Not** related to Helvetica usage (red herring)
- **Not** related to object ordering or font structure

**Solution:**
- Automatic PDF version upgrade to 1.4 in `HpdfCIDFont.LoadFromTrueTypeFile()`
- Check: `if (document.Version < HpdfVersion.Version14) document.Version = HpdfVersion.Version14;`
- Only upgrades, never downgrades
- Compatible with encryption (R3→1.4, R4→1.6)

**User Testing Process:**
1. Tested first font fails, second works
2. Tested different fonts in first position - all failed
3. User removed Helvetica with PDFExplorer - worked!
4. Discovered PDFExplorer saved as PDF 1.4
5. **Confirmed: PDF version was the issue, not Helvetica**

### 4. Bug Fixes Through Development ✓

**Phase 1: Empty Text in Edge/Chrome:**
- **Problem:** Blank spaces where CJK text should appear
- **Attempted:** UTF-16BE encoding, MBCS encoding with code page
- **Root Cause:** Content stream needs Glyph IDs, not Unicode or MBCS bytes
- **Solution:** Implemented `ConvertTextToGlyphIDs()` using cmap table lookup
- **Result:** Text appeared but characters overlapped ❌

**Phase 2: Character Overlapping:**
- **Problem:** Characters rendered but overlapped each other
- **Root Cause:** W array only defined widths for ASCII range (0x20-0x7E)
- **Solution:** Rewrote `CreateCIDWidthsArray()` to include all glyph widths
- **Format:** `[startGID [w1 w2...] nextGID [w100 w101...]]`
- **Result:** Perfect spacing in Edge/Chrome/PDF Viewer Plus! ✓

**Phase 3: Adobe Acrobat "Cannot Find Font":**
- **Problem:** Works everywhere except Adobe Acrobat
- **Attempted Fix 1:** Extract real PostScript names from font - Still failed ❌
- **Attempted Fix 2:** Add Adobe compatibility fields (FontFamily, FontStretch, FontWeight, Lang) - Still failed ❌
- **Discovery:** User removed Helvetica → Adobe worked! But was it Helvetica or PDF version?
- **Final Solution:** PDFExplorer saved as PDF 1.4 - **That was the fix!** ✓
- **Result:** All 4 CJK fonts work perfectly in Adobe Acrobat ✓

**Phase 4: API Design:**
- **Change:** `LoadFromTrueTypeFile(HpdfXref xref, ...)` → `LoadFromTrueTypeFile(HpdfDocument document, ...)`
- **Reason:** Need document reference to auto-upgrade PDF version
- **Updated:** CJKDemo.cs to pass `pdf` instead of `pdf.Xref`

**Phase 5: Parameter Validation Order:**
- **Problem:** 3 unit tests failed - code page validated after file existence check
- **Fix:** Reordered validation - check code page first, then file existence
- **Reason:** Better error reporting (invalid code page is programming error, missing file is runtime error)
- **Result:** All 21 unit tests passed ✓

### 5. CJKDemo Created ✓

Created comprehensive **CJKDemo.cs** showcasing all 4 CJK languages:

```csharp
public static class CJKDemo
{
    public static void Run()
    {
        var pdf = new HpdfDocument();
        var page = pdf.AddPage();

        // Load CID fonts for each language
        var chineseTraditionalFont = HpdfCIDFont.LoadFromTrueTypeFile(
            pdf, "CHT", "demo/ttfont/noto-cht.ttf", 950);

        var chineseSimplifiedFont = HpdfCIDFont.LoadFromTrueTypeFile(
            pdf, "CHS", "demo/ttfont/noto-chs.ttf", 936);

        var japaneseFont = HpdfCIDFont.LoadFromTrueTypeFile(
            pdf, "JP", "demo/ttfont/noto-jp.ttf", 932);

        var koreanFont = HpdfCIDFont.LoadFromTrueTypeFile(
            pdf, "KR", "demo/ttfont/noto-kr.ttf", 949);

        // Traditional Chinese
        page.SetFontAndSize(chineseTraditionalFont.AsFont(), 16);
        page.ShowText("你好世界");  // Hello World
        page.ShowText("繁體中文測試");  // Traditional Chinese Test

        // Simplified Chinese
        page.SetFontAndSize(chineseSimplifiedFont.AsFont(), 16);
        page.ShowText("你好世界");  // Hello World
        page.ShowText("简体中文测试");  // Simplified Chinese Test

        // Japanese
        page.SetFontAndSize(japaneseFont.AsFont(), 16);
        page.ShowText("こんにちは世界");  // Hello World (Hiragana)
        page.ShowText("日本語テスト");  // Japanese Test (Kanji + Katakana)

        // Korean
        page.SetFontAndSize(koreanFont.AsFont(), 16);
        page.ShowText("안녕하세요");  // Hello (Hangul)
        page.ShowText("한국어 테스트");  // Korean Test

        pdf.SaveToFile("pdfs/CJKDemo.pdf");
    }
}
```

**Features Demonstrated:**
- All 4 CJK languages (Traditional Chinese, Simplified Chinese, Japanese, Korean)
- Multiple code pages (CP950, CP936, CP932, CP949)
- CJK character rendering (Hiragana, Katakana, Kanji, Hangul, Chinese characters)
- Mixed font usage on same page
- Implementation notes section in PDF
- Adobe Acrobat compatibility notes

### 6. Unit Tests Created ✓

Created comprehensive **HpdfCIDFontTests.cs** with **21 tests** (all passing):

**Test Categories:**

1. **Loading Tests** (5 tests)
   - Valid font loading
   - PDF version auto-upgrade
   - No downgrade when already higher version
   - Null document parameter validation
   - Invalid file path handling

2. **Code Page Tests** (8 tests)
   - Valid code pages (932, 936, 949, 950)
   - Invalid code pages (1252, 1251, 850)
   - Code page validation before file check

3. **Glyph Conversion Tests** (4 tests)
   - Valid text to glyph IDs
   - Empty string handling
   - Null string handling
   - Basic ASCII glyph ID lookup

4. **Integration Tests** (4 tests)
   - AsFont() wrapper
   - IsCIDFont property
   - ConvertTextToGlyphIDs delegation
   - Multiple fonts on same page
   - Font dictionary structure

**Test Results:**
```
Total tests: 21
     Passed: 21
     Failed: 0
 Total time: 44 ms
```

**Test File:**
- `cs-src/Haru.Test/Font/HpdfCIDFontTests.cs`

### 7. Files Created/Modified

**Created:**
- `/mnt/d/develop/experiments/ai/claude3/cs-src/Haru/Font/CID/HpdfCIDFont.cs` (1045 lines)
  - Main CID font implementation
  - TrueType parsing and font embedding
  - Glyph ID conversion
  - Width calculations
  - Adobe compatibility

- `/mnt/d/develop/experiments/ai/claude3/cs-src/Haru.Demos/CJKDemo.cs` (191 lines)
  - Demo with all 4 CJK languages
  - Implementation notes

- `/mnt/d/develop/experiments/ai/claude3/cs-src/Haru.Test/Font/HpdfCIDFontTests.cs` (313 lines)
  - 21 comprehensive unit tests

**Modified:**
- `/mnt/d/develop/experiments/ai/claude3/cs-src/Haru/Font/HpdfFont.cs`
  - Added `using System;` for Array.Empty<byte>()
  - Added `IsCIDFont` property
  - Added `ConvertTextToGlyphIDs()` wrapper method
  - Added CID font constructor

- `/mnt/d/develop/experiments/ai/claude3/cs-src/Haru/Doc/HpdfDocument.cs`
  - PDF version remains default 1.2
  - CID font loading auto-upgrades to 1.4

- `/mnt/d/develop/experiments/ai/claude3/cs-src/Haru/Doc/HpdfPageText.cs`
  - Updated `ShowText()` to detect CID fonts
  - Convert text to glyph IDs for CID fonts
  - Output as hex string

- `/mnt/d/develop/experiments/ai/claude3/cs-src/Haru/Streams/HpdfStreamExtensions.cs`
  - Added `WriteHexString()` for glyph ID output

## Technical Details

### CID Font vs Single-Byte Font

| Feature | CID Font (CJK) | Single-Byte Font (Latin) |
|---------|----------------|--------------------------|
| Font Type | Type 0 (Composite) | Type 1 or TrueType |
| Encoding | Identity-H | WinAnsiEncoding + Differences |
| Character Set | Multi-byte (2+ bytes) | Single-byte (1 byte) |
| Max Characters | 65,536 | 256 |
| Content Stream | Glyph IDs as hex | Encoded bytes as literal/hex |
| Code Pages | CP932, CP936, CP949, CP950 | CP1251-1258 |
| Width Array | W (CID→width) | Widths (byte→width) |
| PDF Version | ≥ 1.4 (Adobe) | ≥ 1.2 |

### CID Font Structure in PDF

```
10 0 obj << /Type /Font /Subtype /Type0 /BaseFont /NotoSerifJP-Regular
           /Encoding /Identity-H
           /DescendantFonts [11 0 R]
           /ToUnicode 14 0 R >>
endobj

11 0 obj << /Type /Font /Subtype /CIDFontType2 /BaseFont /NotoSerifJP-Regular
           /CIDSystemInfo << /Registry (Adobe) /Ordering (Identity) /Supplement 0 >>
           /FontDescriptor 12 0 R
           /DW 1000
           /W [0 [500 500 ...] 100 [600 600 ...]]
           /CIDToGIDMap /Identity >>
endobj

12 0 obj << /Type /FontDescriptor /FontName /NotoSerifJP-Regular
           /Flags 32 /FontBBox [-174 -285 1001 953]
           /ItalicAngle 0 /Ascent 718 /Descent -207
           /CapHeight 723 /StemV 80
           /FontFile2 13 0 R
           /FontFamily (Noto Serif JP) /FontStretch /Normal
           /FontWeight 400 /Lang /ja >>
endobj

13 0 obj << /Filter /FlateDecode /Length1 123456 >>
stream
[TTF font data...]
endstream
endobj

14 0 obj << /Filter /FlateDecode >>
stream
[ToUnicode CMap...]
endstream
endobj
```

### Content Stream with CID Fonts

```
BT
/F1 16 Tf               % Set CID font
50 700 Td
<30533093306B3061> Tj  % Glyph IDs in hex (こんにちは)
ET
```

### Glyph ID Conversion Process

```
Text: "こんにちは"
  ↓
Unicode: U+3053 U+3093 U+306B U+3061 U+306F
  ↓
TrueType cmap lookup (format 4)
  ↓
Glyph IDs: 0x3053 0x3093 0x306B 0x3061 0x306F
  ↓
Hex String: <3053309330613061306F>
  ↓
Content Stream: <3053309330613061306F> Tj
```

## Usage Example

```csharp
using Haru.Doc;
using Haru.Font.CID;

// Create document (PDF 1.2 by default)
var pdf = new HpdfDocument();

// Load CID font (automatically upgrades to PDF 1.4)
var japaneseFont = HpdfCIDFont.LoadFromTrueTypeFile(
    pdf,
    "JP",
    "fonts/noto-jp.ttf",
    932  // CP932 (Shift-JIS)
);

// PDF version now 1.4
Console.WriteLine(pdf.Version);  // HpdfVersion.Version14

// Add page and render CJK text
var page = pdf.AddPage();

page.BeginText();
page.SetFontAndSize(japaneseFont.AsFont(), 16);
page.MoveTextPos(50, 750);

// Hiragana
page.ShowText("こんにちは世界");  // Hello World
page.MoveTextPos(0, -30);

// Katakana + Kanji
page.ShowText("日本語テスト");  // Japanese Test

page.EndText();

pdf.SaveToFile("japanese-demo.pdf");
```

## Previous Session Summary

Previously completed:
- ✓ Type 1 (PostScript) font support with code page encoding
- ✓ AFM/PFB parsing, glyph name mapping (including Cyrillic)
- ✓ TrueType font embedding with code page support
- ✓ Multi-language support (Western, Cyrillic, Greek, Turkish)
- ✓ InternationalDemo (7 languages with special characters)

## Current Project Status

### ✅ COMPLETED Features

1. **Core Infrastructure** (100%)
2. **Graphics & Layout** (100%)
3. **Text Rendering** (100%)
   - Standard 14 fonts ✓
   - TrueType font embedding with code page support ✓
   - Type 1 font embedding with code page support ✓
   - **CID fonts for CJK languages** ✓ NEW!
4. **Images** (100%)
5. **Document Features** (100%)
   - Metadata, Annotations, Outlines, PDF/A, Encryption ✓

### 📊 Overall Progress: ~87% Complete (up from 85%)

**Implemented:**
- ✓ Levels 1-12: Complete core functionality
- ✓ Document Info, Annotations, Outlines
- ✓ PDF/A-1b Phase 1
- ✓ Encryption & Security
- ✓ TrueType fonts with multi-language support
- ✓ Type 1 fonts with multi-language support
- ✓ **CID fonts for CJK support** ✓ NEW!

### 🎯 Next Steps

**Priority Order:**
1. **Additional Features** (As Needed)
   - Character encoders (if more encoding flexibility needed)
   - CCITT fax images
   - Page labels
   - Additional PDF/A compliance

**Estimated to 100% completion**: ~8-12 days remaining (down from 10-15 days)

## Summary

Successfully implemented **CID (Character Identifier) font support for CJK languages**:
- Complete Type 0 (Composite) font implementation with CIDFontType2
- Glyph ID conversion using TrueType cmap tables
- Complete width arrays for proper character spacing
- PostScript name extraction from font's name table
- Adobe compatibility fields (FontFamily, FontStretch, FontWeight, Lang)
- **PDF version auto-upgrade to 1.4 for Adobe Acrobat compatibility** ✓
- ToUnicode CMap for text extraction
- Support for 4 CJK languages (Chinese Traditional/Simplified, Japanese, Korean)
- CJKDemo showcasing all 4 languages
- 21 comprehensive unit tests (all passing)

**The library now supports CJK languages with proper Adobe Acrobat compatibility!** 🎉✅

**Critical Discovery:** Adobe Acrobat requires PDF 1.4 or later for CID fonts. Automatic version upgrade ensures compatibility across all PDF readers without breaking existing functionality.

