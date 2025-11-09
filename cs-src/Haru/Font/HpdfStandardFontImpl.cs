/*
 * << Haru Free PDF Library >> -- HpdfStandardFontImpl.cs
 *
 * C# port of Haru Free PDF Library
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 * provided that the above copyright notice appear in all copies and
 * that both that copyright notice and this permission notice appear
 * in supporting documentation.
 * It is provided "as is" without express or implied warranty.
 *
 */

using Haru.Objects;
using Haru.Types;
using Haru.Xref;

namespace Haru.Font
{
    /// <summary>
    /// Implementation for PDF Standard 14 fonts.
    /// </summary>
    internal class HpdfStandardFontImpl : IHpdfFontImplementation
    {
        private readonly HpdfDict _dict;
        private readonly string _baseFont;
        private readonly string _localName;
        private readonly StandardFontMetrics _metrics;
        private readonly HpdfStandardFontWidths _widths;
        private readonly int _codePage;

        public HpdfDict Dict => _dict;
        public string BaseFont => _baseFont;
        public string LocalName => _localName;
        public int? CodePage => _codePage;

        public int Ascent => _metrics.Ascent;
        public int Descent => _metrics.Descent;
        public int XHeight => _metrics.XHeight;
        public HpdfBox FontBBox => _metrics.FontBBox;

        /// <summary>
        /// Creates a standard Type 1 font implementation.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="standardFont">The standard font to use.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="codePage">The code page to use for text encoding (default 1252 for WinAnsiEncoding).</param>
        public HpdfStandardFontImpl(HpdfXref xref, HpdfStandardFont standardFont, string localName, int codePage = 1252)
        {
            if (xref is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            if (string.IsNullOrEmpty(localName))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Local name cannot be null or empty");

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
                // For code page 1252 (WinAnsi), use the base encoding directly - no Differences needed
                if (codePage == 1252)
                {
                    _dict.Add("Encoding", new HpdfName("WinAnsiEncoding"));
                }
                else
                {
                    // For other code pages, create custom Encoding with Differences array
                    // Generate Differences dynamically based on the code page
                    var encodingDict = new HpdfDict();
                    encodingDict.Add("Type", new HpdfName("Encoding"));
                    encodingDict.Add("BaseEncoding", new HpdfName("WinAnsiEncoding"));

                    // Get the encoding for this code page
                    System.Text.Encoding encoding;
                    try
                    {
                        encoding = System.Text.Encoding.GetEncoding(codePage);
                    }
                    catch
                    {
                        // Fallback to WinAnsi if code page not available
                        encoding = System.Text.Encoding.GetEncoding(1252);
                    }

                    // Build Differences array by comparing with WinAnsi
                    var differences = CreateDifferencesArray(encoding);

                    if (differences != null && differences.Count > 0)
                    {
                        encodingDict.Add("Differences", differences);
                        xref.Add(encodingDict);
                        _dict.Add("Encoding", encodingDict);
                    }
                    else
                    {
                        // No differences, use base encoding
                        _dict.Add("Encoding", new HpdfName("WinAnsiEncoding"));
                    }
                }
            }

            // Add to xref
            xref.Add(_dict);

            // Initialize metrics and width table
            _metrics = HpdfStandardFontMetrics.GetMetrics(standardFont);
            _widths = new HpdfStandardFontWidths(standardFont);
        }

        /// <summary>
        /// Creates a /Differences array for the encoding by comparing with WinAnsiEncoding (CP1252).
        /// Only includes bytes that differ from WinAnsi.
        /// </summary>
        private HpdfArray? CreateDifferencesArray(System.Text.Encoding encoding)
        {
            // Get WinAnsi encoding for comparison
            var winAnsi = System.Text.Encoding.GetEncoding(1252);

            var differences = new HpdfArray();
            int rangeStart = -1;
            var rangeGlyphs = new System.Collections.Generic.List<string>();

            // Compare all 256 byte values
            for (int i = 0; i < 256; i++)
            {
                byte b = (byte)i;
                byte[] byteArray = new byte[] { b };

                // Get Unicode from both encodings
                string targetChar = encoding.GetString(byteArray);
                string winAnsiChar = winAnsi.GetString(byteArray);

                ushort targetUnicode = (targetChar.Length > 0) ? (ushort)targetChar[0] : (ushort)0;
                ushort winAnsiUnicode = (winAnsiChar.Length > 0) ? (ushort)winAnsiChar[0] : (ushort)0;

                // Skip if the encoding matches WinAnsi or is in ASCII range (typically same)
                if (targetUnicode == winAnsiUnicode || (i >= 0x20 && i <= 0x7E))
                {
                    // Flush current range if any
                    if (rangeStart >= 0)
                    {
                        differences.Add(new HpdfNumber(rangeStart));
                        foreach (var glyph in rangeGlyphs)
                        {
                            differences.Add(new HpdfName(glyph));
                        }
                        rangeStart = -1;
                        rangeGlyphs.Clear();
                    }
                    continue;
                }

                // Get proper glyph name for this Unicode value
                string? glyphName = HpdfGlyphNames.GetGlyphName(targetUnicode);
                if (glyphName == null)
                {
                    // Flush current range and skip this byte if no glyph name found
                    if (rangeStart >= 0)
                    {
                        differences.Add(new HpdfNumber(rangeStart));
                        foreach (var glyph in rangeGlyphs)
                        {
                            differences.Add(new HpdfName(glyph));
                        }
                        rangeStart = -1;
                        rangeGlyphs.Clear();
                    }
                    continue;
                }

                // Start a new range or continue current one
                if (rangeStart < 0)
                {
                    rangeStart = i;
                }
                rangeGlyphs.Add(glyphName);
            }

            // Flush final range if any
            if (rangeStart >= 0)
            {
                differences.Add(new HpdfNumber(rangeStart));
                foreach (var glyph in rangeGlyphs)
                {
                    differences.Add(new HpdfName(glyph));
                }
            }

            return differences.Count > 0 ? differences : null;
        }

        public float GetCharWidth(byte charCode)
        {
            return _widths.GetWidth(charCode);
        }

        public float MeasureText(string text, float fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // Convert Unicode text using the font's code page encoding
            // Standard fonts may use different encodings (e.g., WinAnsiEncoding 1252, CP1251 for Cyrillic)
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(_codePage);
            byte[] bytes = encoding.GetBytes(text);

            float totalWidth = 0;
            foreach (byte b in bytes)
            {
                totalWidth += GetCharWidth(b);
            }

            // Convert from 1000-unit glyph space to user space
            return totalWidth * fontSize / 1000.0f;
        }

        public byte[] ConvertTextToGlyphIDs(string text)
        {
            throw new System.InvalidOperationException("The method is available for CID fonts only");
        }
    }
}
