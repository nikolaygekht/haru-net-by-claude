/*
 * << Haru Free PDF Library >> -- HpdfPageExtensions.cs
 *
 * Extension methods for HpdfPage for API compatibility
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 */

using System;
using Haru.Font;
using Haru.Graphics;
using Haru.Streams;
using Haru.Types;

namespace Haru.Doc
{
    /// <summary>
    /// Extension methods for HpdfPage providing convenience methods for API compatibility.
    /// </summary>
    public static class HpdfPageExtensions
    {
        /// <summary>
        /// Creates a destination for the current page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>A new destination targeting this page.</returns>
        public static HpdfDestination CreateDestination(this HpdfPage page)
        {
            return new HpdfDestination(page);
        }

        /// <summary>
        /// Outputs text at the specified position (convenience method).
        /// This is equivalent to: BeginText() + MoveTextPos() + ShowText() + EndText()
        /// </summary>
        public static void TextOut(this HpdfPage page, float x, float y, string text)
        {
            page.BeginText();
            page.MoveTextPos(x, y);
            page.ShowText(text);
            page.EndText();
        }

        /// <summary>
        /// Measures the width of text in the current font and size.
        /// </summary>
        public static float TextWidth(this HpdfPage page, string text)
        {
            if (page.CurrentFont == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "No font has been set");

            return page.CurrentFont.MeasureText(text, page.GraphicsState.FontSize);
        }

        /// <summary>
        /// Gets the current font size.
        /// </summary>
        public static float GetCurrentFontSize(this HpdfPage page)
        {
            return page.GraphicsState.FontSize;
        }

        /// <summary>
        /// Gets the current RGB fill color.
        /// </summary>
        public static HpdfRgbColor GetRGBFill(this HpdfPage page)
        {
            return page.GraphicsState.RgbFill;
        }

        /// <summary>
        /// Gets the current RGB stroke color.
        /// </summary>
        public static HpdfRgbColor GetRGBStroke(this HpdfPage page)
        {
            return page.GraphicsState.RgbStroke;
        }

        /// <summary>
        /// Gets the current font.
        /// </summary>
        public static HpdfFont GetCurrentFont(this HpdfPage page)
        {
            return page.CurrentFont;
        }

        /// <summary>
        /// Sets the text matrix (convenience overload accepting 6 parameters).
        /// </summary>
        public static void SetTextMatrix(this HpdfPage page, float a, float b, float c, float d, float x, float y)
        {
            var matrix = new HpdfTransMatrix(a, b, c, d, x, y);
            page.SetTextMatrix(matrix);
        }

        /// <summary>
        /// Executes an XObject (typically an image) directly on the page.
        /// Note: You should use GSave/GRestore and set up transformation matrix before calling this.
        /// </summary>
        public static void ExecuteXObject(this HpdfPage page, HpdfImage image)
        {
            if (image == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Image cannot be null");

            // Add image to page resources
            page.AddImageResource(image);

            // Execute the XObject (Do operator)
            var stream = page.Contents.Stream;
            stream.WriteEscapedName(image.LocalName);
            stream.WriteString(" Do\n");
        }

        /// <summary>
        /// Sets the page height (convenience method for API compatibility).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="height">The height in points.</param>
        public static void SetHeight(this HpdfPage page, float height)
        {
            page.Height = height;
        }

        /// <summary>
        /// Sets the page width (convenience method for API compatibility).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="width">The width in points.</param>
        public static void SetWidth(this HpdfPage page, float width)
        {
            page.Width = width;
        }
    }
}
