using System;
using Haru.Font.TrueType;
using Haru.Font.Type1;

namespace Haru.Font
{
    /// <summary>
    /// Base class for parsed font data that can be shared across multiple document instances.
    /// This enables font pooling - loading font files once and reusing the parsed data.
    /// </summary>
    public abstract class ParsedFontData
    {
        /// <summary>
        /// Absolute file path where this font was loaded from (for cache key).
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// When this font data was loaded (for cache management).
        /// </summary>
        public DateTime LoadedAt { get; }

        protected ParsedFontData(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            LoadedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Holds parsed TrueType font data that can be shared across multiple HpdfDocument instances.
    /// The font file is loaded once into memory and cached for reuse.
    /// </summary>
    public sealed class ParsedTrueTypeData : ParsedFontData
    {
        /// <summary>
        /// Raw font file data (needed for embedding in PDF and for parsing).
        /// This is the complete TTF/OTF file content loaded from disk.
        /// </summary>
        public byte[] FontData { get; }

        /// <summary>
        /// Extracted font name (PostScript name or Family-Subfamily).
        /// Cached here to avoid re-extracting on every font instance creation.
        /// </summary>
        public string ExtractedFontName { get; }

        /// <summary>
        /// Code page for text encoding (e.g., 437 for standard ASCII, 932 for Japanese, 936 for Chinese Simplified).
        /// </summary>
        public int CodePage { get; }

        public ParsedTrueTypeData(
            string filePath,
            byte[] fontData,
            string extractedFontName,
            int codePage)
            : base(filePath)
        {
            FontData = fontData ?? throw new ArgumentNullException(nameof(fontData));
            ExtractedFontName = extractedFontName ?? throw new ArgumentNullException(nameof(extractedFontName));
            CodePage = codePage;
        }
    }

    /// <summary>
    /// Holds parsed Type1 font data that can be shared across multiple HpdfDocument instances.
    /// Type1 fonts consist of two files: AFM (Adobe Font Metrics) and PFB (Printer Font Binary).
    /// </summary>
    public sealed class ParsedType1Data : ParsedFontData
    {
        /// <summary>
        /// Path to the AFM (Adobe Font Metrics) file.
        /// </summary>
        public string AfmPath { get; }

        /// <summary>
        /// Path to the PFB (Printer Font Binary) file.
        /// </summary>
        public string PfbPath { get; }

        /// <summary>
        /// Parsed AFM data (font metrics).
        /// </summary>
        public AfmData AfmData { get; }

        /// <summary>
        /// Raw PFB data (font program).
        /// </summary>
        public byte[] PfbData { get; }

        /// <summary>
        /// Font name extracted from AFM (usually from FontName field).
        /// </summary>
        public string ExtractedFontName { get; }

        public ParsedType1Data(
            string afmPath,
            string pfbPath,
            AfmData afmData,
            byte[] pfbData,
            string extractedFontName)
            : base(afmPath) // Use AFM path as primary key
        {
            AfmPath = afmPath ?? throw new ArgumentNullException(nameof(afmPath));
            PfbPath = pfbPath ?? throw new ArgumentNullException(nameof(pfbPath));
            AfmData = afmData ?? throw new ArgumentNullException(nameof(afmData));
            PfbData = pfbData ?? throw new ArgumentNullException(nameof(pfbData));
            ExtractedFontName = extractedFontName ?? throw new ArgumentNullException(nameof(extractedFontName));
        }
    }
}
