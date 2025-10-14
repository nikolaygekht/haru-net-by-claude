/*
 * << Haru Free PDF Library >> -- HpdfStandardFontMetrics.cs
 *
 * Standard font metrics for the PDF Standard 14 fonts
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using Haru.Types;

namespace Haru.Font
{
    /// <summary>
    /// Metrics data for a standard PDF font.
    /// All values are in 1000-unit glyph space (typical for Type 1 fonts).
    /// </summary>
    public class StandardFontMetrics
    {
        public int Ascent { get; set; }
        public int Descent { get; set; }
        public int CapHeight { get; set; }
        public int XHeight { get; set; }
        public HpdfBox FontBBox { get; set; }

        public StandardFontMetrics(int ascent, int descent, int capHeight, int xHeight,
            float left, float bottom, float right, float top)
        {
            Ascent = ascent;
            Descent = descent;
            CapHeight = capHeight;
            XHeight = xHeight;
            FontBBox = new HpdfBox(left, bottom, right, top);
        }
    }

    /// <summary>
    /// Provides font metrics for the PDF Standard 14 fonts.
    /// Metrics sourced from Adobe Font Metrics (AFM) files.
    /// </summary>
    public static class HpdfStandardFontMetrics
    {
        public static StandardFontMetrics GetMetrics(HpdfStandardFont font)
        {
            return font switch
            {
                // Helvetica family - sans-serif
                HpdfStandardFont.Helvetica => new StandardFontMetrics(
                    ascent: 718, descent: -207, capHeight: 718, xHeight: 523,
                    left: -166, bottom: -225, right: 1000, top: 931),

                HpdfStandardFont.HelveticaBold => new StandardFontMetrics(
                    ascent: 718, descent: -207, capHeight: 718, xHeight: 532,
                    left: -170, bottom: -228, right: 1003, top: 962),

                HpdfStandardFont.HelveticaOblique => new StandardFontMetrics(
                    ascent: 718, descent: -207, capHeight: 718, xHeight: 523,
                    left: -170, bottom: -225, right: 1116, top: 931),

                HpdfStandardFont.HelveticaBoldOblique => new StandardFontMetrics(
                    ascent: 718, descent: -207, capHeight: 718, xHeight: 532,
                    left: -174, bottom: -228, right: 1114, top: 962),

                // Times family - serif
                HpdfStandardFont.TimesRoman => new StandardFontMetrics(
                    ascent: 683, descent: -217, capHeight: 662, xHeight: 450,
                    left: -168, bottom: -218, right: 1000, top: 898),

                HpdfStandardFont.TimesBold => new StandardFontMetrics(
                    ascent: 683, descent: -217, capHeight: 676, xHeight: 461,
                    left: -168, bottom: -218, right: 1000, top: 935),

                HpdfStandardFont.TimesItalic => new StandardFontMetrics(
                    ascent: 683, descent: -217, capHeight: 653, xHeight: 441,
                    left: -169, bottom: -217, right: 1010, top: 883),

                HpdfStandardFont.TimesBoldItalic => new StandardFontMetrics(
                    ascent: 683, descent: -217, capHeight: 669, xHeight: 462,
                    left: -200, bottom: -218, right: 996, top: 921),

                // Courier family - monospace
                HpdfStandardFont.Courier => new StandardFontMetrics(
                    ascent: 629, descent: -157, capHeight: 562, xHeight: 426,
                    left: -23, bottom: -250, right: 715, top: 805),

                HpdfStandardFont.CourierBold => new StandardFontMetrics(
                    ascent: 626, descent: -142, capHeight: 562, xHeight: 439,
                    left: -113, bottom: -250, right: 749, top: 801),

                HpdfStandardFont.CourierOblique => new StandardFontMetrics(
                    ascent: 629, descent: -157, capHeight: 562, xHeight: 426,
                    left: -27, bottom: -250, right: 849, top: 805),

                HpdfStandardFont.CourierBoldOblique => new StandardFontMetrics(
                    ascent: 626, descent: -142, capHeight: 562, xHeight: 439,
                    left: -57, bottom: -250, right: 869, top: 801),

                // Symbol font - special
                HpdfStandardFont.Symbol => new StandardFontMetrics(
                    ascent: 1010, descent: -293, capHeight: 673, xHeight: 478,
                    left: -180, bottom: -293, right: 1090, top: 1010),

                // ZapfDingbats - special
                HpdfStandardFont.ZapfDingbats => new StandardFontMetrics(
                    ascent: 820, descent: -143, capHeight: 820, xHeight: 0,
                    left: -1, bottom: -143, right: 981, top: 820),

                _ => new StandardFontMetrics(
                    ascent: 750, descent: -250, capHeight: 700, xHeight: 500,
                    left: 0, bottom: -250, right: 1000, top: 750)
            };
        }
    }
}
