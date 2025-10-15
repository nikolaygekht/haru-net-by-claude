/*
 * << Haru Free PDF Library >> -- FontMetrics.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using Haru.Types;

namespace Haru.Font.CID
{
    /// <summary>
    /// Contains font metrics for predefined CID fonts.
    /// All measurements are in 1000-unit glyph space.
    /// </summary>
    public class FontMetrics
    {
        /// <summary>
        /// Gets or sets the font ascent (maximum height above baseline).
        /// </summary>
        public int Ascent { get; set; }

        /// <summary>
        /// Gets or sets the font descent (maximum depth below baseline, negative value).
        /// </summary>
        public int Descent { get; set; }

        /// <summary>
        /// Gets or sets the cap height (height of capital letters).
        /// </summary>
        public int CapHeight { get; set; }

        /// <summary>
        /// Gets or sets the font bounding box.
        /// </summary>
        public HpdfBox FontBBox { get; set; }

        /// <summary>
        /// Gets or sets the font flags (SYMBOLIC, FIXED_WIDTH, SERIF, etc.).
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the italic angle in degrees (0 for upright, negative for slant).
        /// </summary>
        public int ItalicAngle { get; set; }

        /// <summary>
        /// Gets or sets the stem vertical thickness.
        /// </summary>
        public int StemV { get; set; }

        /// <summary>
        /// Gets or sets the default width for characters not in the width array.
        /// Default is 1000 for CJK fonts.
        /// </summary>
        public int DefaultWidth { get; set; } = 1000;
    }
}
