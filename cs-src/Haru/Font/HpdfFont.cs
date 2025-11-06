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

using Haru.Objects;
using Haru.Xref;
using Haru.Font.CID;
using Haru.Types;

namespace Haru.Font
{
    /// <summary>
    /// Represents a PDF font resource.
    /// </summary>
    public class HpdfFont
    {
        private readonly IHpdfFontImplementation _implementation;
        private readonly bool _isCIDFont;

        /// <summary>
        /// Gets the underlying dictionary object for this font.
        /// </summary>
        public HpdfDict Dict => _implementation.Dict;

        /// <summary>
        /// Gets the BaseFont name (PostScript name).
        /// </summary>
        public string BaseFont => _implementation.BaseFont;

        /// <summary>
        /// Gets the local name used to reference this font in page resources.
        /// </summary>
        public string LocalName => _implementation.LocalName;

        /// <summary>
        /// Gets the encoding code page for this font (for TrueType/Type 1/CID fonts), or null for standard fonts.
        /// </summary>
        public int? EncodingCodePage => _implementation.CodePage;

        /// <summary>
        /// Gets whether this is a CID font (Type 0 composite font with Identity-H encoding).
        /// CID fonts with CIDToGIDMap=Identity require glyph IDs in the content stream.
        /// </summary>
        public bool IsCIDFont => _isCIDFont;

        /// <summary>
        /// Converts text to glyph IDs for CID fonts.
        /// For CID fonts with CIDToGIDMap=Identity, text must be converted to glyph IDs.
        /// For CIDFontType0 with predefined encodings, returns encoded bytes.
        /// </summary>
        public byte[] ConvertTextToGlyphIDs(string text)
        {
            return _implementation.ConvertTextToGlyphIDs(text);
        }

        /// <summary>
        /// Creates a standard Type 1 font.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="standardFont">The standard font to use.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="codePage">The code page to use for text encoding (default 1252 for WinAnsiEncoding).</param>
        public HpdfFont(HpdfXref xref, HpdfStandardFont standardFont, string localName, int codePage = 1252)
        {
            _implementation = new HpdfStandardFontImpl(xref, standardFont, localName, codePage);
            _isCIDFont = false;
        }

        /// <summary>
        /// Creates a font wrapper for a TrueType font.
        /// </summary>
        /// <param name="ttFont">The TrueType font to wrap.</param>
        internal HpdfFont(HpdfTrueTypeFont ttFont)
        {
            _implementation = ttFont ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "TrueType font cannot be null");
            _isCIDFont = false;
        }

        /// <summary>
        /// Creates a font wrapper for a Type 1 font.
        /// </summary>
        /// <param name="type1Font">The Type 1 font to wrap.</param>
        internal HpdfFont(HpdfType1Font type1Font)
        {
            _implementation = type1Font ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Type 1 font cannot be null");
            _isCIDFont = false;
        }

        /// <summary>
        /// Creates a font wrapper for a CID font (CIDFontType2 - embedded TrueType).
        /// </summary>
        /// <param name="cidFont">The CID font to wrap.</param>
        internal HpdfFont(HpdfCIDFont cidFont)
        {
            _implementation = cidFont ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "CID font cannot be null");
            _isCIDFont = true;
        }

        /// <summary>
        /// Creates a font wrapper for a CIDFontType0 (predefined CJK font).
        /// </summary>
        /// <param name="cidFontType0">The CIDFontType0 to wrap.</param>
        internal HpdfFont(CID.HpdfCIDFontType0 cidFontType0)
        {
            _implementation = cidFontType0 ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "CIDFontType0 cannot be null");
            _isCIDFont = true;
        }

        /// <summary>
        /// Gets the font ascent in 1000-unit glyph space.
        /// Ascent is the maximum height above the baseline.
        /// </summary>
        /// <returns>The ascent value.</returns>
        public int GetAscent() => _implementation.Ascent;

        /// <summary>
        /// Gets the font descent in 1000-unit glyph space.
        /// Descent is the maximum depth below the baseline (negative value).
        /// </summary>
        /// <returns>The descent value.</returns>
        public int GetDescent() => _implementation.Descent;

        /// <summary>
        /// Gets the font x-height in 1000-unit glyph space.
        /// X-height is the height of lowercase 'x'.
        /// </summary>
        /// <returns>The x-height value.</returns>
        public int GetXHeight() => _implementation.XHeight;

        /// <summary>
        /// Gets the font bounding box in 1000-unit glyph space.
        /// </summary>
        /// <returns>The font bounding box.</returns>
        public HpdfBox GetBBox() => _implementation.FontBBox;

        /// <summary>
        /// Gets the width of a character in 1000-unit glyph space.
        /// </summary>
        /// <param name="charCode">The character code.</param>
        /// <returns>The character width.</returns>
        public float GetCharWidth(byte charCode) => _implementation.GetCharWidth(charCode);

        /// <summary>
        /// Calculates the width of text in user space units.
        /// Delegates to the specific font type implementation.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="fontSize">The font size.</param>
        /// <returns>The text width in user space units.</returns>
        public float MeasureText(string text, float fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return _implementation.MeasureText(text, fontSize);
        }
    }
}
