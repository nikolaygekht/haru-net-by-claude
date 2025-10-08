/*
 * << Haru Free PDF Library >> -- HpdfPageSize.cs
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

namespace Haru.Doc
{
    /// <summary>
    /// Predefined page sizes for PDF documents.
    /// </summary>
    public enum HpdfPageSize
    {
        /// <summary>
        /// Letter size (8.5" x 11") = 612 x 792 points.
        /// </summary>
        Letter = 0,

        /// <summary>
        /// Legal size (8.5" x 14") = 612 x 1008 points.
        /// </summary>
        Legal,

        /// <summary>
        /// A3 size (297mm x 420mm) = 841.89 x 1190.55 points.
        /// </summary>
        A3,

        /// <summary>
        /// A4 size (210mm x 297mm) = 595.28 x 841.89 points.
        /// </summary>
        A4,

        /// <summary>
        /// A5 size (148mm x 210mm) = 419.53 x 595.28 points.
        /// </summary>
        A5,

        /// <summary>
        /// B4 size (250mm x 353mm) = 708.66 x 1000.63 points.
        /// </summary>
        B4,

        /// <summary>
        /// B5 size (176mm x 250mm) = 498.90 x 708.66 points.
        /// </summary>
        B5,

        /// <summary>
        /// Executive size (7.25" x 10.5") = 522 x 756 points.
        /// </summary>
        Executive,

        /// <summary>
        /// US 4x6 size = 288 x 432 points.
        /// </summary>
        US4x6,

        /// <summary>
        /// US 4x8 size = 288 x 576 points.
        /// </summary>
        US4x8,

        /// <summary>
        /// US 5x7 size = 360 x 504 points.
        /// </summary>
        US5x7,

        /// <summary>
        /// Commercial #10 Envelope = 297 x 684 points.
        /// </summary>
        Comm10
    }

    /// <summary>
    /// Page orientation.
    /// </summary>
    public enum HpdfPageDirection
    {
        /// <summary>
        /// Portrait orientation (height > width).
        /// </summary>
        Portrait = 0,

        /// <summary>
        /// Landscape orientation (width > height).
        /// </summary>
        Landscape
    }

    /// <summary>
    /// Helper class for page size values.
    /// </summary>
    public static class HpdfPageSizes
    {
        private struct PageSizeValue
        {
            public float Width;
            public float Height;

            public PageSizeValue(float width, float height)
            {
                Width = width;
                Height = height;
            }
        }

        private static readonly PageSizeValue[] PredefinedSizes = new[]
        {
            new PageSizeValue(612, 792),        // Letter
            new PageSizeValue(612, 1008),       // Legal
            new PageSizeValue(841.89f, 1190.551f), // A3
            new PageSizeValue(595.276f, 841.89f),  // A4
            new PageSizeValue(419.528f, 595.276f), // A5
            new PageSizeValue(708.661f, 1000.63f), // B4
            new PageSizeValue(498.898f, 708.661f), // B5
            new PageSizeValue(522, 756),        // Executive
            new PageSizeValue(288, 432),        // US4x6
            new PageSizeValue(288, 576),        // US4x8
            new PageSizeValue(360, 504),        // US5x7
            new PageSizeValue(297, 684)         // Comm10
        };

        /// <summary>
        /// Gets the width and height for a predefined page size.
        /// </summary>
        /// <param name="size">The page size.</param>
        /// <param name="direction">The page orientation.</param>
        /// <param name="width">Output: the page width in points.</param>
        /// <param name="height">Output: the page height in points.</param>
        public static void GetSize(HpdfPageSize size, HpdfPageDirection direction, out float width, out float height)
        {
            var sizeValue = PredefinedSizes[(int)size];

            if (direction == HpdfPageDirection.Portrait)
            {
                width = sizeValue.Width;
                height = sizeValue.Height;
            }
            else // Landscape
            {
                width = sizeValue.Height;
                height = sizeValue.Width;
            }
        }
    }
}
