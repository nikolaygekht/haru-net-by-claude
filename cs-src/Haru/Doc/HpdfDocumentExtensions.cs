/*
 * << Haru Free PDF Library >> -- HpdfDocumentExtensions.cs
 *
 * Extension methods for HpdfDocument for API compatibility
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 */

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
        /// <param name="encodingName">The encoding name (can be null for standard encoding). Currently not used as all fonts use WinAnsiEncoding.</param>
        /// <returns>A new HpdfFont instance.</returns>
        public static HpdfFont GetFont(this HpdfDocument document, string fontName, string? encodingName = null)
        {
            if (document is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Document cannot be null");
            if (string.IsNullOrEmpty(fontName))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Font name cannot be null or empty");

            // Check if this font was previously loaded (both standard and custom fonts are cached)
            if (document.FontRegistry.TryGetValue(fontName, out var cachedFont))
            {
                return cachedFont;
            }

            // Map font names to standard fonts
            var standardFont = MapFontName(fontName);

            // Generate a unique local name for this font
            var localName = $"F{document.Xref.Entries.Count}";

            // Create the font and cache it in the registry to avoid duplicates
            var font = new HpdfFont(document.Xref, standardFont, localName);
            document.FontRegistry[fontName] = font;

            return font;
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
