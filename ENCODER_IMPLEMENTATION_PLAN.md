# Encoder Implementation Plan

## Overview

This document describes the plan to implement a complete encoder system for the Haru C# port, matching the original libharu C implementation's architecture. This would replace the current hardcoded Cyrillic encoding support with a flexible, extensible system supporting 22+ encodings.

**Status**: PARTIALLY IMPLEMENTED
**Estimated Effort**: 10-15 hours (original estimate)
**Date Created**: 2025-01-06
**Last Updated**: 2025-01-09

## Progress Update (2025-01-09)

### Completed Work

**Phase 1.2: Glyph Name Mapping Table** ✅ COMPLETED
- **File**: `/home/gleb/work/gehtsoft/haru-net-by-claude/cs-src/Haru/Font/HpdfGlyphNames.cs` (NEW - 1068 lines)
- Implemented complete Unicode → PostScript glyph name mapping table with 1051 entries
- Extracted from libharu C source `HPDF_UNICODE_GRYPH_NAME_MAP` table
- Includes mappings for:
  - Basic Latin, Latin Extended
  - Greek (U+0384 - U+03CE)
  - Cyrillic (U+0401 - U+04D9) including uppercase, lowercase, and extended forms
  - Hebrew (U+05B0 - U+05F2)
  - Arabic (U+060C - U+06D5)
  - Special characters and symbols (em-dash, en-dash, mathematical symbols, etc.)
- Static `GetGlyphName(ushort unicode)` method for lookup

**Standard Font Dynamic Encoding** ✅ COMPLETED
- **File**: `/home/gleb/work/gehtsoft/haru-net-by-claude/cs-src/Haru/Font/HpdfStandardFontImpl.cs` (MODIFIED)
- Removed hardcoded Cyrillic-only encoding
- Implemented dynamic `/Differences` array generation based on code page
- New `CreateDifferencesArray()` method that:
  - Compares target encoding with WinAnsiEncoding byte-by-byte
  - Uses `HpdfGlyphNames.GetGlyphName()` for proper PostScript glyph names
  - Only includes bytes that differ from base encoding
- For CP1252 (WinAnsi), uses base encoding directly without custom dictionary
- For other code pages (CP1250, CP1251, CP1253-1258, KOI8-R, ISO8859-x):
  - Creates custom encoding dictionary with `/BaseEncoding` and `/Differences`
  - Dynamically generates glyph names using .NET's `System.Text.Encoding`
- **Tested and verified**: Hebrew (CP1255), Arabic (CP1256), Cyrillic (CP1251, KOI8-R), and other encodings

**TrueType Font Glyph Names** ✅ COMPLETED
- **File**: `/home/gleb/work/gehtsoft/haru-net-by-claude/cs-src/Haru/Font/HpdfTrueTypeFont.cs` (MODIFIED)
- Updated to use `HpdfGlyphNames.GetGlyphName()` instead of `uni{unicode:X4}` format
- Falls back to `uni{XXXX}` format for unmapped characters
- Properly handles non-ASCII characters in TrueType fonts

**Commit**: `74e5b43` - "feat: Implement Unicode to PostScript glyph name mapping and dynamic encoding support for Standard fonts"

### Remaining Work

The following phases from the original plan are **not yet implemented** and remain planned for future work:

**Phase 1.1: Define Encoder Interfaces and Classes** ⏳ NOT STARTED
- `IHpdfEncoder` interface
- `HpdfBasicEncoder` class
- Full encoder abstraction matching libharu architecture

**Phase 2: Create Encoding Definitions** ⏳ NOT STARTED
- Extract complete unicode_map arrays from libharu C source for all 22 encodings
- `HpdfEncoderFactory` with all encoder registrations
- Dedicated encoder classes for each code page

**Phase 3-6: Full Integration** ⏳ NOT STARTED
- Complete integration with font system
- Update stream extensions to use encoder abstraction
- Comprehensive unit tests
- Documentation updates

### Current Implementation Notes

The current implementation (completed 2025-01-09) provides a **lightweight, pragmatic solution** that:
- ✅ Uses .NET's `System.Text.Encoding` to dynamically get Unicode mappings for any code page
- ✅ Leverages the HpdfGlyphNames table to map Unicode → PostScript glyph names
- ✅ Generates `/Differences` arrays on-the-fly without hardcoded tables
- ✅ Supports all encodings that .NET supports (CP125x, KOI8-R, ISO8859-x, etc.)
- ✅ Tested with Hebrew, Arabic, Cyrillic, and other encodings - all working correctly

This approach differs from the full encoder system described below (which would use hardcoded unicode_map arrays from libharu), but achieves the same functional result with less code complexity. The full encoder system remains available for future implementation if needed for:
- Exact byte-for-byte parity with libharu
- CJK (Chinese/Japanese/Korean) encodings
- Performance optimization (pre-computed tables vs dynamic lookup)

## Background

### Current Implementation (Hardcoded Approach)

The current Haru C# port implementation in `HpdfStandardFontImpl.cs` hardcodes Cyrillic support by:
1. Always creating a custom `/Encoding` dictionary for standard fonts
2. Hardcoding a `/Differences` array mapping bytes 192-255 to Cyrillic glyph names (afii10017-afii10097)
3. Using hex string format `<XXXX>` for all text output

This works for Cyrillic (CP1251) but:
- Cannot support other encodings (CP1250, CP1253-1258, ISO8859-x, KOI8-R, etc.)
- Hardcodes glyph mappings instead of computing them dynamically
- Does not match the original libharu architecture

### Original LibHaru Architecture

The original C implementation uses a sophisticated encoder system:

**File**: `hpdf_encoder.h` and `hpdf_encoder.c`

**Key Structure**:
```c
typedef struct _HPDF_BasicEncoderAttr_Rec {
    HPDF_BYTE            sig_bytes[HPDF_LIMIT_MAX_NAME_LEN];
    HPDF_EncType         type;
    HPDF_WritingMode     writing_mode;
    HPDF_UINT16          unicode_map[256];        // Byte → Unicode mapping
    char                 differences[256];        // Flags for /Differences array
    HPDF_CMapEncodingType cmap_encoding_type;
    HPDF_BOOL            has_differences;
    HPDF_ParseText_Rec   parse_text_fn;
} HPDF_BasicEncoderAttr_Rec;
```

**Supported Encodings** (22 total):
- **Windows Code Pages**: CP1250, CP1251, CP1252, CP1253, CP1254, CP1255, CP1256, CP1257, CP1258
- **ISO Encodings**: ISO8859-2 through ISO8859-16 (except 12)
- **Cyrillic**: KOI8-R
- **CJK**: 90ms-RKSJ-H, 90msp-RKSJ-H, EUC-H, ETen-B5-H, GBK-EUC-H

**Unicode to Glyph Name Mapping**:
The C code has a large table (~1000 entries) mapping Unicode codepoints to PostScript glyph names:
```c
static const HPDF_CID_Width UNICODE_TO_GRYPH_NAME[] = {
    {0x0020, "space"},
    {0x0021, "exclam"},
    // ... ~1000 more entries
    {0x0410, "afii10017"},  // Cyrillic А
    {0x0411, "afii10018"},  // Cyrillic Б
    // ...
};
```

**Dynamic Differences Generation**:
LibHaru compares each encoding's unicode_map against the base encoding (StandardEncoding or WinAnsiEncoding) and dynamically generates `/Differences` arrays only for bytes that differ.

## Implementation Plan

### Phase 1: Create Encoder Infrastructure (3-4 hours)

#### 1.1 Define Encoder Interfaces and Classes

**File**: `Haru/Encoding/IHpdfEncoder.cs` (NEW)
```csharp
namespace Haru.Encoding
{
    /// <summary>
    /// Interface for PDF text encoders.
    /// Maps between Unicode text and byte sequences for PDF fonts.
    /// </summary>
    public interface IHpdfEncoder
    {
        /// <summary>
        /// Gets the encoder name (e.g., "WinAnsiEncoding", "CP1251", "ISO8859-5").
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the base encoding name for the /BaseEncoding entry.
        /// </summary>
        string BaseEncodingName { get; }

        /// <summary>
        /// Gets whether this encoder requires a /Differences array.
        /// </summary>
        bool HasDifferences { get; }

        /// <summary>
        /// Gets the Unicode codepoint for a given byte value.
        /// </summary>
        /// <param name="byteValue">The byte value (0-255).</param>
        /// <returns>The Unicode codepoint, or 0 if unmapped.</returns>
        ushort GetUnicode(byte byteValue);

        /// <summary>
        /// Gets the byte value for a given Unicode codepoint.
        /// Returns null if the character cannot be encoded.
        /// </summary>
        byte? GetByteValue(char unicodeChar);

        /// <summary>
        /// Creates the /Differences array for this encoding.
        /// Returns null if no differences from base encoding.
        /// </summary>
        HpdfArray? CreateDifferencesArray();

        /// <summary>
        /// Encodes Unicode text to bytes using this encoder.
        /// </summary>
        byte[] EncodeText(string text);
    }
}
```

**File**: `Haru/Encoding/HpdfBasicEncoder.cs` (NEW)
```csharp
namespace Haru.Encoding
{
    /// <summary>
    /// Basic single-byte encoder implementation matching libharu's HPDF_BasicEncoderAttr.
    /// </summary>
    public class HpdfBasicEncoder : IHpdfEncoder
    {
        private readonly string _name;
        private readonly string _baseEncodingName;
        private readonly ushort[] _unicodeMap;      // [256] byte → Unicode
        private readonly bool[] _differences;       // [256] flags
        private readonly Dictionary<char, byte> _reverseMap;  // Unicode → byte

        public string Name => _name;
        public string BaseEncodingName => _baseEncodingName;
        public bool HasDifferences => _differences.Any(d => d);

        public HpdfBasicEncoder(
            string name,
            string baseEncodingName,
            ushort[] unicodeMap,
            bool[]? differences = null)
        {
            _name = name;
            _baseEncodingName = baseEncodingName;
            _unicodeMap = unicodeMap;
            _differences = differences ?? new bool[256];

            // Build reverse map for encoding
            _reverseMap = new Dictionary<char, byte>();
            for (int i = 0; i < 256; i++)
            {
                if (_unicodeMap[i] != 0)
                {
                    char ch = (char)_unicodeMap[i];
                    if (!_reverseMap.ContainsKey(ch))
                        _reverseMap[ch] = (byte)i;
                }
            }
        }

        public ushort GetUnicode(byte byteValue) => _unicodeMap[byteValue];

        public byte? GetByteValue(char unicodeChar)
        {
            return _reverseMap.TryGetValue(unicodeChar, out byte b) ? b : null;
        }

        public HpdfArray? CreateDifferencesArray()
        {
            if (!HasDifferences)
                return null;

            var array = new HpdfArray();
            int startByte = -1;

            for (int i = 0; i < 256; i++)
            {
                if (_differences[i])
                {
                    // Start a new difference run
                    if (startByte == -1)
                    {
                        startByte = i;
                        array.Add(new HpdfNumber(i));
                    }

                    // Add glyph name
                    ushort unicode = _unicodeMap[i];
                    string? glyphName = HpdfGlyphNames.GetGlyphName(unicode);
                    if (glyphName != null)
                    {
                        array.Add(new HpdfName(glyphName));
                    }
                }
                else
                {
                    startByte = -1;
                }
            }

            return array.Count > 0 ? array : null;
        }

        public byte[] EncodeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();

            var result = new List<byte>();
            foreach (char ch in text)
            {
                byte? b = GetByteValue(ch);
                if (b.HasValue)
                {
                    result.Add(b.Value);
                }
                else
                {
                    // Character not in encoding - use '?' as fallback
                    result.Add((byte)'?');
                }
            }
            return result.ToArray();
        }
    }
}
```

#### 1.2 Create Glyph Name Mapping Table

**File**: `Haru/Encoding/HpdfGlyphNames.cs` (NEW)

This file contains the static mapping from Unicode codepoints to PostScript glyph names, extracted from the libharu C source.

```csharp
namespace Haru.Encoding
{
    /// <summary>
    /// Maps Unicode codepoints to PostScript glyph names.
    /// Based on libharu's UNICODE_TO_GRYPH_NAME table.
    /// </summary>
    public static class HpdfGlyphNames
    {
        private static readonly Dictionary<ushort, string> _glyphNameMap = new()
        {
            // Basic Latin
            { 0x0020, "space" },
            { 0x0021, "exclam" },
            { 0x0022, "quotedbl" },
            // ... (full table from libharu - ~1000 entries)

            // Cyrillic Capital Letters (U+0410 - U+042F)
            { 0x0410, "afii10017" },  // CYRILLIC CAPITAL LETTER A
            { 0x0411, "afii10018" },  // CYRILLIC CAPITAL LETTER BE
            { 0x0412, "afii10019" },  // CYRILLIC CAPITAL LETTER VE
            { 0x0413, "afii10020" },  // CYRILLIC CAPITAL LETTER GHE
            { 0x0414, "afii10021" },  // CYRILLIC CAPITAL LETTER DE
            { 0x0415, "afii10022" },  // CYRILLIC CAPITAL LETTER IE
            { 0x0416, "afii10024" },  // CYRILLIC CAPITAL LETTER ZHE
            { 0x0417, "afii10025" },  // CYRILLIC CAPITAL LETTER ZE
            { 0x0418, "afii10026" },  // CYRILLIC CAPITAL LETTER I
            { 0x0419, "afii10027" },  // CYRILLIC CAPITAL LETTER SHORT I
            { 0x041A, "afii10028" },  // CYRILLIC CAPITAL LETTER KA
            { 0x041B, "afii10029" },  // CYRILLIC CAPITAL LETTER EL
            { 0x041C, "afii10030" },  // CYRILLIC CAPITAL LETTER EM
            { 0x041D, "afii10031" },  // CYRILLIC CAPITAL LETTER EN
            { 0x041E, "afii10032" },  // CYRILLIC CAPITAL LETTER O
            { 0x041F, "afii10033" },  // CYRILLIC CAPITAL LETTER PE
            { 0x0420, "afii10034" },  // CYRILLIC CAPITAL LETTER ER
            { 0x0421, "afii10035" },  // CYRILLIC CAPITAL LETTER ES
            { 0x0422, "afii10036" },  // CYRILLIC CAPITAL LETTER TE
            { 0x0423, "afii10037" },  // CYRILLIC CAPITAL LETTER U
            { 0x0424, "afii10038" },  // CYRILLIC CAPITAL LETTER EF
            { 0x0425, "afii10039" },  // CYRILLIC CAPITAL LETTER HA
            { 0x0426, "afii10040" },  // CYRILLIC CAPITAL LETTER TSE
            { 0x0427, "afii10041" },  // CYRILLIC CAPITAL LETTER CHE
            { 0x0428, "afii10042" },  // CYRILLIC CAPITAL LETTER SHA
            { 0x0429, "afii10043" },  // CYRILLIC CAPITAL LETTER SHCHA
            { 0x042A, "afii10044" },  // CYRILLIC CAPITAL LETTER HARD SIGN
            { 0x042B, "afii10045" },  // CYRILLIC CAPITAL LETTER YERU
            { 0x042C, "afii10046" },  // CYRILLIC CAPITAL LETTER SOFT SIGN
            { 0x042D, "afii10047" },  // CYRILLIC CAPITAL LETTER E
            { 0x042E, "afii10048" },  // CYRILLIC CAPITAL LETTER YU
            { 0x042F, "afii10049" },  // CYRILLIC CAPITAL LETTER YA

            // Cyrillic Lowercase Letters (U+0430 - U+044F)
            { 0x0430, "afii10065" },  // CYRILLIC SMALL LETTER A
            { 0x0431, "afii10066" },  // CYRILLIC SMALL LETTER BE
            { 0x0432, "afii10067" },  // CYRILLIC SMALL LETTER VE
            { 0x0433, "afii10068" },  // CYRILLIC SMALL LETTER GHE
            { 0x0434, "afii10069" },  // CYRILLIC SMALL LETTER DE
            { 0x0435, "afii10070" },  // CYRILLIC SMALL LETTER IE
            { 0x0436, "afii10072" },  // CYRILLIC SMALL LETTER ZHE
            { 0x0437, "afii10073" },  // CYRILLIC SMALL LETTER ZE
            { 0x0438, "afii10074" },  // CYRILLIC SMALL LETTER I
            { 0x0439, "afii10075" },  // CYRILLIC SMALL LETTER SHORT I
            { 0x043A, "afii10076" },  // CYRILLIC SMALL LETTER KA
            { 0x043B, "afii10077" },  // CYRILLIC SMALL LETTER EL
            { 0x043C, "afii10078" },  // CYRILLIC SMALL LETTER EM
            { 0x043D, "afii10079" },  // CYRILLIC SMALL LETTER EN
            { 0x043E, "afii10080" },  // CYRILLIC SMALL LETTER O
            { 0x043F, "afii10081" },  // CYRILLIC SMALL LETTER PE
            { 0x0440, "afii10082" },  // CYRILLIC SMALL LETTER ER
            { 0x0441, "afii10083" },  // CYRILLIC SMALL LETTER ES
            { 0x0442, "afii10084" },  // CYRILLIC SMALL LETTER TE
            { 0x0443, "afii10085" },  // CYRILLIC SMALL LETTER U
            { 0x0444, "afii10086" },  // CYRILLIC SMALL LETTER EF
            { 0x0445, "afii10087" },  // CYRILLIC SMALL LETTER HA
            { 0x0446, "afii10088" },  // CYRILLIC SMALL LETTER TSE
            { 0x0447, "afii10089" },  // CYRILLIC SMALL LETTER CHE
            { 0x0448, "afii10090" },  // CYRILLIC SMALL LETTER SHA
            { 0x0449, "afii10091" },  // CYRILLIC SMALL LETTER SHCHA
            { 0x044A, "afii10092" },  // CYRILLIC SMALL LETTER HARD SIGN
            { 0x044B, "afii10093" },  // CYRILLIC SMALL LETTER YERU
            { 0x044C, "afii10094" },  // CYRILLIC SMALL LETTER SOFT SIGN
            { 0x044D, "afii10095" },  // CYRILLIC SMALL LETTER E
            { 0x044E, "afii10096" },  // CYRILLIC SMALL LETTER YU
            { 0x044F, "afii10097" },  // CYRILLIC SMALL LETTER YA

            // ... Additional entries from libharu table
        };

        /// <summary>
        /// Gets the PostScript glyph name for a Unicode codepoint.
        /// </summary>
        public static string? GetGlyphName(ushort unicode)
        {
            return _glyphNameMap.TryGetValue(unicode, out string? name) ? name : null;
        }
    }
}
```

### Phase 2: Create Encoding Definitions (4-5 hours)

#### 2.1 Extract Encoding Maps from LibHaru C Source

Use the original libharu C source files to extract the unicode_map arrays for each encoding:

**Source Files**:
- `hpdf_encoder_utf.c` - UTF-8 support
- `hpdf_encoder_cns.c` - Chinese encodings
- `hpdf_encoder_cnt.c` - Chinese Traditional
- `hpdf_encoder_jp.c` - Japanese encodings
- `hpdf_encoder_kr.c` - Korean encodings

#### 2.2 Create Encoder Factory

**File**: `Haru/Encoding/HpdfEncoderFactory.cs` (NEW)

```csharp
namespace Haru.Encoding
{
    /// <summary>
    /// Factory for creating encoder instances.
    /// Matches libharu's encoder initialization.
    /// </summary>
    public static class HpdfEncoderFactory
    {
        private static readonly Dictionary<string, Func<IHpdfEncoder>> _encoders = new();

        static HpdfEncoderFactory()
        {
            // Register all supported encoders
            RegisterEncoder("WinAnsiEncoding", () => CreateWinAnsiEncoder());
            RegisterEncoder("CP1250", () => CreateCP1250Encoder());
            RegisterEncoder("CP1251", () => CreateCP1251Encoder());
            RegisterEncoder("CP1252", () => CreateCP1252Encoder());
            RegisterEncoder("CP1253", () => CreateCP1253Encoder());
            RegisterEncoder("CP1254", () => CreateCP1254Encoder());
            RegisterEncoder("CP1255", () => CreateCP1255Encoder());
            RegisterEncoder("CP1256", () => CreateCP1256Encoder());
            RegisterEncoder("CP1257", () => CreateCP1257Encoder());
            RegisterEncoder("CP1258", () => CreateCP1258Encoder());
            RegisterEncoder("ISO8859-2", () => CreateISO8859_2Encoder());
            RegisterEncoder("ISO8859-3", () => CreateISO8859_3Encoder());
            RegisterEncoder("ISO8859-4", () => CreateISO8859_4Encoder());
            RegisterEncoder("ISO8859-5", () => CreateISO8859_5Encoder());
            RegisterEncoder("ISO8859-6", () => CreateISO8859_6Encoder());
            RegisterEncoder("ISO8859-7", () => CreateISO8859_7Encoder());
            RegisterEncoder("ISO8859-8", () => CreateISO8859_8Encoder());
            RegisterEncoder("ISO8859-9", () => CreateISO8859_9Encoder());
            RegisterEncoder("ISO8859-10", () => CreateISO8859_10Encoder());
            RegisterEncoder("ISO8859-11", () => CreateISO8859_11Encoder());
            RegisterEncoder("ISO8859-13", () => CreateISO8859_13Encoder());
            RegisterEncoder("ISO8859-14", () => CreateISO8859_14Encoder());
            RegisterEncoder("ISO8859-15", () => CreateISO8859_15Encoder());
            RegisterEncoder("ISO8859-16", () => CreateISO8859_16Encoder());
            RegisterEncoder("KOI8-R", () => CreateKOI8REncoder());
        }

        private static void RegisterEncoder(string name, Func<IHpdfEncoder> factory)
        {
            _encoders[name] = factory;
            // Also register with .NET code page number if applicable
            if (name.StartsWith("CP"))
            {
                _encoders[name.Replace("CP", "")] = factory;
            }
        }

        public static IHpdfEncoder GetEncoder(string encodingName)
        {
            if (_encoders.TryGetValue(encodingName, out var factory))
            {
                return factory();
            }

            // Try to parse as code page number
            if (int.TryParse(encodingName, out int codePage))
            {
                string cpName = $"CP{codePage}";
                if (_encoders.TryGetValue(cpName, out factory))
                {
                    return factory();
                }
            }

            // Default to WinAnsiEncoding
            return CreateWinAnsiEncoder();
        }

        private static IHpdfEncoder CreateWinAnsiEncoder()
        {
            // WinAnsiEncoding is the base - no differences
            var unicodeMap = new ushort[256];
            // ... populate from libharu's HPDF_WINANSI_ENCODING array
            return new HpdfBasicEncoder("WinAnsiEncoding", "WinAnsiEncoding", unicodeMap);
        }

        private static IHpdfEncoder CreateCP1251Encoder()
        {
            var unicodeMap = new ushort[256];
            var differences = new bool[256];

            // ... populate from libharu's HPDF_CP1251_ENCODING array
            // Mark bytes that differ from WinAnsiEncoding

            return new HpdfBasicEncoder("CP1251", "WinAnsiEncoding", unicodeMap, differences);
        }

        // ... similar methods for all other encodings
    }
}
```

### Phase 3: Integrate with Font System (2-3 hours)

#### 3.1 Update HpdfStandardFontImpl

**File**: `Haru/Font/HpdfStandardFontImpl.cs` (MODIFY)

Replace the hardcoded Cyrillic encoding with dynamic encoder:

```csharp
public HpdfStandardFontImpl(HpdfXref xref, HpdfStandardFont standardFont, string localName, int codePage = 1252)
{
    // ... existing validation code ...

    _baseFont = standardFont.GetPostScriptName();
    _localName = localName;
    _codePage = codePage;

    // Create font dictionary
    _dict = new HpdfDict();
    _dict.Add("Type", new HpdfName("Font"));
    _dict.Add("Subtype", new HpdfName("Type1"));
    _dict.Add("BaseFont", new HpdfName(_baseFont));

    // Standard 14 fonts use StandardEncoding by default
    // Symbol and ZapfDingbats use their own built-in encoding
    if (standardFont != HpdfStandardFont.Symbol &&
        standardFont != HpdfStandardFont.ZapfDingbats)
    {
        // Get encoder for the specified code page
        var encoder = HpdfEncoderFactory.GetEncoder(codePage.ToString());

        // Only create custom encoding if there are differences from base encoding
        if (encoder.HasDifferences)
        {
            var encodingDict = new HpdfDict();
            encodingDict.Add("Type", new HpdfName("Encoding"));
            encodingDict.Add("BaseEncoding", new HpdfName(encoder.BaseEncodingName));

            var differences = encoder.CreateDifferencesArray();
            if (differences != null)
            {
                encodingDict.Add("Differences", differences);
            }

            xref.Add(encodingDict);
            _dict.Add("Encoding", encodingDict);
        }
        else
        {
            // Use base encoding directly
            _dict.Add("Encoding", new HpdfName(encoder.BaseEncodingName));
        }
    }

    // ... rest of constructor ...
}
```

#### 3.2 Update Text Measurement

**File**: `Haru/Font/HpdfStandardFontImpl.cs` (MODIFY)

Update `MeasureText` to use the encoder:

```csharp
public float MeasureText(string text, float fontSize)
{
    if (string.IsNullOrEmpty(text))
        return 0;

    // Use encoder for text conversion
    var encoder = HpdfEncoderFactory.GetEncoder(_codePage.ToString());
    byte[] bytes = encoder.EncodeText(text);

    float totalWidth = 0;
    foreach (byte b in bytes)
    {
        totalWidth += GetCharWidth(b);
    }

    return totalWidth * fontSize / 1000.0f;
}
```

### Phase 4: Update Stream Extensions (1 hour)

**File**: `Haru/Streams/HpdfStreamExtensions.cs` (MODIFY)

Update `WriteEscapedText` to use encoder:

```csharp
public static void WriteEscapedText(this HpdfStream stream, string text, int codePage)
{
    if (stream is null)
        throw new ArgumentNullException(nameof(stream));
    if (text is null)
        throw new ArgumentNullException(nameof(text));

    // Use encoder for text conversion
    var encoder = HpdfEncoderFactory.GetEncoder(codePage.ToString());
    byte[] bytes = encoder.EncodeText(text);

    // Write as hex string format <XXXX>
    // This works correctly with custom encodings
    stream.WriteByte((byte)'<');

    foreach (byte b in bytes)
    {
        stream.WriteByte(ToHexDigit((b >> 4) & 0x0F));
        stream.WriteByte(ToHexDigit(b & 0x0F));
    }

    stream.WriteByte((byte)'>');
}
```

### Phase 5: Testing (2-3 hours)

#### 5.1 Unit Tests

**File**: `Haru.Test/EncoderTests.cs` (NEW)

```csharp
public class EncoderTests
{
    [Fact]
    public void CP1251_Encoder_Should_Map_Cyrillic_Correctly()
    {
        var encoder = HpdfEncoderFactory.GetEncoder("CP1251");

        // Test uppercase Cyrillic А (U+0410)
        byte? byteValue = encoder.GetByteValue('А');
        Assert.Equal(192, byteValue);  // 0xC0

        // Test lowercase Cyrillic а (U+0430)
        byteValue = encoder.GetByteValue('а');
        Assert.Equal(224, byteValue);  // 0xE0
    }

    [Fact]
    public void CP1251_Encoder_Should_Create_Differences_Array()
    {
        var encoder = HpdfEncoderFactory.GetEncoder("CP1251");

        Assert.True(encoder.HasDifferences);

        var differences = encoder.CreateDifferencesArray();
        Assert.NotNull(differences);

        // Should start at byte 192 and contain Cyrillic glyph names
        var firstElement = differences.Values[0] as HpdfNumber;
        Assert.Equal(192, firstElement?.Value);

        var firstGlyph = differences.Values[1] as HpdfName;
        Assert.Equal("afii10017", firstGlyph?.Value);  // Cyrillic А
    }

    [Fact]
    public void WinAnsi_Encoder_Should_Not_Have_Differences()
    {
        var encoder = HpdfEncoderFactory.GetEncoder("WinAnsiEncoding");

        Assert.False(encoder.HasDifferences);
        Assert.Null(encoder.CreateDifferencesArray());
    }

    [Theory]
    [InlineData("CP1250")]  // Central European
    [InlineData("CP1253")]  // Greek
    [InlineData("CP1254")]  // Turkish
    [InlineData("CP1255")]  // Hebrew
    [InlineData("CP1256")]  // Arabic
    [InlineData("ISO8859-5")]  // Cyrillic
    [InlineData("KOI8-R")]  // Cyrillic
    public void All_Encoders_Should_Be_Creatable(string encodingName)
    {
        var encoder = HpdfEncoderFactory.GetEncoder(encodingName);
        Assert.NotNull(encoder);
        Assert.Equal(encodingName, encoder.Name);
    }
}
```

#### 5.2 Integration Tests

Test with actual PDF generation for all supported encodings:
- Create test PDFs with text in each encoding
- Verify `/Differences` arrays are correct in PDF output
- Verify text extraction using pdftotext/pdftohtml

### Phase 6: Documentation (1 hour)

Update documentation:
1. Update `CLAUDE.md` with new encoder architecture
2. Add encoder usage examples
3. Document supported encodings
4. Update migration guide

## Benefits of This Implementation

1. **Standards Compliance**: Matches original libharu architecture
2. **Extensibility**: Easy to add new encodings
3. **Maintainability**: Single source of truth for encoding mappings
4. **Dynamic**: Generates `/Differences` arrays automatically
5. **Complete**: Supports all 22+ libharu encodings
6. **Testable**: Clear separation of concerns

## Migration Path

This can be implemented without breaking existing code:
1. Current hardcoded approach continues to work
2. New encoder system runs in parallel
3. Gradually migrate fonts to use new system
4. Remove hardcoded logic once all tests pass

## Trade-offs

**Pros**:
- Complete feature parity with libharu C
- Supports all encoding scenarios
- Clean, maintainable architecture

**Cons**:
- More code complexity
- Requires extracting ~1000-entry glyph name table from C source
- Requires extracting unicode_map arrays for 22 encodings

## References

- Original libharu source: `/home/gleb/work/gehtsoft/haru-net-by-claude/c-src/hpdf_encoder.h`
- Original libharu source: `/home/gleb/work/gehtsoft/haru-net-by-claude/c-src/hpdf_encoder.c`
- Current implementation: `/home/gleb/work/gehtsoft/haru-net-by-claude/cs-src/Haru/Font/HpdfStandardFontImpl.cs`
- Current implementation: `/home/gleb/work/gehtsoft/haru-net-by-claude/cs-src/Haru/Streams/HpdfStreamExtensions.cs`

## Notes

- This plan was created on 2025-01-06 after successfully implementing the hardcoded Cyrillic encoding fix
- The hardcoded approach works perfectly for the current use case (Cyrillic support)
- This full encoder system is recommended for future work if additional encodings are needed
- Not prioritized for immediate implementation per user request
