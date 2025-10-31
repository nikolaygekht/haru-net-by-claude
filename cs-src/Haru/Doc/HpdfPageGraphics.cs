/*
 * << Haru Free PDF Library >> -- HpdfPageGraphics.cs
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
using Haru.Graphics;
using Haru.Types;
using Haru.Streams;

namespace Haru.Doc
{
    /// <summary>
    /// Graphics operations for IDrawable objects (pages, appearance streams, etc.)
    /// </summary>
    public static class HpdfPageGraphics
    {
        // Graphics State Operations

        /// <summary>
        /// Saves the current graphics state (q operator)
        /// </summary>
        public static void GSave(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("q\n");

            // Push graphics state onto stack
            drawable.PushGraphicsState();
        }

        /// <summary>
        /// Restores the previous graphics state (Q operator)
        /// </summary>
        public static void GRestore(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("Q\n");

            // Pop graphics state from stack
            drawable.PopGraphicsState();
        }

        // Line Attributes

        /// <summary>
        /// Sets the line width (w operator)
        /// </summary>
        public static void SetLineWidth(this IDrawable drawable, float width)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            if (width < 0)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Line width must be non-negative");

            var stream = drawable.Stream;
            stream.WriteReal(width);
            stream.WriteString(" w\n");

            drawable.GraphicsState.LineWidth = width;
        }

        /// <summary>
        /// Sets the line cap style (J operator)
        /// </summary>
        public static void SetLineCap(this IDrawable drawable, HpdfLineCap lineCap)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteInt((int)lineCap);
            stream.WriteString(" J\n");

            drawable.GraphicsState.LineCap = lineCap;
        }

        /// <summary>
        /// Sets the line join style (j operator)
        /// </summary>
        public static void SetLineJoin(this IDrawable drawable, HpdfLineJoin lineJoin)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteInt((int)lineJoin);
            stream.WriteString(" j\n");

            drawable.GraphicsState.LineJoin = lineJoin;
        }

        /// <summary>
        /// Sets the miter limit (M operator)
        /// </summary>
        public static void SetMiterLimit(this IDrawable drawable, float miterLimit)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            if (miterLimit < 1)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Miter limit must be >= 1");

            var stream = drawable.Stream;
            stream.WriteReal(miterLimit);
            stream.WriteString(" M\n");

            drawable.GraphicsState.MiterLimit = miterLimit;
        }

        /// <summary>
        /// Sets the dash pattern (d operator)
        /// Pass null or empty array to clear dash pattern (solid line)
        /// </summary>
        public static void SetDash(this IDrawable drawable, ushort[]? pattern, uint phase)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;

            // Null or empty pattern means solid line (clear dash pattern)
            if (pattern == null || pattern.Length == 0)
            {
                stream.WriteString("[] 0 d\n");
                drawable.GraphicsState.DashMode = new HpdfDashMode(Array.Empty<ushort>(), 0);
                return;
            }

            if (pattern.Length > HpdfDashMode.MaxPatternLength)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Pattern too long");

            stream.WriteChar('[');
            for (int i = 0; i < pattern.Length; i++)
            {
                if (i > 0) stream.WriteChar(' ');
                stream.WriteInt(pattern[i]);
            }
            stream.WriteString("] ");
            stream.WriteInt((int)phase);
            stream.WriteString(" d\n");

            drawable.GraphicsState.DashMode = new HpdfDashMode(pattern, phase);
        }

        // Path Construction

        /// <summary>
        /// Begins a new subpath at (x, y) (m operator)
        /// </summary>
        public static void MoveTo(this IDrawable drawable, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteReal(x);
            stream.WriteChar(' ');
            stream.WriteReal(y);
            stream.WriteString(" m\n");

            drawable.CurrentPos = new HpdfPoint(x, y);
        }

        /// <summary>
        /// Appends a straight line segment from current point to (x, y) (l operator)
        /// </summary>
        public static void LineTo(this IDrawable drawable, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteReal(x);
            stream.WriteChar(' ');
            stream.WriteReal(y);
            stream.WriteString(" l\n");

            drawable.CurrentPos = new HpdfPoint(x, y);
        }

        /// <summary>
        /// Appends a rectangle to the current path (re operator)
        /// </summary>
        public static void Rectangle(this IDrawable drawable, float x, float y, float width, float height)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteReal(x);
            stream.WriteChar(' ');
            stream.WriteReal(y);
            stream.WriteChar(' ');
            stream.WriteReal(width);
            stream.WriteChar(' ');
            stream.WriteReal(height);
            stream.WriteString(" re\n");
        }

        /// <summary>
        /// Closes the current subpath (h operator)
        /// </summary>
        public static void ClosePath(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("h\n");
        }

        // Path Painting

        /// <summary>
        /// Strokes the current path (S operator)
        /// </summary>
        public static void Stroke(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("S\n");
        }

        /// <summary>
        /// Closes and strokes the current path (s operator)
        /// </summary>
        public static void ClosePathStroke(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("s\n");
        }

        /// <summary>
        /// Fills the current path using nonzero winding rule (f operator)
        /// </summary>
        public static void Fill(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("f\n");
        }

        /// <summary>
        /// Fills the current path using even-odd rule (f* operator)
        /// </summary>
        public static void EoFill(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("f*\n");
        }

        /// <summary>
        /// Fills and then strokes the current path (B operator)
        /// </summary>
        public static void FillStroke(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("B\n");
        }

        /// <summary>
        /// Closes, fills, and strokes the current path (b operator)
        /// </summary>
        public static void ClosePathFillStroke(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("b\n");
        }

        /// <summary>
        /// Ends the path without filling or stroking (n operator)
        /// </summary>
        public static void EndPath(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("n\n");
        }

        // Color Operations

        /// <summary>
        /// Sets the grayscale fill color (g operator)
        /// </summary>
        public static void SetGrayFill(this IDrawable drawable, float gray)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            if (gray < 0 || gray > 1)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Gray value must be 0-1");

            var stream = drawable.Stream;
            stream.WriteReal(gray);
            stream.WriteString(" g\n");

            drawable.GraphicsState.GrayFill = gray;
            drawable.GraphicsState.FillColorSpace = HpdfColorSpace.DeviceGray;
        }

        /// <summary>
        /// Sets the grayscale stroke color (G operator)
        /// </summary>
        public static void SetGrayStroke(this IDrawable drawable, float gray)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            if (gray < 0 || gray > 1)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Gray value must be 0-1");

            var stream = drawable.Stream;
            stream.WriteReal(gray);
            stream.WriteString(" G\n");

            drawable.GraphicsState.GrayStroke = gray;
            drawable.GraphicsState.StrokeColorSpace = HpdfColorSpace.DeviceGray;
        }

        /// <summary>
        /// Sets the RGB fill color (rg operator)
        /// </summary>
        public static void SetRgbFill(this IDrawable drawable, float r, float g, float b)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            if (r < 0 || r > 1 || g < 0 || g > 1 || b < 0 || b > 1)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "RGB values must be 0-1");

            var stream = drawable.Stream;
            stream.WriteReal(r);
            stream.WriteChar(' ');
            stream.WriteReal(g);
            stream.WriteChar(' ');
            stream.WriteReal(b);
            stream.WriteString(" rg\n");

            drawable.GraphicsState.RgbFill = new HpdfRgbColor(r, g, b);
            drawable.GraphicsState.FillColorSpace = HpdfColorSpace.DeviceRgb;
        }

        /// <summary>
        /// Sets the RGB stroke color (RG operator)
        /// </summary>
        public static void SetRgbStroke(this IDrawable drawable, float r, float g, float b)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            if (r < 0 || r > 1 || g < 0 || g > 1 || b < 0 || b > 1)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "RGB values must be 0-1");

            var stream = drawable.Stream;
            stream.WriteReal(r);
            stream.WriteChar(' ');
            stream.WriteReal(g);
            stream.WriteChar(' ');
            stream.WriteReal(b);
            stream.WriteString(" RG\n");

            drawable.GraphicsState.RgbStroke = new HpdfRgbColor(r, g, b);
            drawable.GraphicsState.StrokeColorSpace = HpdfColorSpace.DeviceRgb;
        }

        /// <summary>
        /// Sets the CMYK fill color (k operator)
        /// </summary>
        public static void SetCmykFill(this IDrawable drawable, float c, float m, float y, float k)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            if (c < 0 || c > 1 || m < 0 || m > 1 || y < 0 || y > 1 || k < 0 || k > 1)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "CMYK values must be 0-1");

            var stream = drawable.Stream;
            stream.WriteReal(c);
            stream.WriteChar(' ');
            stream.WriteReal(m);
            stream.WriteChar(' ');
            stream.WriteReal(y);
            stream.WriteChar(' ');
            stream.WriteReal(k);
            stream.WriteString(" k\n");

            drawable.GraphicsState.CmykFill = new HpdfCmykColor(c, m, y, k);
            drawable.GraphicsState.FillColorSpace = HpdfColorSpace.DeviceCmyk;
        }

        /// <summary>
        /// Sets the CMYK stroke color (K operator)
        /// </summary>
        public static void SetCmykStroke(this IDrawable drawable, float c, float m, float y, float k)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            if (c < 0 || c > 1 || m < 0 || m > 1 || y < 0 || y > 1 || k < 0 || k > 1)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "CMYK values must be 0-1");

            var stream = drawable.Stream;
            stream.WriteReal(c);
            stream.WriteChar(' ');
            stream.WriteReal(m);
            stream.WriteChar(' ');
            stream.WriteReal(y);
            stream.WriteChar(' ');
            stream.WriteReal(k);
            stream.WriteString(" K\n");

            drawable.GraphicsState.CmykStroke = new HpdfCmykColor(c, m, y, k);
            drawable.GraphicsState.StrokeColorSpace = HpdfColorSpace.DeviceCmyk;
        }

        // Advanced Path Construction - Bezier Curves

        /// <summary>
        /// Appends a cubic Bezier curve to the current path (c operator).
        /// The curve extends from the current point to (x3, y3) using (x1, y1) and (x2, y2) as control points.
        /// </summary>
        public static void CurveTo(this IDrawable drawable, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteReal(x1);
            stream.WriteChar(' ');
            stream.WriteReal(y1);
            stream.WriteChar(' ');
            stream.WriteReal(x2);
            stream.WriteChar(' ');
            stream.WriteReal(y2);
            stream.WriteChar(' ');
            stream.WriteReal(x3);
            stream.WriteChar(' ');
            stream.WriteReal(y3);
            stream.WriteString(" c\n");

            drawable.CurrentPos = new HpdfPoint(x3, y3);
        }

        /// <summary>
        /// Appends a cubic Bezier curve to the current path (v operator).
        /// The curve extends from the current point to (x3, y3) using the current point and (x2, y2) as control points.
        /// </summary>
        public static void CurveTo2(this IDrawable drawable, float x2, float y2, float x3, float y3)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteReal(x2);
            stream.WriteChar(' ');
            stream.WriteReal(y2);
            stream.WriteChar(' ');
            stream.WriteReal(x3);
            stream.WriteChar(' ');
            stream.WriteReal(y3);
            stream.WriteString(" v\n");

            drawable.CurrentPos = new HpdfPoint(x3, y3);
        }

        /// <summary>
        /// Appends a cubic Bezier curve to the current path (y operator).
        /// The curve extends from the current point to (x3, y3) using (x1, y1) and (x3, y3) as control points.
        /// </summary>
        public static void CurveTo3(this IDrawable drawable, float x1, float y1, float x3, float y3)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteReal(x1);
            stream.WriteChar(' ');
            stream.WriteReal(y1);
            stream.WriteChar(' ');
            stream.WriteReal(x3);
            stream.WriteChar(' ');
            stream.WriteReal(y3);
            stream.WriteString(" y\n");

            drawable.CurrentPos = new HpdfPoint(x3, y3);
        }

        // Transformation Matrix

        /// <summary>
        /// Concatenates a transformation matrix to the current transformation matrix (cm operator).
        /// </summary>
        public static void Concat(this IDrawable drawable, float a, float b, float c, float d, float x, float y)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteReal(a);
            stream.WriteChar(' ');
            stream.WriteReal(b);
            stream.WriteChar(' ');
            stream.WriteReal(c);
            stream.WriteChar(' ');
            stream.WriteReal(d);
            stream.WriteChar(' ');
            stream.WriteReal(x);
            stream.WriteChar(' ');
            stream.WriteReal(y);
            stream.WriteString(" cm\n");
        }

        /// <summary>
        /// Concatenates a transformation matrix to the current transformation matrix (cm operator).
        /// </summary>
        public static void Concat(this IDrawable drawable, HpdfTransMatrix matrix)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            drawable.Concat(matrix.A, matrix.B, matrix.C, matrix.D, matrix.X, matrix.Y);
        }

        // Clipping Paths

        /// <summary>
        /// Modifies the current clipping path using the nonzero winding number rule (W operator).
        /// </summary>
        public static void Clip(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("W\n");
        }

        /// <summary>
        /// Modifies the current clipping path using the even-odd rule (W* operator).
        /// </summary>
        public static void EoClip(this IDrawable drawable)
        {
            ArgumentNullException.ThrowIfNull(drawable);
            var stream = drawable.Stream;
            stream.WriteString("W*\n");
        }

        // Extended Graphics State

        /// <summary>
        /// Sets the extended graphics state (gs operator).
        /// </summary>
        public static void SetExtGState(this HpdfPage page, HpdfExtGState extGState)
        {
            ArgumentNullException.ThrowIfNull(page);
            if (extGState is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Extended graphics state cannot be null");

            // Add to page resources
            page.AddExtGStateResource(extGState);

            var stream = page.Stream;
            stream.WriteEscapedName(extGState.LocalName);
            stream.WriteString(" gs\n");
        }

        // Image Operations

        /// <summary>
        /// Draws an image at the specified position and size (Do operator).
        /// The image is drawn with transformations to position and scale it correctly.
        /// </summary>
        /// <param name="page">The page to draw on.</param>
        /// <param name="image">The image to draw.</param>
        /// <param name="x">X coordinate of lower-left corner.</param>
        /// <param name="y">Y coordinate of lower-left corner.</param>
        /// <param name="width">Width of the image in user space units.</param>
        /// <param name="height">Height of the image in user space units.</param>
        public static void DrawImage(this HpdfPage page, HpdfImage image, float x, float y, float width, float height)
        {
            ArgumentNullException.ThrowIfNull(page);
            if (image is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Image cannot be null");
            if (width <= 0 || height <= 0)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Image dimensions must be positive");

            // Add image to page resources
            page.AddImageResource(image);

            // Save graphics state
            page.GSave();

            // Transform: scale to width/height and translate to x/y
            // Image XObjects are defined in a 1x1 unit square, so we need to scale and position
            page.Concat(width, 0, 0, height, x, y);

            // Execute the XObject (Do operator)
            var stream = page.Stream;
            stream.WriteEscapedName(image.LocalName);
            stream.WriteString(" Do\n");

            // Restore graphics state
            page.GRestore();
        }
    }
}
