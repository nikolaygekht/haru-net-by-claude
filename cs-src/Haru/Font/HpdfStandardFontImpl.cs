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

        public HpdfDict Dict => _dict;
        public string BaseFont => _baseFont;
        public string LocalName => _localName;
        public int? CodePage => null; // Standard fonts don't have a code page

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
        public HpdfStandardFontImpl(HpdfXref xref, HpdfStandardFont standardFont, string localName)
        {
            if (xref == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            if (string.IsNullOrEmpty(localName))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Local name cannot be null or empty");

            _baseFont = standardFont.GetPostScriptName();
            _localName = localName;

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
                _dict.Add("Encoding", new HpdfName("WinAnsiEncoding"));
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

            float totalWidth = 0;
            foreach (char c in text)
            {
                totalWidth += GetCharWidth((byte)c);
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
