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

using Haru.Encoding;
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
        private readonly IHpdfEncoder _encoder;
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

            // Get encoder for the specified code page
            string encoderName = MapCodePageToEncoderName(codePage);
            _encoder = HpdfEncoderFactory.GetEncoder(encoderName);

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
                // Use the encoder's base encoding name
                string baseEncodingName = _encoder.BaseEncodingName;

                // If no differences, just use the base encoding name
                if (!_encoder.HasDifferences)
                {
                    _dict.Add("Encoding", new HpdfName(baseEncodingName));
                }
                else
                {
                    // Create custom Encoding with Differences array from encoder
                    var encodingDict = new HpdfDict();
                    encodingDict.Add("Type", new HpdfName("Encoding"));
                    encodingDict.Add("BaseEncoding", new HpdfName(baseEncodingName));

                    var differences = _encoder.CreateDifferencesArray();
                    if (differences != null && differences.Count > 0)
                    {
                        encodingDict.Add("Differences", differences);
                        xref.Add(encodingDict);
                        _dict.Add("Encoding", encodingDict);
                    }
                    else
                    {
                        // No differences, use base encoding
                        _dict.Add("Encoding", new HpdfName(baseEncodingName));
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
        /// Maps a code page number to an encoder name.
        /// </summary>
        private static string MapCodePageToEncoderName(int codePage)
        {
            return codePage switch
            {
                1250 => "CP1250",
                1251 => "CP1251",
                1252 => "WinAnsiEncoding",  // CP1252 is the same as WinAnsiEncoding
                1253 => "CP1253",
                1254 => "CP1254",
                1255 => "CP1255",
                1256 => "CP1256",
                1257 => "CP1257",
                1258 => "CP1258",
                20866 => "KOI8-R",         // Russian Cyrillic
                28592 => "ISO8859-2",      // Latin-2 (Central European)
                28593 => "ISO8859-3",      // Latin-3 (South European)
                28594 => "ISO8859-4",      // Latin-4 (North European)
                28595 => "ISO8859-5",      // Cyrillic
                28596 => "ISO8859-6",      // Arabic
                28597 => "ISO8859-7",      // Greek
                28598 => "ISO8859-8",      // Hebrew
                28599 => "ISO8859-9",      // Turkish
                28600 => "ISO8859-10",     // Nordic
                28601 => "ISO8859-11",     // Thai
                28603 => "ISO8859-13",     // Baltic Rim
                28604 => "ISO8859-14",     // Celtic
                28605 => "ISO8859-15",     // Western European with Euro
                28606 => "ISO8859-16",     // South-Eastern European
                _ => "WinAnsiEncoding"     // Default fallback
            };
        }

        public float GetCharWidth(byte charCode)
        {
            return _widths.GetWidth(charCode);
        }

        public float MeasureText(string text, float fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // Convert Unicode text using the font's encoder
            byte[] bytes = _encoder.EncodeText(text);

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
