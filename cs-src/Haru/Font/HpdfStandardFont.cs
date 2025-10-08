/*
 * << Haru Free PDF Library >> -- HpdfStandardFont.cs
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
    /// The 14 standard PDF fonts that are guaranteed to be available in all PDF viewers.
    /// These fonts do not need to be embedded in the PDF document.
    /// </summary>
    public enum HpdfStandardFont
    {
        /// <summary>
        /// Courier (monospaced, normal weight)
        /// </summary>
        Courier,

        /// <summary>
        /// Courier Bold (monospaced, bold weight)
        /// </summary>
        CourierBold,

        /// <summary>
        /// Courier Oblique (monospaced, italic)
        /// </summary>
        CourierOblique,

        /// <summary>
        /// Courier Bold Oblique (monospaced, bold + italic)
        /// </summary>
        CourierBoldOblique,

        /// <summary>
        /// Helvetica (sans-serif, normal weight)
        /// </summary>
        Helvetica,

        /// <summary>
        /// Helvetica Bold (sans-serif, bold weight)
        /// </summary>
        HelveticaBold,

        /// <summary>
        /// Helvetica Oblique (sans-serif, italic)
        /// </summary>
        HelveticaOblique,

        /// <summary>
        /// Helvetica Bold Oblique (sans-serif, bold + italic)
        /// </summary>
        HelveticaBoldOblique,

        /// <summary>
        /// Times Roman (serif, normal weight)
        /// </summary>
        TimesRoman,

        /// <summary>
        /// Times Bold (serif, bold weight)
        /// </summary>
        TimesBold,

        /// <summary>
        /// Times Italic (serif, italic)
        /// </summary>
        TimesItalic,

        /// <summary>
        /// Times Bold Italic (serif, bold + italic)
        /// </summary>
        TimesBoldItalic,

        /// <summary>
        /// Symbol (special characters and Greek letters)
        /// </summary>
        Symbol,

        /// <summary>
        /// ZapfDingbats (decorative symbols and dingbats)
        /// </summary>
        ZapfDingbats
    }

    /// <summary>
    /// Helper methods for standard fonts
    /// </summary>
    public static class HpdfStandardFontExtensions
    {
        /// <summary>
        /// Gets the PostScript name for a standard font
        /// </summary>
        public static string GetPostScriptName(this HpdfStandardFont font)
        {
            return font switch
            {
                HpdfStandardFont.Courier => "Courier",
                HpdfStandardFont.CourierBold => "Courier-Bold",
                HpdfStandardFont.CourierOblique => "Courier-Oblique",
                HpdfStandardFont.CourierBoldOblique => "Courier-BoldOblique",
                HpdfStandardFont.Helvetica => "Helvetica",
                HpdfStandardFont.HelveticaBold => "Helvetica-Bold",
                HpdfStandardFont.HelveticaOblique => "Helvetica-Oblique",
                HpdfStandardFont.HelveticaBoldOblique => "Helvetica-BoldOblique",
                HpdfStandardFont.TimesRoman => "Times-Roman",
                HpdfStandardFont.TimesBold => "Times-Bold",
                HpdfStandardFont.TimesItalic => "Times-Italic",
                HpdfStandardFont.TimesBoldItalic => "Times-BoldItalic",
                HpdfStandardFont.Symbol => "Symbol",
                HpdfStandardFont.ZapfDingbats => "ZapfDingbats",
                _ => "Helvetica"
            };
        }
    }
}
