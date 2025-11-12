/*
 * << Haru Free PDF Library >> -- HpdfDocumentExtensions.cs
 *
 * Extension methods for HpdfDocument for API compatibility
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 */

using System;
using Haru.Font;
using Haru.Types;

namespace Haru.Doc
{
    /// <summary>
    /// Extension methods for HpdfDocument providing convenience methods for API compatibility.
    /// </summary>
    public static class HpdfDocumentExtensions
    {
        /// <summary>
        /// Gets or creates a standard font by name.
        /// This is a convenience method for API compatibility with the original library.
        /// </summary>
        /// <param name="document">The PDF document.</param>
        /// <param name="fontName">The name of the standard font (e.g., "Helvetica", "Courier", "Times-Roman").</param>
        /// <param name="encodingName">The encoding name (e.g., "WinAnsiEncoding", "CP1251", "ISO8859-5").</param>
        /// <returns>A new HpdfFont instance.</returns>
        public static HpdfFont GetFont(this HpdfDocument document, string fontName, string? encodingName = null)
        {
            if (document is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Document cannot be null");
            if (string.IsNullOrEmpty(fontName))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Font name cannot be null or empty");

            int codePage = ParseEncodingToCodePage(encodingName);
            string cacheKey = $"{fontName}#{codePage}";

            if (document.FontRegistry.TryGetValue(cacheKey, out var cachedFont))
            {
                return cachedFont;
            }

            if (document.FontRegistry.TryGetValue(fontName, out var loadedFont))
            {
                return loadedFont;
            }

            var standardFont = MapFontName(fontName);
            var localName = $"F{document.Xref.Entries.Count}";
            var font = new HpdfFont(document.Xref, standardFont, localName, codePage);
            document.FontRegistry[cacheKey] = font;

            return font;
        }

        /// <summary>
        /// Parses an encoding name to a code page number.
        /// </summary>
        private static int ParseEncodingToCodePage(string? encodingName)
        {
            if (string.IsNullOrEmpty(encodingName) ||
                encodingName == "WinAnsiEncoding" ||
                encodingName == "StandardEncoding")
            {
                return 1252; // Default WinAnsiEncoding
            }

            // Handle UTF-8 explicitly (consistent with HpdfDocument.ParseEncodingToCodePage)
            if (encodingName.Equals("UTF-8", System.StringComparison.OrdinalIgnoreCase) ||
                encodingName.Equals("UTF8", System.StringComparison.OrdinalIgnoreCase))
            {
                return 65001;
            }

            // Handle CPxxxx format (e.g., "CP1251")
            if (encodingName.StartsWith("CP", System.StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(encodingName.Substring(2), out int cpNumber))
                    return cpNumber;
            }

            // Handle ISO8859-x format
            if (encodingName.StartsWith("ISO8859", System.StringComparison.OrdinalIgnoreCase))
            {
                string numberPart = encodingName.Substring(encodingName.IndexOf('-') + 1);
                if (int.TryParse(numberPart, out int isoNumber))
                {
                    // ISO-8859-1 = CP28591, ISO-8859-5 = CP28595, etc.
                    if (isoNumber >= 1 && isoNumber <= 9)
                        return 28590 + isoNumber;
                    else if (isoNumber >= 10 && isoNumber <= 16 && isoNumber != 12)
                        return 28600 + (isoNumber % 10);
                }
            }

            // For other encodings, try to get the code page directly
            try
            {
                var enc = System.Text.Encoding.GetEncoding(encodingName);
                return enc.CodePage;
            }
            catch
            {
                // Fall back to default
                return 1252;
            }
        }

        /// <summary>
        /// Maps a font name string to a HpdfStandardFont enum value.
        /// </summary>
        private static HpdfStandardFont MapFontName(string fontName)
        {
            return fontName switch
            {
                "Courier" => HpdfStandardFont.Courier,
                "Courier-Bold" => HpdfStandardFont.CourierBold,
                "Courier-Oblique" => HpdfStandardFont.CourierOblique,
                "Courier-BoldOblique" => HpdfStandardFont.CourierBoldOblique,
                "Helvetica" => HpdfStandardFont.Helvetica,
                "Helvetica-Bold" => HpdfStandardFont.HelveticaBold,
                "Helvetica-Oblique" => HpdfStandardFont.HelveticaOblique,
                "Helvetica-BoldOblique" => HpdfStandardFont.HelveticaBoldOblique,
                "Times-Roman" => HpdfStandardFont.TimesRoman,
                "Times-Bold" => HpdfStandardFont.TimesBold,
                "Times-Italic" => HpdfStandardFont.TimesItalic,
                "Times-BoldItalic" => HpdfStandardFont.TimesBoldItalic,
                "Symbol" => HpdfStandardFont.Symbol,
                "ZapfDingbats" => HpdfStandardFont.ZapfDingbats,
                _ => throw new HpdfException(HpdfErrorCode.InvalidParameter, $"Unknown font name: {fontName}")
            };
        }

        /// <summary>
        /// Sets the compression mode for the document.
        /// This controls which types of content will be compressed in the PDF.
        /// </summary>
        /// <param name="document">The PDF document.</param>
        /// <param name="mode">The compression mode flags.</param>
        public static void SetCompressionMode(this HpdfDocument document, HpdfCompressionMode mode)
        {
            if (document is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Document cannot be null");

            document.CompressionMode = mode;
        }

        /// <summary>
        /// Sets the compression mode for the document using uint flags (P/Invoke compatibility).
        /// </summary>
        /// <param name="document">The PDF document.</param>
        /// <param name="mode">The compression mode flags as uint.</param>
        public static void SetCompressionMode(this HpdfDocument document, uint mode)
        {
            SetCompressionMode(document, (HpdfCompressionMode)mode);
        }

        /// <summary>
        /// Loads a PNG image from a file (convenience method for API compatibility).
        /// </summary>
        /// <param name="document">The PDF document.</param>
        /// <param name="filePath">Path to the PNG image file.</param>
        /// <returns>The loaded HpdfImage instance.</returns>
        public static HpdfImage LoadPngImageFromFile(this HpdfDocument document, string filePath)
        {
            if (document is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Document cannot be null");

            // Generate a unique local name for this image
            var localName = $"Im{document.Xref.Entries.Count}";

            return HpdfImage.LoadPngImageFromFile(document.Xref, localName, filePath);
        }
    }
}
