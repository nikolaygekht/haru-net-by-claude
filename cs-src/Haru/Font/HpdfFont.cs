/*
 * << Haru Free PDF Library >> -- HpdfFont.cs
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

using System;
using Haru.Objects;
using Haru.Xref;
using Haru.Font.CID;

namespace Haru.Font
{
    /// <summary>
    /// Represents a PDF font resource.
    /// </summary>
    public class HpdfFont
    {
        private readonly HpdfDict _dict;
        private readonly string _baseFont;
        private readonly string _localName;
        private readonly HpdfTrueTypeFont _ttFont;
        private readonly HpdfType1Font _type1Font;
        private readonly HpdfCIDFont _cidFont;

        /// <summary>
        /// Gets the underlying dictionary object for this font.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the BaseFont name (PostScript name).
        /// </summary>
        public string BaseFont => _baseFont;

        /// <summary>
        /// Gets the local name used to reference this font in page resources.
        /// </summary>
        public string LocalName => _localName;

        /// <summary>
        /// Gets the encoding code page for this font (for TrueType/Type 1/CID fonts), or null for standard fonts.
        /// </summary>
        public int? EncodingCodePage => _ttFont?.CodePage ?? _type1Font?.CodePage ?? _cidFont?.CodePage;

        /// <summary>
        /// Gets whether this is a CID font (Type 0 composite font with Identity-H encoding).
        /// CID fonts with CIDToGIDMap=Identity require glyph IDs in the content stream.
        /// </summary>
        public bool IsCIDFont => _cidFont != null;

        /// <summary>
        /// Converts text to glyph IDs for CID fonts.
        /// For CID fonts with CIDToGIDMap=Identity, text must be converted to glyph IDs.
        /// </summary>
        public byte[] ConvertTextToGlyphIDs(string text)
        {
            if (_cidFont != null)
                return _cidFont.ConvertTextToGlyphIDs(text);

            return Array.Empty<byte>();
        }

        /// <summary>
        /// Creates a standard Type 1 font.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="standardFont">The standard font to use.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        public HpdfFont(HpdfXref xref, HpdfStandardFont standardFont, string localName)
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
        }

        /// <summary>
        /// Creates a font wrapper for a TrueType font.
        /// </summary>
        /// <param name="ttFont">The TrueType font to wrap.</param>
        internal HpdfFont(HpdfTrueTypeFont ttFont)
        {
            if (ttFont == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "TrueType font cannot be null");

            _ttFont = ttFont;
            _dict = ttFont.Dict;
            _baseFont = ttFont.BaseFont;
            _localName = ttFont.LocalName;
        }

        /// <summary>
        /// Creates a font wrapper for a Type 1 font.
        /// </summary>
        /// <param name="type1Font">The Type 1 font to wrap.</param>
        internal HpdfFont(HpdfType1Font type1Font)
        {
            if (type1Font == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Type 1 font cannot be null");

            _type1Font = type1Font;
            _dict = type1Font.Dict;
            _baseFont = type1Font.BaseFont;
            _localName = type1Font.LocalName;
        }

        /// <summary>
        /// Creates a font wrapper for a CID font.
        /// </summary>
        /// <param name="cidFont">The CID font to wrap.</param>
        internal HpdfFont(HpdfCIDFont cidFont)
        {
            if (cidFont == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "CID font cannot be null");

            _cidFont = cidFont;
            _dict = cidFont.Dict;
            _baseFont = cidFont.BaseFont;
            _localName = cidFont.LocalName;
        }

        /// <summary>
        /// Gets the width of a character in 1000-unit glyph space.
        /// For standard fonts, this is a simplified implementation.
        /// </summary>
        /// <param name="charCode">The character code.</param>
        /// <returns>The character width.</returns>
        public float GetCharWidth(byte charCode)
        {
            // Simplified: return average width for now
            // A full implementation would use actual font metrics
            return 500; // Average width in 1000-unit space
        }

        /// <summary>
        /// Calculates the width of text in user space units.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="fontSize">The font size.</param>
        /// <returns>The text width in user space units.</returns>
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
    }
}
