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
                // Always create custom encoding with Cyrillic support in Differences array
                // This matches old libharu behavior and allows Cyrillic to render via font substitution
                var encodingDict = new HpdfDict();
                encodingDict.Add("Type", new HpdfName("Encoding"));
                encodingDict.Add("BaseEncoding", new HpdfName("WinAnsiEncoding"));

                // Create Differences array to map bytes 192-255 to Cyrillic glyphs
                // CP1252 best-fit mapping maps Cyrillic to bytes 192-255, so we remap those to Cyrillic glyphs
                var differences = new HpdfArray();
                differences.Add(new HpdfNumber(192));  // Start at byte 192 (0xC0)

                // Cyrillic capital letters А-Я (afii10017 through afii10049)
                string[] cyrillicCaps = {
                    "afii10017", "afii10018", "afii10019", "afii10020", "afii10021", "afii10022",
                    "afii10024", "afii10025", "afii10026", "afii10027", "afii10028", "afii10029",
                    "afii10030", "afii10031", "afii10032", "afii10033", "afii10034", "afii10035",
                    "afii10036", "afii10037", "afii10038", "afii10039", "afii10040", "afii10041",
                    "afii10042", "afii10043", "afii10044", "afii10045", "afii10046", "afii10047",
                    "afii10048", "afii10049"
                };

                // Cyrillic lowercase letters а-я (afii10065 through afii10097)
                string[] cyrillicLower = {
                    "afii10065", "afii10066", "afii10067", "afii10068", "afii10069", "afii10070",
                    "afii10072", "afii10073", "afii10074", "afii10075", "afii10076", "afii10077",
                    "afii10078", "afii10079", "afii10080", "afii10081", "afii10082", "afii10083",
                    "afii10084", "afii10085", "afii10086", "afii10087", "afii10088", "afii10089",
                    "afii10090", "afii10091", "afii10092", "afii10093", "afii10094", "afii10095",
                    "afii10096", "afii10097"
                };

                // Add capital letters (bytes 192-223)
                foreach (var glyph in cyrillicCaps)
                {
                    differences.Add(new HpdfName(glyph));
                }

                // Add lowercase letters (bytes 224-255)
                foreach (var glyph in cyrillicLower)
                {
                    differences.Add(new HpdfName(glyph));
                }

                encodingDict.Add("Differences", differences);
                xref.Add(encodingDict);
                _dict.Add("Encoding", encodingDict);
            }

            // Add to xref
            xref.Add(_dict);

            // Initialize metrics and width table
            _metrics = HpdfStandardFontMetrics.GetMetrics(standardFont);
            _widths = new HpdfStandardFontWidths(standardFont);
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
