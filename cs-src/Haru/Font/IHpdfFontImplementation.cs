/*
 * << Haru Free PDF Library >> -- IHpdfFontImplementation.cs
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

namespace Haru.Font
{
    /// <summary>
    /// Interface for font implementation providing metrics and text measurement capabilities.
    /// Implemented by Standard, TrueType, Type 1, and CID fonts.
    /// </summary>
    public interface IHpdfFontImplementation
    {
        /// <summary>
        /// Gets the font dictionary object.
        /// </summary>
        HpdfDict Dict { get; }

        /// <summary>
        /// Gets the BaseFont name (PostScript name).
        /// </summary>
        string BaseFont { get; }

        /// <summary>
        /// Gets the local name used to reference this font in page resources.
        /// </summary>
        string LocalName { get; }

        /// <summary>
        /// Gets the encoding code page for this font, or null for standard fonts.
        /// </summary>
        int? CodePage { get; }

        /// <summary>
        /// Gets the font ascent in 1000-unit glyph space.
        /// Ascent is the maximum height above the baseline.
        /// </summary>
        int Ascent { get; }

        /// <summary>
        /// Gets the font descent in 1000-unit glyph space.
        /// Descent is the maximum depth below the baseline (negative value).
        /// </summary>
        int Descent { get; }

        /// <summary>
        /// Gets the font x-height in 1000-unit glyph space.
        /// X-height is the height of lowercase 'x'.
        /// </summary>
        int XHeight { get; }

        /// <summary>
        /// Gets the font bounding box in 1000-unit glyph space.
        /// </summary>
        HpdfBox FontBBox { get; }

        /// <summary>
        /// Gets the width of a character in 1000-unit glyph space.
        /// </summary>
        /// <param name="charCode">The character code.</param>
        /// <returns>The character width.</returns>
        float GetCharWidth(byte charCode);

        /// <summary>
        /// Calculates the width of text in user space units.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="fontSize">The font size.</param>
        /// <returns>The text width in user space units.</returns>
        float MeasureText(string text, float fontSize);

        /// <summary>
        /// Converts text to bytes for use in PDF content stream.
        /// For CID fonts, this may return glyph IDs or encoded bytes depending on the encoding.
        /// For non-CID fonts, returns empty array (handled by page text encoding).
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>Byte array for PDF content stream, or empty if not applicable.</returns>
        byte[] ConvertTextToGlyphIDs(string text);

        /// <summary>
        /// Encodes text to bytes using the font's encoding.
        /// This is used for both measurement and output to ensure consistency.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <returns>Byte array encoded using the font's encoding.</returns>
        byte[] EncodeText(string text);
    }
}
