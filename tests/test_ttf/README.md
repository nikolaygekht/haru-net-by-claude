# TrueType Font Embedding Test

This test demonstrates the TrueType font embedding feature in the Haru PDF Library.

## What This Test Does

1. Downloads and uses **Roboto Regular** font from Google Fonts
2. Embeds the font in a PDF document
3. Writes the classic pangram: **"The quick brown fox jumps over the lazy dog"**
4. Demonstrates various font features:
   - Font embedding (FontFile2)
   - ToUnicode CMap for text extraction
   - Font metrics calculation
   - Multiple text lines with various character sets

## Test Output

The test creates `output_roboto_test.pdf` containing:
- The pangram in lowercase and uppercase
- Numbers and special characters
- Information about the embedded font
- Font metrics demonstration

## Running the Test

```bash
# From the project root directory
cd tests/test_ttf
dotnet run
```

## Expected Results

```
=== TrueType Font Embedding Test ===

✓ Font file found: Roboto-Regular.ttf
  Size: 503 KB

✓ PDF document created
✓ Page added (A4 Portrait)
✓ TrueType font loaded and embedded
  Font name: CustomTTFont
  Local name: Roboto
  Font embedded: True
  ToUnicode CMap: True
  Italic angle: 0
  Flags: 34

✓ Text written to page
✓ PDF saved: output_roboto_test.pdf
  File size: ~258 KB

=== TEST PASSED ===
```

## What to Verify

Open `output_roboto_test.pdf` and verify:

1. **Text is rendered with Roboto font** - The text should have the distinctive Roboto appearance
2. **Text can be selected and copied** - ToUnicode CMap enables text extraction
3. **Font is embedded** - The PDF can be viewed on systems without Roboto installed
4. **All characters display correctly** - Including numbers and special characters

## Technical Details

### Font Information
- **Font**: Roboto Regular
- **Source**: Google Fonts (https://fonts.google.com/specimen/Roboto)
- **Format**: TrueType (.ttf)
- **Size**: ~503 KB (original), ~258 KB (in PDF with compression)

### Features Demonstrated
- ✅ TrueType font parsing
- ✅ Font embedding (FontFile2 stream)
- ✅ FlateDecode compression
- ✅ ToUnicode CMap generation
- ✅ Font descriptor with accurate metrics
- ✅ Font flags calculation (Nonsymbolic, etc.)
- ✅ Italic angle detection
- ✅ Text measurement and metrics

### PDF Structure
The generated PDF includes:
- Font dictionary with proper Type/Subtype
- Font descriptor with metrics (Ascent, Descent, CapHeight, StemV, ItalicAngle)
- Embedded font data in FontFile2 stream (compressed)
- ToUnicode CMap for Unicode text mapping
- WinAnsiEncoding for character encoding

## Files

- `Program.cs` - Test program
- `test_ttf.csproj` - Project file
- `Roboto-Regular.ttf` - Font file (downloaded from Google Fonts)
- `output_roboto_test.pdf` - Generated PDF output
- `README.md` - This file

## Notes

- The test automatically downloads the Roboto font on first run
- The font is embedded with compression, reducing file size significantly
- Text in the PDF can be searched and extracted thanks to the ToUnicode CMap
- The embedded font ensures the PDF displays correctly on all systems

## Success Criteria

✅ Font loads without errors
✅ Font is properly embedded (FontFile2 present)
✅ ToUnicode CMap is generated
✅ PDF is created successfully
✅ PDF file size is reasonable (~258 KB)
✅ All text is rendered correctly
✅ Text can be selected and copied

All criteria met! ✅
