/*
 * << Haru Free PDF Library >> -- HpdfCIDFontType0.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using System.Collections.Generic;
using Haru.Objects;
using Haru.Xref;
using Haru.Types;
using Haru.Streams;
using Haru.Doc;

namespace Haru.Font.CID
{
    /// <summary>
    /// Represents a CIDFontType0 (predefined CJK font with built-in metrics).
    /// Does NOT embed font data - references system fonts by name.
    /// Requires the font to be installed on the viewer's system.
    /// </summary>
    public class HpdfCIDFontType0 : IHpdfFontImplementation
    {
        private readonly HpdfDict _dict;               // Type 0 (composite) font dictionary
        private readonly HpdfDict _cidFontDict;        // CIDFontType0 descendant font dictionary
        private readonly HpdfDict _descriptor;         // Font descriptor
        private readonly string _baseFont;
        private readonly string _localName;
        private readonly HpdfXref _xref;
        private readonly int _codePage;
        private readonly string _encodingName;
        private readonly CIDEncoder _encoder; // Optional encoder

        // Font definition (contains metrics and widths)
        private readonly PredefinedFontDefinition _fontDef;
        private HpdfStreamObject _toUnicodeStream;

        /// <summary>
        /// Gets the underlying Type 0 font dictionary.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the BaseFont name.
        /// </summary>
        public string BaseFont => _baseFont;

        /// <summary>
        /// Gets the local name used to reference this font in page resources.
        /// </summary>
        public string LocalName => _localName;

        /// <summary>
        /// Gets the code page used for text encoding.
        /// </summary>
        public int? CodePage => _codePage;

        /// <summary>
        /// Gets the font ascent in 1000-unit glyph space.
        /// </summary>
        public int Ascent => _fontDef.Metrics.Ascent;

        /// <summary>
        /// Gets the font descent in 1000-unit glyph space.
        /// </summary>
        public int Descent => _fontDef.Metrics.Descent;

        /// <summary>
        /// Gets the x-height in 1000-unit glyph space.
        /// </summary>
        public int XHeight => _fontDef.Metrics.CapHeight; // Use cap height as approximation

        /// <summary>
        /// Gets the font bounding box in 1000-unit glyph space.
        /// </summary>
        public HpdfBox FontBBox => _fontDef.Metrics.FontBBox;

        /// <summary>
        /// Gets an HpdfFont wrapper for this CID font that can be used with page operations.
        /// </summary>
        public HpdfFont AsFont()
        {
            return new HpdfFont(this);
        }

        private HpdfCIDFontType0(
            HpdfXref xref,
            string localName,
            PredefinedFontDefinition fontDef,
            int codePage,
            string encodingName,
            CIDEncoder encoder = null)
        {
            _xref = xref ?? throw new ArgumentNullException(nameof(xref));
            _localName = localName ?? throw new ArgumentNullException(nameof(localName));
            _fontDef = fontDef ?? throw new ArgumentNullException(nameof(fontDef));
            _baseFont = fontDef.FontName;
            _codePage = codePage;
            _encodingName = encodingName ?? throw new ArgumentNullException(nameof(encodingName));
            _encoder = encoder; // Optional

            _dict = new HpdfDict();
            _cidFontDict = new HpdfDict();
            _descriptor = new HpdfDict();
        }

        /// <summary>
        /// Creates a CIDFontType0 font from a predefined font definition.
        /// </summary>
        /// <param name="document">The PDF document.</param>
        /// <param name="fontName">The font name (e.g., "SimSun", "MingLiU").</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="codePage">The code page (e.g., 936 for GBK, 950 for Big5).</param>
        /// <param name="encodingName">The encoding name (e.g., "GBK-EUC-H", "ETen-B5-H").</param>
        /// <param name="encoder">Optional CIDEncoder instance. If not provided, uses .NET's code page encoding.</param>
        /// <returns>A new CIDFontType0 instance.</returns>
        public static HpdfCIDFontType0 Create(
            HpdfDocument document,
            string fontName,
            string localName,
            int codePage,
            string encodingName,
            CIDEncoder encoder = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            // Load font definition (from registry or embedded resource)
            var fontDef = PredefinedFontRegistry.GetDefinition(fontName);

            // CIDFontType0 requires PDF 1.4+
            if (document.Version < HpdfVersion.Version14)
            {
                document.Version = HpdfVersion.Version14;
            }

            var font = new HpdfCIDFontType0(
                document.Xref,
                localName,
                fontDef,
                codePage,
                encodingName,
                encoder);

            // Build PDF objects
            font.CreateFontDescriptor();
            font.CreateCIDFontDictionary();
            font.CreateToUnicodeCMap();
            font.CreateType0FontDictionary();

            return font;
        }

        /// <summary>
        /// Creates a SimSun CIDFontType0 font (POC - hardcoded data).
        /// This is a convenience method for testing.
        /// </summary>
        public static HpdfCIDFontType0 CreateSimSun(HpdfDocument document, string localName)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            // SimSun font metrics from C source (hpdf_fontdef_cns.c:275-282)
            var metrics = new FontMetrics
            {
                Ascent = 859,
                Descent = -140,
                CapHeight = 683,
                FontBBox = new HpdfBox(0, -140, 996, 855),
                Flags = 0x04 | 0x01 | 0x02, // SYMBOLIC + FIXED_WIDTH + SERIF
                ItalicAngle = 0,
                StemV = 78,
                DefaultWidth = 1000
            };

            // Width array from C source (SIMSUN_W_ARRAY)
            var widths = new Dictionary<ushort, short>();

            // CIDs 668-694 (width 500)
            for (ushort cid = 668; cid <= 694; cid++)
            {
                if (cid != 695) widths[cid] = 500;
            }

            // CIDs 696-699 (width 500)
            for (ushort cid = 696; cid <= 699; cid++)
            {
                widths[cid] = 500;
            }

            // CIDs 814-907 (width 500)
            for (ushort cid = 814; cid <= 907; cid++)
            {
                widths[cid] = 500;
            }

            // CID 7716 (width 500)
            widths[7716] = 500;

            // CIDSystemInfo for Simplified Chinese (Adobe-GB1)
            var systemInfo = new CIDSystemInfo
            {
                Registry = "Adobe",
                Ordering = "GB1",
                Supplement = 2
            };

            // Create font definition
            var fontDef = PredefinedFontDefinition.Create("SimSun", metrics, systemInfo, widths);

            // Register in cache for consistency
            PredefinedFontRegistry.Register("SimSun", fontDef);

            // Use the general Create method
            return Create(document, "SimSun", localName, 936, "GBK-EUC-H");
        }

        /// <summary>
        /// Step 1: Create FontDescriptor (but no FontFile2 - predefined font).
        /// </summary>
        private void CreateFontDescriptor()
        {
            _descriptor.Add("Type", new HpdfName("FontDescriptor"));
            _descriptor.Add("FontName", new HpdfName(_baseFont));
            _descriptor.Add("Flags", new HpdfNumber(_fontDef.Metrics.Flags));

            // Add font bounding box
            var bbox = new HpdfArray();
            bbox.Add(new HpdfNumber((int)_fontDef.Metrics.FontBBox.Left));
            bbox.Add(new HpdfNumber((int)_fontDef.Metrics.FontBBox.Bottom));
            bbox.Add(new HpdfNumber((int)_fontDef.Metrics.FontBBox.Right));
            bbox.Add(new HpdfNumber((int)_fontDef.Metrics.FontBBox.Top));
            _descriptor.Add("FontBBox", bbox);

            _descriptor.Add("ItalicAngle", new HpdfNumber(_fontDef.Metrics.ItalicAngle));
            _descriptor.Add("Ascent", new HpdfNumber(_fontDef.Metrics.Ascent));
            _descriptor.Add("Descent", new HpdfNumber(_fontDef.Metrics.Descent));
            _descriptor.Add("CapHeight", new HpdfNumber(_fontDef.Metrics.CapHeight));
            _descriptor.Add("StemV", new HpdfNumber(_fontDef.Metrics.StemV));

            // NOTE: No FontFile2 entry - this is a predefined font

            // Add to xref
            _xref.Add(_descriptor);
        }

        /// <summary>
        /// Step 2: Create CIDFontType0 dictionary (descendant font).
        /// </summary>
        private void CreateCIDFontDictionary()
        {
            _cidFontDict.Add("Type", new HpdfName("Font"));
            _cidFontDict.Add("Subtype", new HpdfName("CIDFontType0"));
            _cidFontDict.Add("BaseFont", new HpdfName(_baseFont));

            // Add CIDSystemInfo
            _cidFontDict.Add("CIDSystemInfo", _fontDef.SystemInfo.ToDict());

            // Link to FontDescriptor
            _cidFontDict.Add("FontDescriptor", _descriptor);

            // Add default width
            _cidFontDict.Add("DW", new HpdfNumber(_fontDef.Metrics.DefaultWidth));

            // Add widths array (W)
            if (_fontDef.Widths != null && _fontDef.Widths.Length > 0)
            {
                CreateCIDWidthsArray();
            }

            // Add to xref
            _xref.Add(_cidFontDict);
        }

        /// <summary>
        /// Creates the W (widths) array for CIDFontType0.
        /// Format: [c1 c2 w] - CIDs from c1 to c2 have width w
        /// Or: [c [w1 w2 ...]] - CID c and following have widths w1, w2, ...
        /// </summary>
        private void CreateCIDWidthsArray()
        {
            var widths = new HpdfArray();

            // _fontDef.Widths is already sorted by CID (from PredefinedFontDefinition.Load)
            // Group consecutive CIDs with same width into ranges
            int i = 0;
            while (i < _fontDef.Widths.Length)
            {
                ushort startCID = _fontDef.Widths[i].CID;
                short width = _fontDef.Widths[i].Width;
                ushort endCID = startCID;

                // Find consecutive CIDs with same width
                while (i + 1 < _fontDef.Widths.Length &&
                       _fontDef.Widths[i + 1].CID == endCID + 1 &&
                       _fontDef.Widths[i + 1].Width == width)
                {
                    i++;
                    endCID = _fontDef.Widths[i].CID;
                }

                // Add range: [startCID endCID width]
                widths.Add(new HpdfNumber(startCID));
                widths.Add(new HpdfNumber(endCID));
                widths.Add(new HpdfNumber(width));

                i++;
            }

            _cidFontDict.Add("W", widths);
        }

        /// <summary>
        /// Step 3: Create ToUnicode CMap.
        /// </summary>
        private void CreateToUnicodeCMap()
        {
            // For POC, create a minimal ToUnicode CMap
            // Map common Chinese characters (你好 = U+4F60 U+597D)
            var cidToUnicode = new Dictionary<ushort, ushort>
            {
                // ASCII range
                {0x20, 0x20}, {0x21, 0x21}, {0x22, 0x22}, {0x23, 0x23},
                {0x24, 0x24}, {0x25, 0x25}, {0x26, 0x26}, {0x27, 0x27},
                {0x28, 0x28}, {0x29, 0x29}, {0x2A, 0x2A}, {0x2B, 0x2B},
                {0x2C, 0x2C}, {0x2D, 0x2D}, {0x2E, 0x2E}, {0x2F, 0x2F},

                // Common Chinese characters
                {0x4F60, 0x4F60}, // 你 (you)
                {0x597D, 0x597D}, // 好 (good/hello)
                {0x4E16, 0x4E16}, // 世 (world)
                {0x754C, 0x754C}, // 界 (boundary/world)
            };

            // Generate ToUnicode CMap
            string cmapContent = CMapGenerator.GenerateToUnicodeCMap(_fontDef.SystemInfo, cidToUnicode);

            // Create stream object
            _toUnicodeStream = new HpdfStreamObject();
            byte[] cmapBytes = System.Text.Encoding.UTF8.GetBytes(cmapContent);
            _toUnicodeStream.WriteToStream(cmapBytes);
            _toUnicodeStream.Filter = HpdfStreamFilter.FlateDecode;

            // Add to xref
            _xref.Add(_toUnicodeStream);
        }

        /// <summary>
        /// Step 4: Create Type 0 (composite) font dictionary.
        /// </summary>
        private void CreateType0FontDictionary()
        {
            _dict.Add("Type", new HpdfName("Font"));
            _dict.Add("Subtype", new HpdfName("Type0"));
            _dict.Add("BaseFont", new HpdfName(_baseFont));

            // Encoding: e.g., "GBK-EUC-H" for horizontal writing
            _dict.Add("Encoding", new HpdfName(_encodingName));

            // DescendantFonts array (contains the CIDFont)
            var descendantFonts = new HpdfArray();
            descendantFonts.Add(_cidFontDict);
            _dict.Add("DescendantFonts", descendantFonts);

            // Link ToUnicode CMap
            _dict.Add("ToUnicode", _toUnicodeStream);

            // Add to xref
            _xref.Add(_dict);
        }

        /// <summary>
        /// Gets the width of a CID in 1000-unit glyph space.
        /// </summary>
        public short GetCIDWidth(ushort cid)
        {
            return _fontDef.GetCIDWidth(cid);
        }

        /// <summary>
        /// Gets the width of a character in 1000-unit glyph space.
        /// For CIDFontType0, we need to convert the byte to CID using the encoder.
        /// For POC, we're using a simple identity mapping for testing.
        /// </summary>
        public float GetCharWidth(byte charCode)
        {
            // For POC: treat byte as CID directly (identity mapping)
            // In full implementation, this would use the encoder
            ushort cid = charCode;
            return GetCIDWidth(cid);
        }

        /// <summary>
        /// Measures text width in user space units.
        /// </summary>
        public float MeasureText(string text, float fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // For POC: use Unicode code points directly as CIDs
            float totalWidth = 0;
            foreach (char c in text)
            {
                ushort cid = (ushort)c;
                totalWidth += GetCIDWidth(cid);
            }

            // Scale from 1000-unit glyph space to user space
            return totalWidth * fontSize / 1000f;
        }

        /// <summary>
        /// Converts text to CID codes for use in content stream.
        /// For CIDFontType0 with GBK-EUC-H, text must be encoded as GBK bytes.
        /// </summary>
        public byte[] ConvertTextToBytes(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();

            // Use encoder if provided, otherwise fall back to .NET code page
            if (_encoder != null)
            {
                return _encoder.EncodeText(text);
            }

            // Fallback: Use .NET's code page encoding directly
            var encoding = System.Text.Encoding.GetEncoding(_codePage);
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// Converts text to glyph IDs for use in PDF content stream.
        /// For CIDFontType0 with predefined encodings like GBK-EUC-H,
        /// this returns the GBK-encoded bytes (NOT Unicode glyph IDs).
        /// </summary>
        public byte[] ConvertTextToGlyphIDs(string text)
        {
            // For CIDFontType0, we use the encoding's byte representation
            // This is different from CIDFontType2 which uses Unicode→GlyphID mapping
            return ConvertTextToBytes(text);
        }
    }
}
