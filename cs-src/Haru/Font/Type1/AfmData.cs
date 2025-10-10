/*
 * << Haru Free PDF Library >> -- AfmData.cs
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

using Haru.Types;

namespace Haru.Font.Type1
{
    /// <summary>
    /// Character metrics from AFM file.
    /// </summary>
    public class AfmCharMetric
    {
        /// <summary>
        /// Character code (-1 for unmapped).
        /// </summary>
        public int CharCode { get; set; }

        /// <summary>
        /// Character width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// PostScript glyph name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unicode value for this glyph.
        /// </summary>
        public ushort Unicode { get; set; }
    }

    /// <summary>
    /// Adobe Font Metrics (AFM) data structure.
    /// Represents font metrics parsed from an AFM file.
    /// </summary>
    public class AfmData
    {
        /// <summary>
        /// Font name (PostScript name).
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Full font name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Font family name.
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// Font weight (e.g., "Bold", "Book", "Regular").
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// Italic angle in degrees.
        /// </summary>
        public float ItalicAngle { get; set; }

        /// <summary>
        /// Is fixed pitch font.
        /// </summary>
        public bool IsFixedPitch { get; set; }

        /// <summary>
        /// Underline position.
        /// </summary>
        public int UnderlinePosition { get; set; }

        /// <summary>
        /// Underline thickness.
        /// </summary>
        public int UnderlineThickness { get; set; }

        /// <summary>
        /// Font version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Encoding scheme (e.g., "AdobeStandardEncoding").
        /// </summary>
        public string EncodingScheme { get; set; }

        /// <summary>
        /// Font bounding box.
        /// </summary>
        public HpdfRect FontBBox { get; set; }

        /// <summary>
        /// Cap height.
        /// </summary>
        public int CapHeight { get; set; }

        /// <summary>
        /// X-height.
        /// </summary>
        public int XHeight { get; set; }

        /// <summary>
        /// Ascender.
        /// </summary>
        public int Ascender { get; set; }

        /// <summary>
        /// Descender (typically negative).
        /// </summary>
        public int Descender { get; set; }

        /// <summary>
        /// Standard horizontal stem width (StemH).
        /// </summary>
        public int StdHW { get; set; }

        /// <summary>
        /// Standard vertical stem width (StemV).
        /// </summary>
        public int StdVW { get; set; }

        /// <summary>
        /// Character set name.
        /// </summary>
        public string CharacterSet { get; set; }

        /// <summary>
        /// Character metrics (glyph data).
        /// </summary>
        public AfmCharMetric[] CharMetrics { get; set; }

        /// <summary>
        /// Font flags for PDF font descriptor.
        /// </summary>
        public int Flags { get; set; }

        public AfmData()
        {
            FontBBox = new HpdfRect();
        }
    }
}
