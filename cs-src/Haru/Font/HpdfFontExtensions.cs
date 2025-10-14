/*
 * << Haru Free PDF Library >> -- HpdfFontExtensions.cs
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

namespace Haru.Font
{
    /// <summary>
    /// Extension methods for HpdfFont providing advanced text measurement capabilities.
    /// </summary>
    public static class HpdfFontExtensions
    {
        /// <summary>
        /// Measures how many characters of text fit within a specified width with word wrapping support.
        /// This is the full version compatible with libharu's HPDF_Font_MeasureText.
        /// </summary>
        /// <param name="font">The font to use for measurement.</param>
        /// <param name="text">The text to measure.</param>
        /// <param name="fontSize">The font size in points.</param>
        /// <param name="width">The maximum width constraint in user space units.</param>
        /// <param name="charSpace">Character spacing in user space units.</param>
        /// <param name="wordSpace">Word spacing (added to each whitespace character) in user space units.</param>
        /// <param name="wordWrap">If true, break only at word boundaries; if false, break anywhere.</param>
        /// <param name="realWidth">OUTPUT: The actual width of the text that fits.</param>
        /// <returns>The number of characters that fit within the specified width.</returns>
        public static int MeasureText(
            this HpdfFont font,
            string text,
            float fontSize,
            float width,
            float charSpace,
            float wordSpace,
            bool wordWrap,
            out float realWidth)
        {
            realWidth = 0;

            if (string.IsNullOrEmpty(text))
                return 0;

            float w = 0;
            int tmpLen = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                byte b = (byte)c;

                // Handle whitespace
                if (IsWhiteSpace(b))
                {
                    tmpLen = i + 1;  // Can break AFTER whitespace
                    realWidth = w;
                    w += wordSpace;
                }
                else if (!wordWrap)
                {
                    // Non-wordwrap mode: can break anywhere
                    tmpLen = i;  // Can break BEFORE this character
                    realWidth = w;
                }

                // Add character width (scaled from 1000-unit glyph space to user space)
                w += font.GetCharWidth(b) * fontSize / 1000f;

                // Check if we exceeded width OR hit line feed
                if (w > width || b == 0x0A)
                    return tmpLen;

                // Add character spacing (except for first character)
                if (i > 0)
                    w += charSpace;
            }

            // All text fits within the specified width
            realWidth = w;
            return text.Length;
        }

        /// <summary>
        /// Checks if a byte value represents a whitespace character.
        /// Whitespace includes: null, tab, line feed, form feed, carriage return, and space.
        /// </summary>
        /// <param name="b">The byte value to check.</param>
        /// <returns>True if the byte is whitespace, false otherwise.</returns>
        private static bool IsWhiteSpace(byte b)
        {
            return b == 0x00 ||  // null
                   b == 0x09 ||  // tab
                   b == 0x0A ||  // line feed
                   b == 0x0C ||  // form feed
                   b == 0x0D ||  // carriage return
                   b == 0x20;    // space
        }
    }
}
