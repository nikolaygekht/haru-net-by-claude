/*
 * << Haru Free PDF Library >> -- HpdfCompatExtensions.cs
 *
 * Compatibility extension methods for P/Invoke wrapper API
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 */

using System;
using System.IO;
using Haru.Annotations;
using Haru.Font;
using Haru.Types;

namespace Haru.Doc
{
    /// <summary>
    /// Extension methods providing API compatibility with the P/Invoke wrapper (HPdf namespace).
    /// These methods bridge naming and API differences between the native wrapper and pure C# implementation.
    /// </summary>
    public static class HpdfCompatExtensions
    {
        #region HpdfDocument Save/Dispose Compatibility

        /// <summary>
        /// Saves the document to a stream (P/Invoke compatibility: SaveToStream → Save).
        /// </summary>
        public static void SaveToStream(this HpdfDocument doc, Stream stream)
        {
            ArgumentNullException.ThrowIfNull(doc);
            doc.Save(stream);
        }

        /// <summary>
        /// Frees all document resources (P/Invoke compatibility: FreeDocAll → no-op).
        /// In C# version, resources are managed by garbage collection.
        /// </summary>
        public static void FreeDocAll(this HpdfDocument doc)
        {
            // In C# version, this is handled by garbage collection
            // No-op for compatibility
        }

        #endregion

        #region HpdfDocument Font Loading Compatibility

        /// <summary>
        /// Loads a TrueType font from file and returns its name (P/Invoke compatibility).
        /// </summary>
        public static string LoadTTFontFromFile(this HpdfDocument doc, string file, bool embedding)
        {
            ArgumentNullException.ThrowIfNull(doc);
            string localName = $"F{doc.Xref.Entries.Count + 1}";
            var ttFont = HpdfTrueTypeFont.LoadFromFile(doc.Xref, localName, file, embedding);
            var hpdfFont = new HpdfFont(ttFont);

            // Register the font so it can be retrieved later
            doc.FontRegistry[ttFont.BaseFont] = hpdfFont;

            return ttFont.BaseFont;
        }

        /// <summary>
        /// Loads a Type1 font from AFM and PFB files (P/Invoke compatibility).
        /// </summary>
        public static string LoadType1FontFromFile(this HpdfDocument doc, string afmFile, string pfbFile)
        {
            ArgumentNullException.ThrowIfNull(doc);
            string localName = $"F{doc.Xref.Entries.Count + 1}";
            var type1Font = HpdfType1Font.LoadFromFile(doc.Xref, localName, afmFile, pfbFile);
            var hpdfFont = new HpdfFont(type1Font);

            // Register the font so it can be retrieved later
            doc.FontRegistry[type1Font.BaseFont] = hpdfFont;

            return type1Font.BaseFont;
        }

        #endregion

        #region HpdfDocument Encoder Compatibility (No-op methods)

        /// <summary>
        /// Sets the current encoder (P/Invoke compatibility: no-op in C# version).
        /// C# version handles encodings automatically.
        /// </summary>
        public static void SetCurrentEncoder(this HpdfDocument doc, string encodingName)
        {
            // C# version handles encodings automatically
            // No-op for compatibility
        }

        /// <summary>
        /// Returns whether UTF encodings are supported (P/Invoke compatibility: always returns 1).
        /// C# version always supports UTF encodings.
        /// </summary>
        public static int UseUTFEncodings(this HpdfDocument doc)
        {
            return 1; // Always supported in C# version
        }

        #endregion

        #region HpdfDocument CJK Font Support (No-op methods)

        /// <summary>
        /// Enables Chinese Simplified fonts (P/Invoke compatibility: no-op).
        /// In C# version, use LoadTrueTypeFont with a CJK font file instead.
        /// </summary>
        public static void UseCNSFonts(this HpdfDocument doc)
        {
            // C# version: use LoadTrueTypeFont with CJK font file
            // No-op for compatibility
        }

        /// <summary>
        /// Enables Chinese Simplified encodings (P/Invoke compatibility: no-op).
        /// C# version handles encodings automatically.
        /// </summary>
        public static void UseCNSEncodings(this HpdfDocument doc)
        {
            // C# version handles encodings automatically
            // No-op for compatibility
        }

        /// <summary>
        /// Enables Chinese Traditional fonts (P/Invoke compatibility: no-op).
        /// In C# version, use LoadTrueTypeFont with a CJK font file instead.
        /// </summary>
        public static void UseCNTFonts(this HpdfDocument doc)
        {
            // C# version: use LoadTrueTypeFont with CJK font file
            // No-op for compatibility
        }

        /// <summary>
        /// Enables Chinese Traditional encodings (P/Invoke compatibility: no-op).
        /// C# version handles encodings automatically.
        /// </summary>
        public static void UseCNTEncodings(this HpdfDocument doc)
        {
            // C# version handles encodings automatically
            // No-op for compatibility
        }

        /// <summary>
        /// Enables Japanese fonts (P/Invoke compatibility: no-op).
        /// In C# version, use LoadTrueTypeFont with a CJK font file instead.
        /// </summary>
        public static void UseJPFonts(this HpdfDocument doc)
        {
            // C# version: use LoadTrueTypeFont with CJK font file
            // No-op for compatibility
        }

        /// <summary>
        /// Enables Japanese encodings (P/Invoke compatibility: no-op).
        /// C# version handles encodings automatically.
        /// </summary>
        public static void UseJPEncodings(this HpdfDocument doc)
        {
            // C# version handles encodings automatically
            // No-op for compatibility
        }

        /// <summary>
        /// Enables Korean fonts (P/Invoke compatibility: no-op).
        /// In C# version, use LoadTrueTypeFont with a CJK font file instead.
        /// </summary>
        public static void UseKRFonts(this HpdfDocument doc)
        {
            // C# version: use LoadTrueTypeFont with CJK font file
            // No-op for compatibility
        }

        /// <summary>
        /// Enables Korean encodings (P/Invoke compatibility: no-op).
        /// C# version handles encodings automatically.
        /// </summary>
        public static void UseKREncodings(this HpdfDocument doc)
        {
            // C# version handles encodings automatically
            // No-op for compatibility
        }

        #endregion

        #region HpdfDocument Image Loading Compatibility

        /// <summary>
        /// Loads a PNG image from memory (P/Invoke compatibility).
        /// </summary>
        public static HpdfImage LoadPngImageFromMem(this HpdfDocument doc, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(doc);
            using (var stream = new MemoryStream(data))
            {
                return doc.LoadPngImage(stream);
            }
        }

        /// <summary>
        /// Loads a raw image from memory (P/Invoke compatibility).
        /// </summary>
        public static HpdfImage LoadRawImageFromMem(this HpdfDocument doc, uint width, uint height,
            HpdfColorSpace colorSpace, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(doc);
            string localName = $"Im{doc.Xref.Entries.Count + 1}";
            return HpdfImage.LoadRawImageFromMem(doc.Xref, localName, data, width, height, colorSpace, 8);
        }

        #endregion

        #region HpdfDocument Page Mode Compatibility

        /// <summary>
        /// Gets the current page mode (P/Invoke compatibility: GetPageMode → Catalog.PageMode property).
        /// </summary>
        public static HpdfPageMode GetPageMode(this HpdfDocument doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            return doc.Catalog.PageMode;
        }

        /// <summary>
        /// Sets the page mode (P/Invoke compatibility: SetPageMode → Catalog.PageMode property).
        /// </summary>
        public static void SetPageMode(this HpdfDocument doc, HpdfPageMode mode)
        {
            ArgumentNullException.ThrowIfNull(doc);
            doc.Catalog.PageMode = mode;
        }

        #endregion

        #region HpdfPage Dimension Compatibility

        /// <summary>
        /// Gets the page height (P/Invoke compatibility: GetHeight() → Height property).
        /// </summary>
        public static float GetHeight(this HpdfPage page)
        {
            ArgumentNullException.ThrowIfNull(page);
            return page.Height;
        }

        /// <summary>
        /// Gets the page width (P/Invoke compatibility: GetWidth() → Width property).
        /// </summary>
        public static float GetWidth(this HpdfPage page)
        {
            ArgumentNullException.ThrowIfNull(page);
            return page.Width;
        }

        #endregion

        #region HpdfPage Color Compatibility (RGB uppercase methods)

        /// <summary>
        /// Sets RGB fill color (P/Invoke compatibility: SetRGBFill → SetRgbFill).
        /// Note the uppercase RGB in method name for P/Invoke compatibility.
        /// </summary>
        public static void SetRGBFill(this HpdfPage page, float r, float g, float b)
        {
            page.SetRgbFill(r, g, b);
        }

        /// <summary>
        /// Sets RGB stroke color (P/Invoke compatibility: SetRGBStroke → SetRgbStroke).
        /// Note the uppercase RGB in method name for P/Invoke compatibility.
        /// </summary>
        public static void SetRGBStroke(this HpdfPage page, float r, float g, float b)
        {
            page.SetRgbStroke(r, g, b);
        }

        #endregion

        #region HpdfPage Link Annotation Compatibility

        /// <summary>
        /// Creates a URI link annotation (P/Invoke compatibility: CreateURILinkAnnot → CreateLinkAnnotation).
        /// </summary>
        public static HpdfLinkAnnotation CreateURILinkAnnot(this HpdfPage page, HpdfRect rect, string uri)
        {
            ArgumentNullException.ThrowIfNull(page);
            return page.CreateLinkAnnotation(rect, uri);
        }

        #endregion

        #region HpdfFont Compatibility

        /// <summary>
        /// Gets the font name (P/Invoke compatibility: GetFontName() → BaseFont property).
        /// </summary>
        public static string GetFontName(this HpdfFont font)
        {
            ArgumentNullException.ThrowIfNull(font);
            return font.BaseFont;
        }

        /// <summary>
        /// Gets the encoding name (P/Invoke compatibility).
        /// Returns the encoding name string representation.
        /// </summary>
        public static string GetEncodingName(this HpdfFont font)
        {
            ArgumentNullException.ThrowIfNull(font);
            // In C# version, return a string representation of the code page
            return font.EncodingCodePage.HasValue && font.EncodingCodePage.Value > 0
                ? $"CP{font.EncodingCodePage.Value}"
                : "StandardEncoding";
        }

        #endregion

        #region HpdfImage Compatibility

        /// <summary>
        /// Gets the image width (P/Invoke compatibility: GetWidth() → Width property).
        /// </summary>
        public static uint GetWidth(this HpdfImage image)
        {
            ArgumentNullException.ThrowIfNull(image);
            return (uint)image.Width;
        }

        /// <summary>
        /// Gets the image height (P/Invoke compatibility: GetHeight() → Height property).
        /// </summary>
        public static uint GetHeight(this HpdfImage image)
        {
            ArgumentNullException.ThrowIfNull(image);
            return (uint)image.Height;
        }

        #endregion

        #region HpdfBox Compatibility (lowercase field names)

        /// <summary>
        /// Gets the left coordinate (P/Invoke compatibility: lowercase 'left').
        /// </summary>
        public static float left(this HpdfBox box) => box.Left;

        /// <summary>
        /// Gets the bottom coordinate (P/Invoke compatibility: lowercase 'bottom').
        /// </summary>
        public static float bottom(this HpdfBox box) => box.Bottom;

        /// <summary>
        /// Gets the right coordinate (P/Invoke compatibility: lowercase 'right').
        /// </summary>
        public static float right(this HpdfBox box) => box.Right;

        /// <summary>
        /// Gets the top coordinate (P/Invoke compatibility: lowercase 'top').
        /// </summary>
        public static float top(this HpdfBox box) => box.Top;

        #endregion

        #region HpdfRgbColor Compatibility (lowercase field names)

        /// <summary>
        /// Gets the red component (P/Invoke compatibility: lowercase 'r').
        /// </summary>
        public static float r(this HpdfRgbColor color) => color.R;

        /// <summary>
        /// Gets the green component (P/Invoke compatibility: lowercase 'g').
        /// </summary>
        public static float g(this HpdfRgbColor color) => color.G;

        /// <summary>
        /// Gets the blue component (P/Invoke compatibility: lowercase 'b').
        /// </summary>
        public static float b(this HpdfRgbColor color) => color.B;

        #endregion
    }
}
