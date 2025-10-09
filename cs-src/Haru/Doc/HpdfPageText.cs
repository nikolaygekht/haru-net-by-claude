/*
 * << Haru Free PDF Library >> -- HpdfPageText.cs
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

using Haru.Font;
using Haru.Types;
using Haru.Objects;
using Haru.Streams;
using System.Text;

namespace Haru.Doc
{
    /// <summary>
    /// Text operations for HpdfPage
    /// </summary>
    public static class HpdfPageText
    {
        // Text Object Operations

        /// <summary>
        /// Begins a text object (BT operator).
        /// Must be paired with EndText().
        /// </summary>
        public static void BeginText(this HpdfPage page)
        {
            var stream = page.Contents.Stream;
            stream.WriteString("BT\n");

            // Initialize text matrix to identity
            page.TextMatrix = new HpdfTransMatrix(1, 0, 0, 1, 0, 0);
            page.TextLineMatrix = new HpdfTransMatrix(1, 0, 0, 1, 0, 0);
        }

        /// <summary>
        /// Ends a text object (ET operator).
        /// </summary>
        public static void EndText(this HpdfPage page)
        {
            var stream = page.Contents.Stream;
            stream.WriteString("ET\n");
        }

        // Text State Operations

        /// <summary>
        /// Sets the character spacing (Tc operator).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="charSpace">Character spacing in user space units.</param>
        public static void SetCharSpace(this HpdfPage page, float charSpace)
        {
            var stream = page.Contents.Stream;
            stream.WriteReal(charSpace);
            stream.WriteString(" Tc\n");

            page.GraphicsState.CharSpace = charSpace;
        }

        /// <summary>
        /// Sets the word spacing (Tw operator).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="wordSpace">Word spacing in user space units.</param>
        public static void SetWordSpace(this HpdfPage page, float wordSpace)
        {
            var stream = page.Contents.Stream;
            stream.WriteReal(wordSpace);
            stream.WriteString(" Tw\n");

            page.GraphicsState.WordSpace = wordSpace;
        }

        /// <summary>
        /// Sets the horizontal scaling (Tz operator).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="scale">Horizontal scaling as a percentage (100 = normal).</param>
        public static void SetHorizontalScaling(this HpdfPage page, float scale)
        {
            if (scale <= 0)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Horizontal scaling must be positive");

            var stream = page.Contents.Stream;
            stream.WriteReal(scale);
            stream.WriteString(" Tz\n");

            page.GraphicsState.HorizontalScaling = scale;
        }

        /// <summary>
        /// Sets the text leading (TL operator).
        /// Leading is the vertical distance between baselines.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="leading">Text leading in user space units.</param>
        public static void SetTextLeading(this HpdfPage page, float leading)
        {
            var stream = page.Contents.Stream;
            stream.WriteReal(leading);
            stream.WriteString(" TL\n");

            page.GraphicsState.TextLeading = leading;
        }

        /// <summary>
        /// Sets the font and size (Tf operator).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="size">The font size in points.</param>
        public static void SetFontAndSize(this HpdfPage page, HpdfFont font, float size)
        {
            if (font == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Font cannot be null");
            if (size <= 0)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Font size must be positive");

            // Add font to page resources if not already added
            page.AddFontResource(font);

            var stream = page.Contents.Stream;
            stream.WriteEscapedName(font.LocalName);
            stream.WriteChar(' ');
            stream.WriteReal(size);
            stream.WriteString(" Tf\n");

            page.CurrentFont = font;
            page.GraphicsState.FontSize = size;
        }

        /// <summary>
        /// Sets the text rendering mode (Tr operator).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="mode">The rendering mode.</param>
        public static void SetTextRenderingMode(this HpdfPage page, HpdfTextRenderingMode mode)
        {
            var stream = page.Contents.Stream;
            stream.WriteInt((int)mode);
            stream.WriteString(" Tr\n");

            page.GraphicsState.RenderingMode = (int)mode;
        }

        /// <summary>
        /// Sets the text rise (Ts operator).
        /// Rise is the vertical distance from the baseline.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="rise">Text rise in user space units.</param>
        public static void SetTextRise(this HpdfPage page, float rise)
        {
            var stream = page.Contents.Stream;
            stream.WriteReal(rise);
            stream.WriteString(" Ts\n");

            page.GraphicsState.TextRise = rise;
        }

        // Text Positioning Operations

        /// <summary>
        /// Moves to the start of the next line (Td operator).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="tx">Horizontal translation.</param>
        /// <param name="ty">Vertical translation.</param>
        public static void MoveTextPos(this HpdfPage page, float tx, float ty)
        {
            var stream = page.Contents.Stream;
            stream.WriteReal(tx);
            stream.WriteChar(' ');
            stream.WriteReal(ty);
            stream.WriteString(" Td\n");

            // Update text line matrix
            var tm = page.TextLineMatrix;
            page.TextLineMatrix = new HpdfTransMatrix(tm.A, tm.B, tm.C, tm.D, tm.X + tx, tm.Y + ty);
            page.TextMatrix = page.TextLineMatrix;
        }

        /// <summary>
        /// Moves to the start of the next line and sets leading (TD operator).
        /// Equivalent to: SetTextLeading(-ty); MoveTextPos(tx, ty);
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="tx">Horizontal translation.</param>
        /// <param name="ty">Vertical translation.</param>
        public static void MoveTextPosAndSetLeading(this HpdfPage page, float tx, float ty)
        {
            var stream = page.Contents.Stream;
            stream.WriteReal(tx);
            stream.WriteChar(' ');
            stream.WriteReal(ty);
            stream.WriteString(" TD\n");

            // Set leading
            page.GraphicsState.TextLeading = -ty;

            // Update text line matrix
            var tm = page.TextLineMatrix;
            page.TextLineMatrix = new HpdfTransMatrix(tm.A, tm.B, tm.C, tm.D, tm.X + tx, tm.Y + ty);
            page.TextMatrix = page.TextLineMatrix;
        }

        /// <summary>
        /// Sets the text matrix and text line matrix (Tm operator).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="matrix">The transformation matrix.</param>
        public static void SetTextMatrix(this HpdfPage page, HpdfTransMatrix matrix)
        {
            var stream = page.Contents.Stream;
            stream.WriteReal(matrix.A);
            stream.WriteChar(' ');
            stream.WriteReal(matrix.B);
            stream.WriteChar(' ');
            stream.WriteReal(matrix.C);
            stream.WriteChar(' ');
            stream.WriteReal(matrix.D);
            stream.WriteChar(' ');
            stream.WriteReal(matrix.X);
            stream.WriteChar(' ');
            stream.WriteReal(matrix.Y);
            stream.WriteString(" Tm\n");

            page.TextMatrix = matrix;
            page.TextLineMatrix = matrix;
        }

        /// <summary>
        /// Moves to the start of the next line (T* operator).
        /// Uses the text leading set by SetTextLeading().
        /// </summary>
        public static void MoveToNextLine(this HpdfPage page)
        {
            var stream = page.Contents.Stream;
            stream.WriteString("T*\n");

            // Move down by text leading
            var leading = page.GraphicsState.TextLeading;
            var tm = page.TextLineMatrix;
            page.TextLineMatrix = new HpdfTransMatrix(tm.A, tm.B, tm.C, tm.D, tm.X, tm.Y - leading);
            page.TextMatrix = page.TextLineMatrix;
        }

        // Text Showing Operations

        /// <summary>
        /// Shows a text string (Tj operator).
        /// Text is encoded using the current font's encoding (for TrueType fonts) or PDFDocEncoding (for standard fonts).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="text">The text to show.</param>
        public static void ShowText(this HpdfPage page, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var stream = page.Contents.Stream;

            // Use font's encoding if it has one (TrueType fonts)
            if (page.CurrentFont?.EncodingCodePage.HasValue == true)
            {
                stream.WriteEscapedText(text, page.CurrentFont.EncodingCodePage.Value);
            }
            else
            {
                // Standard fonts use PDFDocEncoding
                stream.WriteEscapedText(text);
            }

            stream.WriteString(" Tj\n");
        }

        /// <summary>
        /// Moves to the next line and shows text (combined operator).
        /// Equivalent to: MoveToNextLine(); ShowText(text);
        /// Text is encoded using the current font's encoding (for TrueType fonts) or PDFDocEncoding (for standard fonts).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="text">The text to show.</param>
        public static void ShowTextNextLine(this HpdfPage page, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var stream = page.Contents.Stream;

            // Use font's encoding if it has one (TrueType fonts)
            if (page.CurrentFont?.EncodingCodePage.HasValue == true)
            {
                stream.WriteEscapedText(text, page.CurrentFont.EncodingCodePage.Value);
            }
            else
            {
                // Standard fonts use PDFDocEncoding
                stream.WriteEscapedText(text);
            }

            stream.WriteString(" '\n");

            // Move to next line
            var leading = page.GraphicsState.TextLeading;
            var tm = page.TextLineMatrix;
            page.TextLineMatrix = new HpdfTransMatrix(tm.A, tm.B, tm.C, tm.D, tm.X, tm.Y - leading);
            page.TextMatrix = page.TextLineMatrix;
        }

        /// <summary>
        /// Sets word and character spacing, moves to next line, and shows text (" operator).
        /// Text is encoded using the current font's encoding (for TrueType fonts) or PDFDocEncoding (for standard fonts).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="wordSpace">Word spacing.</param>
        /// <param name="charSpace">Character spacing.</param>
        /// <param name="text">The text to show.</param>
        public static void SetSpacingAndShowText(this HpdfPage page, float wordSpace, float charSpace, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var stream = page.Contents.Stream;
            stream.WriteReal(wordSpace);
            stream.WriteChar(' ');
            stream.WriteReal(charSpace);
            stream.WriteChar(' ');

            // Use font's encoding if it has one (TrueType fonts)
            if (page.CurrentFont?.EncodingCodePage.HasValue == true)
            {
                stream.WriteEscapedText(text, page.CurrentFont.EncodingCodePage.Value);
            }
            else
            {
                // Standard fonts use PDFDocEncoding
                stream.WriteEscapedText(text);
            }

            stream.WriteString(" \"\n");

            // Update state
            page.GraphicsState.WordSpace = wordSpace;
            page.GraphicsState.CharSpace = charSpace;

            // Move to next line
            var leading = page.GraphicsState.TextLeading;
            var tm = page.TextLineMatrix;
            page.TextLineMatrix = new HpdfTransMatrix(tm.A, tm.B, tm.C, tm.D, tm.X, tm.Y - leading);
            page.TextMatrix = page.TextLineMatrix;
        }
    }
}
